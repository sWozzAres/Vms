using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Transactions;
using Vms.Domain.UseCase;
using Vms.Tests;

namespace VmsTesting;

public class TestDatabaseFixture
{
    private const string ConnectionString =
        "Server=SKYLAKE\\SQL2019;Database=Vms_Test;Trusted_Connection=True;MultipleActiveResultSets=true;Encrypt=False";

    private static readonly object _lock = new();
    private static bool _databaseInitialized;

    // Create a semaphore with initial count 1 and maximum count 1
    private static SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);

    public static async Task Initialize()
    {
        // Wait for the semaphore asynchronously
        await semaphore.WaitAsync();
        try
        {
            if (!_databaseInitialized)
            {
                using (var context = CreateContext())
                {
                    context.Database.EnsureDeleted();
                    context.Database.EnsureCreated();

                    using var transaction = await context.BeginTransactionAsync();
                    if (transaction is not null)
                    {
                        var company = await new CreateCompany(context).CreateAsync(new("TEST001", "Test Company"));

                        var customer1 = await new CreateCustomer(context).CreateAsync(new(company.Id, "CUS001", "Customer #1"));
                        var customer2 = await new CreateCustomer(context).CreateAsync(new(company.Id, "CUS002", "Customer #2"));

                        var make1 = new VehicleMake("SUBARU");
                        await context.VehicleMakes.AddAsync(make1);

                        var model1_1 = new VehicleModel(make1.Make, "IMPREZA WRX");
                        await context.VehicleModels.AddAsync(model1_1);

                        var vehicle1 = await new CreateVehicle(context).CreateAsync(
                            new(company.Id, "HK52YUL", make1.Make, model1_1.Model, new DateOnly(2000, 1, 14), new DateOnly(2000, 1, 14)));

                        //, new List<Driver>()
                        //{
                        //    new Driver() { Salutation = "Mr", FirstName = "Mark", LastName = "Baldwin", EmailAddress = "markb@utopiasoftware.co.uk" }
                        //});


                        //await context.SaveChangesAsync();

                        await context.CommitTransactionAsync(transaction);
                    }
                }

                _databaseInitialized = true;
            }
        }
        finally
        {
            // Release the semaphore
            semaphore.Release();
        }
    }

    public TestDatabaseFixture()
    {
        lock (_lock)
        {
            //if (!_databaseInitialized)
            //{
            //    using (var context = CreateContext())
            //    {
            //        context.Database.EnsureDeleted();
            //        context.Database.EnsureCreated();

            //        Company company = await new CreateCompany(context).CreateAsync(new("TEST001", "Test Company"));


            //        var customer1 = new CreateCustomer(context).Create(new(company.Id, "CUS001", "Customer #1"));
            //        var customer2 = new CreateCustomer(context).Create(new(company.Id, "CUS002", "Customer #2"));
            //        //context.AddRange(new[] { customer1, customer2 });

            //        var make1 = new VehicleMake("SUBARU");
            //        context.VehicleMakes.Add(make1);

            //        var model1_1 = new VehicleModel(make1.Make, "IMPREZA WRX");
            //        context.VehicleModels.Add(model1_1);

            //        // seed
            //        var vehicle1 = new Vehicle("HK52YUL", make1.Make, model1_1.Model, new DateOnly(2000, 1, 14), new DateOnly(2000, 1, 14));

            //        //, new List<Driver>()
            //        //{
            //        //    new Driver() { Salutation = "Mr", FirstName = "Mark", LastName = "Baldwin", EmailAddress = "markb@utopiasoftware.co.uk" }
            //        //});


            //        context.Add(vehicle1);

            //        context.SaveChanges();
            //    }

            //    _databaseInitialized = true;
            //}
        }
    }

    public static VmsDbContext CreateContext()
        => new(new DbContextOptionsBuilder<VmsDbContext>()
                .UseSqlServer(ConnectionString, x =>
                {
                    x.UseNetTopologySuite();
                    x.UseDateOnlyTimeOnly();
                })
                .Options, new UserProvider());
}