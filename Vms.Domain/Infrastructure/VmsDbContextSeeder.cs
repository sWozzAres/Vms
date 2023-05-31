using NetTopologySuite.Geometries;
using Vms.Domain.Entity;

namespace Vms.Domain.Infrastructure;

public interface IVmsDbContextSeeder
{
    Task<int> Seed();
}

public class VmsDbContextSeeder : IVmsDbContextSeeder
{
    private readonly VmsDbContext _context;

    public VmsDbContextSeeder(VmsDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    static string[] SurNames = new string[]
    {
        "Smith", "Jones", "Taylor", "Brown", "Williams", "Wilson", "Johnson", "Davies", "Robinson", "Wright", "Thomson", "Evans",
        "Walker", "White", "Roberts", "Green", "Hall", "Wood", "Jackson", "Clark"
    };

    static string[] MaleFirstNames = new string[]
    {
        "Oliver", "George", "Arthur", "Noah", "Muhammad", "Leo", "Oscar", "Harry", "Archie", "Henry"
    };

    static string[] FemaleFirstNames = new string[]
    {
        "Olivia", "Amelia", "Isla", "Ava", "Mia", "Ivy", "Lily", "Isabella", "Sophia", "Rosie"
    };

    static string[] MakeNames = new string[] { "MERCEDES", "FORD", "SUBARU" };

    const double minLatitude = 58.88670472493245;
    const double minLongitude = -9.372902643217499;
    const double maxLatitude = 50.26714546722624;
    const double maxLongitude = 2.668112510252122;
    const double deltaLatitude = maxLatitude - minLatitude;
    const double deltaLongitude = maxLongitude - minLongitude;
    static Point RandomPoint() => new(minLongitude + (deltaLongitude * rnd.NextDouble()), minLatitude + (deltaLatitude * rnd.NextDouble())) { SRID = 4326 };

    static readonly Random rnd = new();

    public async Task<int> Seed()
    {
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
            await SeedSuppliers();
        }

        // generate make / model data

        var makes = MakeNames.Select(name =>
            new MakeInfo(name,
                Enumerable.Range(1, 100).Select(x => new ModelInfo($"Model #{x}")).ToList()));


        if (!_context.VehicleMakes.Any())
        {
            await _context.VehicleMakes.AddRangeAsync(makes.Select(x => new VehicleMake(x.Make)));
        }

        IEnumerable<(string Make, string Model)> flattenedMakeModel = from make in makes
                                                                      from model in make.Models
                                                                      select (make.Make, model.Model);

        (string Make, string Model) RandomMakeModel(Random rnd) =>
            flattenedMakeModel.ElementAt(rnd.Next(flattenedMakeModel.Count()));


        if (!_context.VehicleModels.Any())
        {
            await _context.VehicleModels.AddRangeAsync(flattenedMakeModel.Select(x => new VehicleModel(x.Make, x.Model)));
        }

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

                var vehicle = new Vehicle()
                {
                    Make = Make,
                    Model = Model,
                    DateFirstRegistered = RandomDate(2001, DateTime.Now.Year - 1)
                };

                var driverVehicle = new DriverVehicle()
                {
                    Vehicle = vehicle,
                    EmailAddress = driver.EmailAddress
                };


                char[] letters = new char[] { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'K', 'L', 'M', 'N', 'O', 'P', 'R', 'S', 'V', 'W', 'X', 'Y' };

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

                var vehicleVrm = new VehicleVrm()
                {
                    Vehicle = vehicle,
                    Vrm = RandomVrm(vehicle.DateFirstRegistered)

                };

                var vehicleMot = new NextMot()
                {
                    Vehicle = vehicle,
                    Due = DateOnly.FromDateTime(DateTime.Now.AddDays(14 + rnd.Next(28)))
                };

                _context.VehicleVrms.Add(vehicleVrm);
                _context.Vehicles.Add(vehicle);
                _context.DriverVehicles.Add(driverVehicle);
                _context.NextMots.Add(vehicleMot);
            }
        }

        await _context.SaveChangesAsync();

        return 0;

        async Task SeedSuppliers()
        {
            foreach (var i in Enumerable.Range(1, 100))
            {

                var supplier = new Supplier()
                {
                    Code = $"SUP{i:D4}",
                    Name = $"Supplier #{i}",
                    Postcode = "",
                    IsIndependent = true,
                    Location = RandomPoint()
                };

                await _context.Suppliers.AddAsync(supplier);
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