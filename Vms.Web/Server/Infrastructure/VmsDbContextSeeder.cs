using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NetTopologySuite.Geometries;
using Polly;
using Vms.Application.UseCase;
using Vms.Domain.Entity;
using Vms.Domain.Entity.ServiceBookingEntity;
using Vms.Web.Server;

namespace Vms.Domain.Infrastructure.Seed;

public interface IVmsDbContextSeeder
{
    Task SeedAsync(IWebHostEnvironment env, IOptions<AppSettings> settings);
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
        "Walker", "White", "Roberts", "Green", "Hall", "Wood", "Jackson", "Clark",
        "Marshall", "Stevenson", "Sutherland", "Craig", "Wright", "McKenzie", "Kennedy",
        "Jones", "Burns", "White", "Muir", "Murphy", "Johnstone", "Hughes"
    };

    static readonly string[] MaleFirstNames = new string[]
    {
        "Oliver", "George", "Arthur", "Noah", "Muhammad", "Leo", "Oscar", "Harry", "Archie", "Henry",
        "James", "Robert", "John", "Michael", "David", "William", "Richard","Joseph","Thomas",
        "Charles", "Daniel", "Matthew", "Anthony", "Mark", "Donald", "Steven", "Andrew", "Paul",
        "Joshua", "Kenneth", "Kevin", "Brian", "Timothy", "Ronald", "Jason", "Edward"
    };

    static readonly string[] FemaleFirstNames = new string[]
    {
        "Olivia", "Amelia", "Isla", "Ava", "Mia", "Ivy", "Lily", "Isabella", "Sophia", "Rosie",
        "Mary", "Patricia", "Jennifer", "Linda", "Elizabeth", "Barbara", "Susan", "Jessica",
        "Sarah", "Karen", "Lisa", "Nancy", "Betty", "Sandra", "Margaret", "Ashley",
        "Kimberly", "Emily", "Donna", "Michelle", "Carol", "Amanda", "Melissa", "Deborah"
    };

    static readonly string[] MakeNames = new string[] { "MERCEDES", "FORD", "SUBARU" };

    static readonly char[] letters = new char[] { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'K', 'L', 'M', 'N', 'O', 'P', 'R', 'S', 'V', 'W', 'X', 'Y' };

    const double minLatitude = 58.88670472493245;
    const double minLongitude = -9.372902643217499;
    const double maxLatitude = 50.26714546722624;
    const double maxLongitude = 2.668112510252122;
    const double deltaLatitude = maxLatitude - minLatitude;
    const double deltaLongitude = maxLongitude - minLongitude;
    static Geometry RandomPoint() => new Point(minLongitude + (deltaLongitude * rnd.NextDouble()), minLatitude + (deltaLatitude * rnd.NextDouble())) { SRID = 4326 };

    static readonly Random rnd = new();


    public async Task SeedAsync(IWebHostEnvironment env, IOptions<AppSettings> settings)
    {
        _logger.LogInformation("Seeding database.");

        var strategy = _context.Database.CreateExecutionStrategy();

        await strategy.ExecuteAsync(async () =>
        {
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                if (!_context.Companies.Any())
                {
                    _logger.LogInformation("Seeding main data.");
                    await SeedData();
                }
                else
                {
                    // load all companies
                    await _context.Companies.ToListAsync();
                }

                if (!_context.Suppliers.Any())
                {
                    _logger.LogInformation("Seeding suppliers.");
                    await SeedSuppliers();
                }

                SeedLists();

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to seed database {ex}.", ex);
                await transaction.RollbackAsync();
                return false;
            }
        });
    }

    public async Task SeedData()
    {

        List<CompanyInfo> companies = new();
        foreach (var ci in Enumerable.Range(1, 5))
        {
            string code = $"TEST{ci:D3}";
            string name = $"Company #{ci}";
            var company = new CreateCompany(_context)
                .Create(new CreateCompanyRequest(code, name));

            foreach (var fi in Enumerable.Range(1, 50))
            {
                string fleetCode = $"FL{ci:D3}{fi:D2}";
                string fleetName = $"Fleet #{fi:D2} Company #{ci:D3}";

                var fleet1 = await new CreateFleet(_context)
                    .CreateAsync(new CreateFleetRequest(company.Code, fleetCode, fleetName));
            }

            foreach (var ni in Enumerable.Range(1, 20))
            {
                string networkCode = $"NET{ci:D3}{ni:D2}";
                string networkName = $"Network #{ni:D2} Company #{ci:D3}";

                var fleet = await new CreateNetwork(_context)
                    .CreateAsync(new CreateNetworkRequest(company.Code, networkCode, networkName));
            }

            foreach (var csi in Enumerable.Range(1, 100))
            {
                string customerCode = $"CUS{ci:D3}{csi:D3}";
                string customerName = $"Customer #{csi:D3} Company #{ci:D3}";

                var customer = await new CreateCustomer(_context)
                    .CreateAsync(new CreateCustomerRequest(company.Code, customerCode, customerName));

                
            }
        }

        //await SeedSuppliers();

        // generate make / model data
        var makes = MakeNames.Select(name =>
            new MakeInfo(name,
                Enumerable.Range(1, 100).Select(x => new ModelInfo($"Model #{x}")).ToList()));

        foreach (var makeInfo in makes)
        {
            new CreateMake(_context)
                .Create(new CreateMakeRequest(makeInfo.Make));

            foreach (var modelInfo in makeInfo.Models)
            {
                await new CreateModel(_context)
                    .CreateAsync(new CreateModelRequest(makeInfo.Make, modelInfo.Model));
            }
        }


        IEnumerable<DriverInfo> GetDrivers()
        {
            var fullNames = (IEnumerable<string> firstNames)
                => from surName in SurNames.Distinct()
                   from firstName in firstNames
                   select (firstName, surName);

            var drivers =
                fullNames(MaleFirstNames.Distinct()).Select(n => new DriverInfo(
                    "Mr",
                    n.firstName,
                    "",
                    n.surName,
                    GenerateEmailAddress(n.firstName, n.surName),
                    RandomPhoneNumber(),
                    RandomPoint()
                ))
                .Concat(fullNames(FemaleFirstNames.Distinct()).Select(n => new DriverInfo(
                    rnd.Next(2) == 0 ? "Ms" : "Mrs",
                    n.firstName,
                    "",
                    n.surName,
                    GenerateEmailAddress(n.firstName, n.surName),
                    RandomPhoneNumber(),
                    RandomPoint()
                )));

            return drivers;

            string RandomPhoneNumber() => "0" + (7700000000 + rnd.Next(99999999)).ToString();
            string GenerateEmailAddress(string firstName, string lastName) => $"{firstName.ToLower()}.{lastName.ToLower()}{DateTime.Now.Year - 20 - rnd.Next(30)}@nowhere.com";
        }

        IEnumerable<(string Make, string Model)> flattenedMakeModel = from make in makes
                                                                      from model in make.Models
                                                                      select (make.Make, model.Model);

        (string Make, string Model) RandomMakeModel(Random rnd) =>
            flattenedMakeModel.ElementAt(rnd.Next(flattenedMakeModel.Count()));


        //if (!_context.VehicleModels.Any())
        //{
        //    await _context.VehicleModels.AddRangeAsync(flattenedMakeModel.Select(x => new VehicleModel(x.Make, x.Model)));
        //}

        var allCompanies = _context.ChangeTracker.Entries<Company>().Select(x => x.Entity).ToList();

        foreach (var driverInfo in GetDrivers())
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

            var company = allCompanies.ElementAt(rnd.Next(0, allCompanies.Count - 1));

            var vehicle = await new CreateVehicle(_context)
                .CreateAsync(new CreateVehicleRequest(
                    company.Code,
                    RandomVrm(dateFirstRegistered),
                    Make, Model,
                    dateFirstRegistered,
                    DateOnly.FromDateTime(DateTime.Now.AddDays(14 + rnd.Next(28))),
                    new Address("", "", "", "", new Point(51.73021720095717, -2.204244578769126) { SRID = 4326 }),
                    null, null));

            var driver = await new CreateDriver(_context)
                .CreateAsync(new CreateDriverRequest(company.Code,
                    vehicle.Id,
                    driverInfo.Salutation, driverInfo.FirstName, driverInfo.MiddleNames, driverInfo.LastName,
                    driverInfo.EmailAddress, driverInfo.MobileNumber,
                    driverInfo.HomeLocation));


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
        }
    }

    void SeedLists()
    {
        var allCompanies = _context.ChangeTracker.Entries<Company>().Select(x => x.Entity).ToList();

        if (!_context.NotCompleteReasons.Any())
        {
            foreach (var company in allCompanies)
            {
                for (int i = 0; i < 5; i++)
                {
                    var refusalReason = new NotCompleteReason(company.Code, $"CODE{i:D2}", $"Not Complete Reason #{i}");
                    _context.NotCompleteReasons.Add(refusalReason);
                }
            }
        }

        if (!_context.ConfirmBookedRefusalReasons.Any())
        {
            foreach (var company in allCompanies)
            {
                for (int i = 0; i < 5; i++)
                {
                    var refusalReason = new ConfirmBookedRefusalReason(company.Code, $"CODE{i:D2}", $"ConfirmBooked Refusal reason #{i}");
                    _context.ConfirmBookedRefusalReasons.Add(refusalReason);
                }
            }
        }

        if (!_context.NonArrivalReasons.Any())
        {
            foreach (var company in allCompanies)
            {
                for (int i = 0; i < 5; i++)
                {
                    var refusalReason = new NonArrivalReason(company.Code, $"CODE{i:D2}", $"NonArrival Reason reason #{i}");
                    _context.NonArrivalReasons.Add(refusalReason);
                }
            }
        }

        if (!_context.RefusalReasons.Any())
        {
            foreach (var company in allCompanies)
            {
                for (int i = 0; i < 5; i++)
                {
                    var refusalReason = new RefusalReason(company.Code, $"CODE{i:D2}", $"Refusal reason #{i}");
                    _context.RefusalReasons.Add(refusalReason);
                }
            }
        }

        if (!_context.RescheduleReasons.Any())
        {
            foreach (var company in allCompanies)
            {
                for (int i = 0; i < 5; i++)
                {
                    var rescheduleReason = new RescheduleReason(company.Code, $"CODE{i:D2}", $"Reschedule reason #{i}");
                    _context.RescheduleReasons.Add(rescheduleReason);
                }
            }
        }
    }

    async Task SeedSuppliers()
    {
        var s1 = new CreateSupplier(_context)
                .Create(new CreateSupplierRequest(
                    "TSTSUP01",
                    "Thrupp Tyre Co Ltd",
                    new Address("Unit 12 Griffin Mill", "London Rd", "STROUD", "GL52AZ", new Point(-2.204244578769126, 51.73021720095717) { SRID = 4326 }), true));

        var s2 = new CreateSupplier(_context)
                .Create(new CreateSupplierRequest(
                    "TSTSUP02",
                    "Warwick Car Co",
                    new Address("Fromeside Ind Est", "Doctor Newtons Way", "STROUD", "GL53JX", new Point(-2.217698852580117, 51.74259678708762) { SRID = 4326 }), true));

        var s3 = new CreateSupplier(_context)
                .Create(new CreateSupplierRequest(
                    "TSTSUP03",
                    "Blake Services Stroud Ltd",
                    new Address("Hopes Mill Business Centre", "", "STROUD", "GL52SE", new Point(-2.197510975726595, 51.72249258962455) { SRID = 4326 }), true));

        var s4 = new CreateSupplier(_context)
                .Create(new CreateSupplierRequest(
                    "TSTSUP04",
                    "Lansdown Road Motors Ltd",
                    new Address("Lansdown Rd", "", "STROUD", "GL51BW", new Point(-2.210593551187789, 51.74831757007313) { SRID = 4326 }), true));

        var s5 = new CreateSupplier(_context)
                .Create(new CreateSupplierRequest(
                    "TSTSUP05",
                    "Kwik Fit - Dursley",
                    new Address("Draycott Business Park", "Cam", "Dursley", "GL115DQ", new Point(-2.210593551187789, 51.74831757007313) { SRID = 4326 }), true));

        foreach (var i in Enumerable.Range(1, 100))
        {
            var supplier = new CreateSupplier(_context)
                .Create(new CreateSupplierRequest(
                    $"SUP{i:D4}",
                    $"Supplier #{i}",
                    new Address("", "", "", "", RandomPoint()),
                    true));
        }

        await Task.CompletedTask;
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

public record DriverInfo(string? Salutation, string? FirstName, string? MiddleNames,
    string LastName, string EmailAddress, string MobileNumber, Geometry HomeLocation);

public class CompanyInfo(string code, string name)
{
    public string Code { get; set; } = code;
    public string Name { get; set; } = name;
    public List<FleetInfo> Fleets { get; set; } = new();
    public List<CompanyInfo> Companies { get; set; } = new();
}

public class FleetInfo(string code, string name)
{
    public string Code { get; set; } = code;
    public string Name { get; set; } = name;
}

public class CustomerInfo(string code, string name)
{
    public string Code { get; set; } = code;
    public string Name { get; set; } = name;
}