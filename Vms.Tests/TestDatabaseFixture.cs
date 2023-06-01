using Microsoft.EntityFrameworkCore;
using Vms.Tests;

namespace VmsTesting;

public class TestDatabaseFixture
{
    private const string ConnectionString = 
        "Server=SKYLAKE\\SQL2019;Database=Vms_Test;Trusted_Connection=True;MultipleActiveResultSets=true;Encrypt=False";

    private static readonly object _lock = new();
    private static bool _databaseInitialized;

    public TestDatabaseFixture()
    {
        lock (_lock)
        {
            if (!_databaseInitialized)
            {
                using (var context = CreateContext())
                {
                    context.Database.EnsureDeleted();
                    context.Database.EnsureCreated();

                    var company = new Company("TEST001", "Test Company");
                    context.Add(company);

                    var make1 = new VehicleMake("SUBARU");
                    context.VehicleMakes.Add(make1);

                    var model1_1 = new VehicleModel(make1.Make, "IMPREZA WRX");
                    context.VehicleModels.Add(model1_1);

                    // seed
                    var vehicle1 = new Vehicle("S684FDU", make1.Make, model1_1.Model, new DateOnly(2000, 1, 14), new DateOnly(2000, 1, 14));
                    
                    //, new List<Driver>()
                    //{
                    //    new Driver() { Salutation = "Mr", FirstName = "Mark", LastName = "Baldwin", EmailAddress = "markb@utopiasoftware.co.uk" }
                    //});
                    
                    
                    context.Add(vehicle1);

                    context.SaveChanges();
                }

                _databaseInitialized = true;
            }
        }
    }

    public static VmsDbContext CreateContext()
        => new(new DbContextOptionsBuilder<VmsDbContext>()
                .UseSqlServer(ConnectionString, x=>
                {
                    x.UseNetTopologySuite();
                    x.UseDateOnlyTimeOnly();
                })
                .Options, new UserProvider());
}