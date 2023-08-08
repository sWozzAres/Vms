using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Vms.Tests;

namespace VmsTesting;

public class TestDatabaseFixture : IAsyncLifetime
{
    private const string ConnectionString =
        "Server=SKYLAKE\\SQL2019;Database=Vms_Test;Trusted_Connection=True;MultipleActiveResultSets=true;Encrypt=False";

    //private static readonly object _lock = new();
    private static readonly bool _databaseInitialized;

    private static readonly SemaphoreSlim semaphore = new(1, 1);



    /// <summary>
    /// 3857    WGS 84 / Pseudo-Mercator (Used by Google Maps, Open Street Map) - Metre
    /// 4326    WGS-84 (World-wide; used by GPS) - Degrees
    /// </summary>
    const int Srid = 4326;

    public async Task InitializeAsync()
    {
        // Wait for the semaphore
        //await semaphore.WaitAsync();
        //try
        //{
        //    if (!_databaseInitialized)
        //    {
        //        using (var context = CreateContext())
        //        {

        //            context.Database.EnsureDeleted();
        //            context.Database.EnsureCreated();

        //            using var transaction = await context.BeginTransactionAsync() ?? throw new InvalidOperationException("Transaction cannot begin.");

        //            var company = new CreateCompany(context).Create(new("TEST001", "Test Company"));

        //            var customer1 = await new CreateCustomer(context).CreateAsync(new(company.Code, "CUS001", "Customer #1"));
        //            var customer2 = await new CreateCustomer(context).CreateAsync(new(company.Code, "CUS002", "Customer #2"));

        //            var network1 = await new CreateNetwork(context).CreateAsync(new(company.Code, "NET001", "Network #1"));
        //            var network2 = await new CreateNetwork(context).CreateAsync(new(company.Code, "NET002", "Network #2"));

        //            var fleet1 = await new CreateFleet(context).CreateAsync(new(company.Code, "FLEET001", "Fleet #1"));
        //            var fleet2 = await new CreateFleet(context).CreateAsync(new(company.Code, "FLEET002", "Fleet #2"));

        //            var make1 = new VehicleMake("SUBARU");
        //            await context.AddAsync(make1);

        //            var model1_1 = new VehicleModel(make1.Make, "IMPREZA WRX");
        //            await context.AddAsync(model1_1);

        //            var vehicle1 = await new CreateVehicle(context).CreateAsync(
        //                new(company.Code, "HK52YUL", make1.Make, model1_1.Model, new DateOnly(2000, 1, 14), new DateOnly(2000, 1, 14),
        //                    new Address("11A Marsh Lane", "Leonard Stanley", "Stonehouse", "GL103NJ", new Point(-2.2834790077963936, 51.728985639485806) { SRID = Srid }),
        //                    customer1.Code, fleet1.Code));

        //            // https://www.yell.com/s/garage+services-stroud-gloucestershire.html
        //            var supplier1 = new Supplier("SUP001", "Warwick Car Co",
        //                new Address("Unit 3 Fromeside Ind Est", "Dr Newtons Way", "Stroud", "GL53JX", new Point(-2.217656486566628, 51.7426960366928) { SRID = Srid }),
        //                false);

        //            var supplier2 = new Supplier("SUP002", "Kings Auto & Motorcycle Centre",
        //                new Address("Church Street", "Kings Stanley", "Stonehouse", "GL103HT", new Point(-2.2744656660315745, 51.7329182056337) { SRID = Srid }),
        //                false);

        //            var supplier3 = new Supplier("SUP003", "Thrupp Tyre Co Ltd",
        //                new Address("Unit 12 Griffin Mill", "London Rd", "Stroud", "GL52AZ", new Point(-2.204629225004565, 51.7301105207617) { SRID = Srid }),
        //                false);
        //            await context.AddRangeAsync(new[] { supplier1, supplier2, supplier3 });

        //            await context.AddRangeAsync(new[] {
        //                new NetworkSupplier(network1.CompanyCode, network1.Code, supplier1.Code),
        //                new NetworkSupplier(network2.CompanyCode, network2.Code, supplier2.Code)
        //            });

        //            CustomerNetwork cn1 = new(customer1.CompanyCode, customer1.Code, network1.Code);
        //            await context.AddAsync(cn1);

        //            FleetNetwork fn1 = new(customer2.CompanyCode, fleet1.Code, network2.Code);
        //            await context.AddAsync(fn1);



        //            //, new List<Driver>()
        //            //{
        //            //    new Driver() { Salutation = "Mr", FirstName = "Mark", LastName = "Baldwin", EmailAddress = "markb@utopiasoftware.co.uk" }
        //            //});


        //            //await context.SaveChangesAsync();


        //            await context.CommitTransactionAsync(transaction);

        //        }

        //        _databaseInitialized = true;
        //    }
        //}
        //finally
        //{
        //    // Release the semaphore
        //    semaphore.Release();
        //}
    }

    //public TestDatabaseFixture()
    //{
    //    lock (_lock)
    //    {
    //        if (!_databaseInitialized)
    //        {
    //            using (var context = CreateContext())
    //            {
    //                context.Database.EnsureDeleted();
    //                context.Database.EnsureCreated();

    //                Company company = await new CreateCompany(context).CreateAsync(new("TEST001", "Test Company"));


    //                var customer1 = new CreateCustomer(context).Create(new(company.Id, "CUS001", "Customer #1"));
    //                var customer2 = new CreateCustomer(context).Create(new(company.Id, "CUS002", "Customer #2"));
    //                //context.AddRange(new[] { customer1, customer2 });

    //                var make1 = new VehicleMake("SUBARU");
    //                context.VehicleMakes.Add(make1);

    //                var model1_1 = new VehicleModel(make1.Make, "IMPREZA WRX");
    //                context.VehicleModels.Add(model1_1);

    //                // seed
    //                var vehicle1 = new Vehicle("HK52YUL", make1.Make, model1_1.Model, new DateOnly(2000, 1, 14), new DateOnly(2000, 1, 14));

    //                //, new List<Driver>()
    //                //{
    //                //    new Driver() { Salutation = "Mr", FirstName = "Mark", LastName = "Baldwin", EmailAddress = "markb@utopiasoftware.co.uk" }
    //                //});


    //                context.Add(vehicle1);

    //                context.SaveChanges();
    //            }

    //            _databaseInitialized = true;
    //        }
    //    }
    //}

    public static VmsDbContext CreateContext()
    {
        var factory = new LoggerFactory();
        var logger = factory.CreateLogger<VmsDbContext>();

        return new(new DbContextOptionsBuilder<VmsDbContext>()
                .UseSqlServer(ConnectionString, x =>
                {
                    x.UseNetTopologySuite();
                    x.UseDateOnlyTimeOnly();
                })
                .Options, new UserProvider(), logger);
    }


    public Task DisposeAsync() => Task.CompletedTask;
}