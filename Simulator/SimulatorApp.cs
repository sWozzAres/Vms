using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Vms.Application.Commands.CompanyUseCase;
using Vms.Application.Commands;
using Vms.Domain.Common;
using Vms.Domain.Infrastructure;
using NetTopologySuite.Geometries;
using Simulator.Services;
using Utopia.Api.Application.Services;
using Vms.Application.Queries;
using Utopia.Blazor.Application.Vms.Shared;
using Vms.Application.Commands.VehicleUseCase;
using Microsoft.EntityFrameworkCore;
using Utopia.Api.Domain.System;
using Utopia.Api.Services;
using static System.Formats.Asn1.AsnWriter;

namespace Simulator
{
    public static class ServiceScopeExtensions
    {
        public static T New<T>(this IServiceScope scope) where T : notnull
            => scope.ServiceProvider.GetRequiredService<T>();
    }

    public class SimulatorApp(IServiceProvider serviceProvider,
        IServiceScopeFactory serviceScopeFactory) : ISimulatorApp
    {
        readonly IServiceScopeFactory ServiceScopeFactory = serviceScopeFactory;
        readonly SimulateTime TimeService = (SimulateTime)serviceProvider.GetRequiredService<ITimeService>();

        async Task InitializeAsync() 
        {
            using (var scope = ServiceScopeFactory.CreateScope())
            {
                var dbContext = scope.New<VmsDbContext>();
                var logger = scope.New<ILogger<ISimulatorApp>>();

                logger.LogInformation("Initializing...");
                dbContext.Database.EnsureDeleted();
                dbContext.Database.EnsureCreated();
            }

            await ConfigureDatabaseAsync();

            async Task ConfigureDatabaseAsync() => await ExecuteInTransactionAsync(async (IServiceScope scope) =>
            {
                const int Srid = 4326;

                var dbContext = scope.New<VmsDbContext>();

                var userProvider = scope.New<IUserProvider>();
                dbContext.Users.Add(new User(userProvider.UserId, userProvider.UserName, userProvider.TenantId, userProvider.EmailAddress));

                var company = scope.New<CreateCompany>().Create(new("TEST001", "Test Company"));

                var customer1 = await scope.New<CreateCustomer>().CreateAsync(new(company.Code, "CUS001", "Customer #1"));
                var customer2 = await scope.New<CreateCustomer>().CreateAsync(new(company.Code, "CUS002", "Customer #2"));

                var network1 = scope.New<CreateNetwork>().CreateAsync(new(company.Code, "NET001", "Network #1"));
                var network2 = scope.New<CreateNetwork>().CreateAsync(new(company.Code, "NET002", "Network #2"));

                var fleet1 = await scope.New<CreateFleet>().CreateAsync(new(company.Code, "FLEET001", "Fleet #1"));
                var fleet2 = await scope.New<CreateFleet>().CreateAsync(new(company.Code, "FLEET002", "Fleet #2"));

                var make1 = scope.New<CreateMake>().Create(new CreateMakeRequest("SUBARU"));

                var model1_1 = await scope.New<CreateModel>().CreateAsync(new CreateModelRequest(make1.Make, "IMPREZA WRX"));

                var vehicle1 = await scope.New<CreateVehicle>().CreateAsync(
                    new(company.Code, "HK52YUL", make1.Make, model1_1.Model,
                        DateOnly.FromDateTime(TimeService.Now).AddDays(-400),
                        DateOnly.FromDateTime(TimeService.Now).AddDays(1),
                        "",
                        new Address("11A Marsh Lane", "Leonard Stanley", "Stonehouse", "GL103NJ", new Point(-2.2834790077963936, 51.728985639485806) { SRID = Srid }),
                        customer1.Code, fleet1.Code));

                //// https://www.yell.com/s/garage+services-stroud-gloucestershire.html
                //var supplier1 = scope.New Supplier("SUP001", "Warwick Car Co",
                //    new Address("Unit 3 Fromeside Ind Est", "Dr Newtons Way", "Stroud", "GL53JX", new Point(-2.217656486566628, 51.7426960366928) { SRID = Srid }),
                //    false);

                //var supplier2 = scope.New Supplier("SUP002", "Kings Auto & Motorcycle Centre",
                //    new Address("Church Street", "Kings Stanley", "Stonehouse", "GL103HT", new Point(-2.2744656660315745, 51.7329182056337) { SRID = Srid }),
                //    false);

                //var supplier3 = scope.New Supplier("SUP003", "Thrupp Tyre Co Ltd",
                //    new Address("Unit 12 Griffin Mill", "London Rd", "Stroud", "GL52AZ", new Point(-2.204629225004565, 51.7301105207617) { SRID = Srid }),
                //    false);
                //await context.AddRangeAsync(new[] { supplier1, supplier2, supplier3 });

                //await context.AddRangeAsync(new[] {
                //    new NetworkSupplier(network1.CompanyCode, network1.Code, supplier1.Code),
                //    new NetworkSupplier(network2.CompanyCode, network2.Code, supplier2.Code)
                //});

                //CustomerNetwork cn1 = scope.New(customer1.CompanyCode, customer1.Code, network1.Code);
                //await context.AddAsync(cn1);

                //FleetNetwork fn1 = scope.New(customer2.CompanyCode, fleet1.Code, network2.Code);
                //await context.AddAsync(fn1);



                //, new List<Driver>()
                //{
                //    new Driver() { Salutation = "Mr", FirstName = "Mark", LastName = "Baldwin", EmailAddress = "markb@utopiasoftware.co.uk" }
                //});

                await dbContext.SaveChangesAsync();
            });
        }

        async Task CreateBookings() => await ExecuteInTransactionAsync(async (IServiceScope scope) =>
        {
            var logger = scope.New<ILogger<ISimulatorApp>>();

            var vehicleQueries = scope.New<VehicleQueries>();

            var (TotalCount, Result) = await vehicleQueries.GetVehicles(VehicleListOptions.DueMot, 0, 10, CancellationToken.None);

            logger.LogInformation("Found {count} vehicles with overdue Mots.", TotalCount);

            foreach (var vehicle in Result)
            {
                var serviceBooking = await scope.New<CreateServiceBooking>().CreateAsync(new(
                        vehicle.Id,
                        DateOnly.FromDateTime(TimeService.Now).AddDays(1),
                        null,
                        null,
                        ServiceLevelDto.Collection,
                        true,
                        vehicle.MotId,
                        null,
                        null
                    ), CancellationToken.None);

                logger.LogInformation("Service booking {serviceBookingId} created for vrm {vrm}.", serviceBooking.Id, vehicle.Vrm);
            }
        });

        //async Task AssignBookings()
        //{
        //    //var bookingQueries = scope.New<ServiceBookingQueries>();

        //    //var result = await bookingQueries.GetServiceBookings(ServiceBookingListOptions.Due, 0, 10, CancellationToken.None);
        //}

        public async Task Run()
        {
            TimeService.SetTime(new DateTime(2020, 1, 1));

            await InitializeAsync();

            await CreateBookings();
        }

        async Task ExecuteInTransactionAsync(Func<IServiceScope, Task> action)
        {
            using var scope = ServiceScopeFactory.CreateScope();
            var dbContext = scope.New<VmsDbContext>();

            await dbContext.Database.CreateExecutionStrategy().ExecuteAsync(async () =>
            {
                using var transaction = dbContext.Database.BeginTransaction();

                await action(scope);

                await dbContext.SaveChangesAsync();
                await transaction.CommitAsync();
            });
        }
    }
}
