using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NetTopologySuite.Geometries;
using Simulator.Services;
using Utopia.Api.Application.Services;
using Utopia.Api.Domain.System;
using Utopia.Api.Services;
using Utopia.Blazor.Application.Vms.Shared;
using Vms.Application.Commands;
using Vms.Application.Commands.CompanyUseCase;
using Vms.Application.Commands.VehicleUseCase;
using Vms.Application.Queries;
using Vms.Domain.Common;
using Vms.Domain.Infrastructure;

namespace Simulator
{
    public class SimulatorApp(
        IServiceProvider serviceProvider,
        IServiceScopeFactory serviceScopeFactory,
        ILogger<SimulatorApp> logger) : ISimulatorApp
    {
        readonly SimulateTime TimeService = (SimulateTime)serviceProvider.GetRequiredService<ITimeService>();

        const int Srid = 4326;

        IServiceScope? TransactionScope;
        T New<T>() where T : notnull => TransactionScope == null
            ? throw new InvalidOperationException($"{nameof(TransactionScope)} is null.")
            : TransactionScope.ServiceProvider.GetRequiredService<T>();

        void CreateDatabase()
        {
            logger.LogInformation("Creeating database...");

            var dbContext = serviceProvider.GetRequiredService<VmsDbContext>();
            dbContext.Database.EnsureDeleted();
            dbContext.Database.EnsureCreated();
        }

        async Task ConfigureDatabaseAsync() => await ExecuteInTransactionAsync(async () =>
        {
            logger.LogInformation("Configuring database...");

            var dbContext = New<VmsDbContext>();

            var userProvider = New<IUserProvider>();
            dbContext.Users.Add(new User(userProvider.UserId, userProvider.UserName, userProvider.TenantId, userProvider.EmailAddress));

            var company = New<CreateCompany>().Create(new(userProvider.TenantId, "Test Company"));

            var customer1 = await New<CreateCustomer>().CreateAsync(new(company.Code, "CUS001", "Customer #1"));
            var customer2 = await New<CreateCustomer>().CreateAsync(new(company.Code, "CUS002", "Customer #2"));

            var network1 = New<CreateNetwork>().CreateAsync(new(company.Code, "NET001", "Network #1"));
            var network2 = New<CreateNetwork>().CreateAsync(new(company.Code, "NET002", "Network #2"));

            var fleet1 = await New<CreateFleet>().CreateAsync(new(company.Code, "FLEET001", "Fleet #1"));
            var fleet2 = await New<CreateFleet>().CreateAsync(new(company.Code, "FLEET002", "Fleet #2"));

            var make1 = New<CreateMake>().Create(new CreateMakeRequest("SUBARU"));

            var model1_1 = await New<CreateModel>().CreateAsync(new CreateModelRequest(make1.Make, "IMPREZA WRX"));

            var vehicle1 = await New<CreateVehicle>().CreateAsync(
                new(company.Code, "HK52YUL", make1.Make, model1_1.Model,
                    DateOnly.FromDateTime(TimeService.Now).AddDays(-400),
                    DateOnly.FromDateTime(TimeService.Now).AddDays(1),
                    "",
                    new Address("11A Marsh Lane", "Leonard Stanley", "Stonehouse", "GL103NJ", new Point(-2.2834790077963936, 51.728985639485806) { SRID = Srid }),
                    customer1.Code, fleet1.Code));

            //// https://www.yell.com/s/garage+services-stroud-gloucestershire.html
            //var supplier1 = New Supplier("SUP001", "Warwick Car Co",
            //    new Address("Unit 3 Fromeside Ind Est", "Dr Newtons Way", "Stroud", "GL53JX", new Point(-2.217656486566628, 51.7426960366928) { SRID = Srid }),
            //    false);

            //var supplier2 = New Supplier("SUP002", "Kings Auto & Motorcycle Centre",
            //    new Address("Church Street", "Kings Stanley", "Stonehouse", "GL103HT", new Point(-2.2744656660315745, 51.7329182056337) { SRID = Srid }),
            //    false);

            //var supplier3 = New Supplier("SUP003", "Thrupp Tyre Co Ltd",
            //    new Address("Unit 12 Griffin Mill", "London Rd", "Stroud", "GL52AZ", new Point(-2.204629225004565, 51.7301105207617) { SRID = Srid }),
            //    false);
            //await context.AddRangeAsync(new[] { supplier1, supplier2, supplier3 });

            //await context.AddRangeAsync(new[] {
            //    new NetworkSupplier(network1.CompanyCode, network1.Code, supplier1.Code),
            //    new NetworkSupplier(network2.CompanyCode, network2.Code, supplier2.Code)
            //});

            //CustomerNetwork cn1 = New(customer1.CompanyCode, customer1.Code, network1.Code);
            //await context.AddAsync(cn1);

            //FleetNetwork fn1 = New(customer2.CompanyCode, fleet1.Code, network2.Code);
            //await context.AddAsync(fn1);



            //, new List<Driver>()
            //{
            //    new Driver() { Salutation = "Mr", FirstName = "Mark", LastName = "Baldwin", EmailAddress = "markb@utopiasoftware.co.uk" }
            //});

            await dbContext.SaveChangesAsync();
        });


        async Task CreateBookings() => await ExecuteInTransactionAsync(async () =>
        {
            var vehicleQueries = New<VehicleQueries>();

            var (totalCount, result) = await vehicleQueries
                .GetVehicles(VehicleListOptions.DueMot, 0, 10, CancellationToken.None);

            logger.LogInformation("Found {count} vehicles with overdue Mots.", totalCount);

            foreach (var vehicle in result)
            {
                var serviceBooking = await New<CreateServiceBooking>().CreateAsync(new(
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
            CreateDatabase();
            await ConfigureDatabaseAsync();

            TimeService.SetTime(new DateTime(2020, 1, 1));
            
            await CreateBookings();
        }

        async Task ExecuteInTransactionAsync(Func<Task> action)
        {
            using var scope = serviceScopeFactory.CreateScope();
            TransactionScope = scope;
            try
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<VmsDbContext>();

                await dbContext.Database.CreateExecutionStrategy().ExecuteAsync(async () =>
                {
                    using var transaction = dbContext.Database.BeginTransaction();

                    await action();

                    await dbContext.SaveChangesAsync();
                    await transaction.CommitAsync();
                });
            }
            finally
            {
                TransactionScope = null;
            }
        }
    }
}
