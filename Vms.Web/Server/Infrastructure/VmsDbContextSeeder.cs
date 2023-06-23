using Microsoft.Extensions.Options;
using NetTopologySuite.Geometries;
using Vms.Application.UseCase;
using Vms.Domain.Entity;
using Vms.Web.Server;

namespace Vms.Domain.Infrastructure;

public interface IVmsDbContextSeeder
{
    Task<int> SeedAsync(IWebHostEnvironment env, IOptions<AppSettings> settings);
}

public class VmsDbContextSeeder : IVmsDbContextSeeder
{
    readonly VmsDbContext _context;
    readonly ILogger<VmsDbContextSeeder> _logger;

    public VmsDbContextSeeder(VmsDbContext context, ILogger<VmsDbContextSeeder> logger)
        => (_context, _logger) = (context, logger);

    static readonly string[] SurNames = new string[]
    {
        "Smith", "Jones", "Taylor", "Brown", "Williams", "Wilson", "Johnson", "Davies", "Robinson", "Wright", "Thomson", "Evans",
        "Walker", "White", "Roberts", "Green", "Hall", "Wood", "Jackson", "Clark"
    };

    static readonly string[] MaleFirstNames = new string[]
    {
        "Oliver", "George", "Arthur", "Noah", "Muhammad", "Leo", "Oscar", "Harry", "Archie", "Henry"
    };

    static readonly string[] FemaleFirstNames = new string[]
    {
        "Olivia", "Amelia", "Isla", "Ava", "Mia", "Ivy", "Lily", "Isabella", "Sophia", "Rosie"
    };

    static readonly string[] MakeNames = new string[] { "MERCEDES", "FORD", "SUBARU" };

    static readonly char[] letters = new char[] { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'K', 'L', 'M', 'N', 'O', 'P', 'R', 'S', 'V', 'W', 'X', 'Y' };

    const double minLatitude = 58.88670472493245;
    const double minLongitude = -9.372902643217499;
    const double maxLatitude = 50.26714546722624;
    const double maxLongitude = 2.668112510252122;
    const double deltaLatitude = maxLatitude - minLatitude;
    const double deltaLongitude = maxLongitude - minLongitude;
    static Point RandomPoint() => new(minLongitude + (deltaLongitude * rnd.NextDouble()), minLatitude + (deltaLatitude * rnd.NextDouble())) { SRID = 4326 };

    static readonly Random rnd = new();

    public async Task<int> SeedAsync(IWebHostEnvironment env, IOptions<AppSettings> settings)
    {
        if (_context.Companies.Any())
            return 0;

        var company1 = new CreateCompany(_context)
            .Create(new CreateCompanyRequest("TEST001", "Company #1"));

        var fleet1_1 = await new CreateFleet(_context)
            .CreateAsync(new CreateFleetRequest(company1.Code, "FL00101", "Fleet #1 Company #1"));
        var fleet2_1 = await new CreateFleet(_context)
            .CreateAsync(new CreateFleetRequest(company1.Code, "FL00102", "Fleet #2 Company #1"));

        var network1_1 = await new CreateNetwork(_context)
            .CreateAsync(new CreateNetworkRequest(company1.Code, "NET00101", "Network #1 Company #1"));
        var network2_1 = await new CreateNetwork(_context)
            .CreateAsync(new CreateNetworkRequest(company1.Code, "NET00102", "Network #2 Company #1"));

        var customer1_1 = await new CreateCustomer(_context)
            .CreateAsync(new CreateCustomerRequest(company1.Code, "CUS00101", "Customer #1 Company #1"));
        var customer2_1 = await new CreateCustomer(_context)
            .CreateAsync(new CreateCustomerRequest(company1.Code, "CUS00102", "Customer #2 Company #1"));

        var company2 = new CreateCompany(_context)
            .Create(new CreateCompanyRequest("TEST002", "Company #2"));

        var fleet1_2 = await new CreateFleet(_context)
            .CreateAsync(new CreateFleetRequest(company1.Code, "FL00201", "Fleet #2 Company #1"));

        var network1_2 = await new CreateNetwork(_context)
            .CreateAsync(new CreateNetworkRequest(company1.Code, "NET00201", "Network #2 Company #1"));

        var customer1_2 = await new CreateCustomer(_context)
            .CreateAsync(new CreateCustomerRequest(company1.Code, "CUS00201", "Customer #2 Company #1"));

        if (!_context.Drivers.Any())
        {
            var fullNames = (string[] firstNames) => from surName in SurNames
                                                     from firstName in firstNames
                                                     select (firstName, surName);

            var drivers =
                fullNames(MaleFirstNames).Select(n => new Driver()
                {
                    FirstName = n.firstName,
                    LastName = n.surName,
                    Salutation = "Mr",
                    EmailAddress = GenerateEmailAddress(n.firstName, n.surName),
                    MobileNumber = RandomPhoneNumber(),
                    HomeLocation = RandomPoint()
                })
                .Concat(fullNames(FemaleFirstNames).Select(n => new Driver()
                {
                    FirstName = n.firstName,
                    LastName = n.surName,
                    Salutation = rnd.Next(2) == 0 ? "Ms" : "Mrs",
                    EmailAddress = GenerateEmailAddress(n.firstName, n.surName),
                    MobileNumber = RandomPhoneNumber(),
                    HomeLocation = RandomPoint()
                })
                );

            await _context.Drivers.AddRangeAsync(drivers);

            string RandomPhoneNumber() => "0" + (7700000000 + rnd.Next(99999999)).ToString();
            string GenerateEmailAddress(string firstName, string lastName) => $"{firstName.ToLower()}.{lastName.ToLower()}{DateTime.Now.Year - 20 - rnd.Next(30)}@somewhere.com";
        }

        if (!_context.Suppliers.Any())
        {
            SeedSuppliers();
        }

        // generate make / model data
        var makes = MakeNames.Select(name =>
            new MakeInfo(name,
                Enumerable.Range(1, 100).Select(x => new ModelInfo($"Model #{x}")).ToList()));

        foreach(var makeInfo in makes)
        {
            new CreateMake(_context)
                .Create(new CreateMakeRequest(makeInfo.Make));

            foreach(var modelInfo in makeInfo.Models)
            {
                await new CreateModel(_context)
                    .CreateAsync(new CreateModelRequest(makeInfo.Make, modelInfo.Model));
            }
        }

        //if (!_context.VehicleMakes.Any())
        //{

        //    await _context.VehicleMakes.AddRangeAsync(makes.Select(x => new VehicleMake(x.Make)));
        //}

        IEnumerable<(string Make, string Model)> flattenedMakeModel = from make in makes
                                                                      from model in make.Models
                                                                      select (make.Make, model.Model);

        (string Make, string Model) RandomMakeModel(Random rnd) =>
            flattenedMakeModel.ElementAt(rnd.Next(flattenedMakeModel.Count()));


        //if (!_context.VehicleModels.Any())
        //{
        //    await _context.VehicleModels.AddRangeAsync(flattenedMakeModel.Select(x => new VehicleModel(x.Make, x.Model)));
        //}

        if (!_context.Vehicles.Any())
        {
            foreach (var driver in _context.Drivers)
            {
                var (Make, Model) = RandomMakeModel(rnd);

                DateOnly RandomDate(int minYear, int maxYear)
                {
                    int year = rnd.Next(minYear, maxYear);
                    int month = rnd.Next(12) + 1;
                    int day = rnd.Next(DateTime.DaysInMonth(year, month)) + 1;
                    return new DateOnly(year, month, day);
                }

                var dateFirstRegistered = RandomDate(2001, DateTime.Now.Year - 1);
                var vehicle = Vehicle.Create(company1.Code, RandomVrm(dateFirstRegistered), Make, Model,
                    dateFirstRegistered,
                    DateOnly.FromDateTime(DateTime.Now.AddDays(14 + rnd.Next(28))),
                    new Address("","","","",new Point(51.72816804510823, -2.2832425208311116) { SRID = 4326 }));
                

                var driverVehicle = new DriverVehicle()
                {
                    Vehicle = vehicle,
                    EmailAddress = driver.EmailAddress
                };

                char RandomLetter() => letters[rnd.Next(0, letters.Length)];

                string RandomVrm(DateOnly firstRegistered)
                {
                    var range1 = new DateOnly(firstRegistered.Year, 3, 1);
                    var range2 = new DateOnly(firstRegistered.Year, 8, 31);
                    var add = firstRegistered >= range1 && firstRegistered <= range2
                        ? 0
                        : 50;

                    return $"{RandomLetter()}{RandomLetter()}{firstRegistered.Year - 2000 + add:D2}{RandomLetter()}{RandomLetter()}{RandomLetter()}";
                }

                //var vehicleVrm = new VehicleVrm()
                //{
                //    Vehicle = vehicle,
                //    Vrm = RandomVrm(vehicle.DateFirstRegistered)

                //};

                //var vehicleMot = new NextMot()
                //{
                //    Vehicle = vehicle,
                //    Due = DateOnly.FromDateTime(DateTime.Now.AddDays(14 + rnd.Next(28)))
                //};

                //_context.VehicleVrms.Add(vehicleVrm);
                _context.Add(vehicle);
                _context.Add(driverVehicle);
                //_context.NextMots.Add(vehicleMot);
            }
        }

        await _context.SaveChangesAsync();

        return 0;

        void SeedSuppliers()
        {
            foreach (var i in Enumerable.Range(1, 100))
            {
                var supplier = new CreateSupplier(_context)
                    .Create(new CreateSupplierRequest(
                        $"SUP{i:D4}",
                        $"Supplier #{i}",
                        new Address("", "", "", "", RandomPoint()),
                        true));

                //var supplier = new Supplier(
                //    $"SUP{i:D4}",
                //    $"Supplier #{i}",
                //    new Address("","","","", RandomPoint()),
                //    true
                //);

                //await _context.Suppliers.AddAsync(supplier);
            }
        }


    }
}

class MakeInfo
{
    public string Make { get; set; }
    public List<ModelInfo> Models { get; set; }

    public MakeInfo(string make, List<ModelInfo> models)
    {
        Make = make ?? throw new ArgumentNullException(nameof(make));
        Models = models ?? throw new ArgumentNullException(nameof(models));
    }
}
class ModelInfo
{
    public string Model { get; set; }

    public ModelInfo(string model)
    {
        Model = model ?? throw new ArgumentNullException(nameof(model));
    }
}