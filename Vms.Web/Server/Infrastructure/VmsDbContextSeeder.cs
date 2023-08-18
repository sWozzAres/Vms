using Microsoft.Extensions.Options;
using Utopia.Api.Application.Services;
using Utopia.Api.Domain.System;
using Vms.Application.Commands;
using Vms.Domain.ServiceBookingProcess;
using Vms.Web.Server;

namespace Vms.Domain.Infrastructure.Seed;

public interface IVmsDbContextSeeder
{
    Task SeedAsync(IWebHostEnvironment env, IOptions<AppSettings> settings);
}

public class VmsDbContextSeeder(
    VmsDbContext context,
    ISearchManager searchManager,
    ILogger<VmsDbContextSeeder> logger,
    ILoggerFactory loggerFactory,
    IActivityLogger<VmsDbContext> activityLog,
    ITaskLogger<VmsDbContext> taskLogger,
    ITimeService timeService,
    IUserProvider userProvider) : IVmsDbContextSeeder
{
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
    static Point RandomPoint() => new(minLongitude + (deltaLongitude * rnd.NextDouble()), minLatitude + (deltaLatitude * rnd.NextDouble())) { SRID = 4326 };

    static readonly Random rnd = new();


    public async Task SeedAsync(IWebHostEnvironment env, IOptions<AppSettings> settings)
    {


        var strategy = context.Database.CreateExecutionStrategy();

        await strategy.ExecuteAsync(async () =>
        {
            using var transaction = context.Database.BeginTransaction();
            try
            {
                if (!context.Users.Any())
                {
                    context.Users.Add(new User(userProvider.UserId, userProvider.UserName, userProvider.TenantId, userProvider.EmailAddress));
                }

                if (!context.Companies.Any())
                {
                    logger.LogInformation("Seeding database.");
                    await SeedData();
                }
                else
                {
                    await context.Companies.ToListAsync();
                }

                if (!context.Suppliers.Any())
                {
                    logger.LogInformation("Seeding suppliers.");
                    await SeedSuppliers();
                }

                SeedLists();

                await context.SaveChangesAsync();
                await transaction.CommitAsync();

                return true;
            }
            catch (Exception ex)
            {
                logger.LogError("Failed to seed database {ex}.", ex);
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
            var company = new CreateCompany(context, searchManager, logger)
                .Create(new CreateCompanyRequest(code, name));

            foreach (var fi in Enumerable.Range(1, 50))
            {
                string fleetCode = $"FL{ci:D3}{fi:D2}";
                string fleetName = $"Fleet #{fi:D2} Company #{ci:D3}";

                var fleet1 = await new CreateFleet(context, searchManager, logger)
                    .CreateAsync(new CreateFleetRequest(company.Code, fleetCode, fleetName));
            }

            foreach (var ni in Enumerable.Range(1, 20))
            {
                string networkCode = $"NET{ci:D3}{ni:D2}";
                string networkName = $"Network #{ni:D2} Company #{ci:D3}";

                var fleet = await new CreateNetwork(context, searchManager, logger)
                    .CreateAsync(new CreateNetworkRequest(company.Code, networkCode, networkName));
            }

            foreach (var csi in Enumerable.Range(1, 100))
            {
                string customerCode = $"CUS{ci:D3}{csi:D3}";
                string customerName = $"Customer #{csi:D3} Company #{ci:D3}";

                var customer = await new CreateCustomer(context, searchManager, logger)
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
            new CreateMake(context, logger)
                .Create(new CreateMakeRequest(makeInfo.Make));

            foreach (var modelInfo in makeInfo.Models)
            {
                await new CreateModel(context, logger)
                    .CreateAsync(new CreateModelRequest(makeInfo.Make, modelInfo.Model));
            }
        }


        IEnumerable<DriverInfo> GetDrivers()
        {
            IEnumerable<(string firstName, string surName)> fullNames(IEnumerable<string> firstNames)
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


        //if (!context.VehicleModels.Any())
        //{
        //    await context.VehicleModels.AddRangeAsync(flattenedMakeModel.Select(x => new VehicleModel(x.Make, x.Model)));
        //}

        var allCompanies = context.ChangeTracker.Entries<Company>().Select(x => x.Entity).ToList();

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

            var vehicle = await new CreateVehicle(context, searchManager)
                .CreateAsync(new CreateVehicleRequest(
                    company.Code,
                    RandomVrm(dateFirstRegistered),
                    Make, Model,
                    dateFirstRegistered,
                    DateOnly.FromDateTime(DateTime.Now.AddDays(14 + rnd.Next(28))),
                    null,
                    new Address("", "", "", "", new Point(-2.3554702709792426, 51.69082531225236) { SRID = 4326 }),
                    null, null));

            var driver = await new CreateDriver(context, searchManager, logger)
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
        var allCompanies = context.ChangeTracker.Entries<Company>().Select(x => x.Entity).ToList();

        if (!context.NotCompleteReasons.Any())
        {
            foreach (var company in allCompanies)
            {
                for (int i = 0; i < 5; i++)
                {
                    var refusalReason = new NotCompleteReason(company.Code, $"CODE{i:D2}", $"Not Complete Reason #{i}");
                    context.NotCompleteReasons.Add(refusalReason);
                }
            }
        }

        if (!context.ConfirmBookedRefusalReasons.Any())
        {
            foreach (var company in allCompanies)
            {
                for (int i = 0; i < 5; i++)
                {
                    var refusalReason = new ConfirmBookedRefusalReason(company.Code, $"CODE{i:D2}", $"ConfirmBooked Refusal reason #{i}");
                    context.ConfirmBookedRefusalReasons.Add(refusalReason);
                }
            }
        }

        if (!context.NonArrivalReasons.Any())
        {
            foreach (var company in allCompanies)
            {
                for (int i = 0; i < 5; i++)
                {
                    var refusalReason = new NonArrivalReason(company.Code, $"CODE{i:D2}", $"NonArrival Reason reason #{i}");
                    context.NonArrivalReasons.Add(refusalReason);
                }
            }
        }

        if (!context.RefusalReasons.Any())
        {
            foreach (var company in allCompanies)
            {
                for (int i = 0; i < 5; i++)
                {
                    var refusalReason = new RefusalReason(company.Code, $"CODE{i:D2}", $"Refusal reason #{i}");
                    context.RefusalReasons.Add(refusalReason);
                }
            }
        }

        if (!context.RescheduleReasons.Any())
        {
            foreach (var company in allCompanies)
            {
                for (int i = 0; i < 5; i++)
                {
                    var rescheduleReason = new RescheduleReason(company.Code, $"CODE{i:D2}", $"Reschedule reason #{i}");
                    context.RescheduleReasons.Add(rescheduleReason);
                }
            }
        }
    }

    async Task SeedSuppliers()
    {
        var logger = loggerFactory.CreateLogger<CreateSupplier>();

        await new CreateSupplier(context, searchManager, logger, activityLog, taskLogger, timeService)
                .CreateAsync(new CreateSupplierRequest(
                    "TSTSUP01",
                    "Thrupp Tyre Co Ltd",
                    new AddressDto("Unit 12 Griffin Mill", "London Rd", "STROUD", "GL52AZ", new GeometryDto(51.73021720095717, -2.204244578769126)), true));

        await new CreateSupplier(context, searchManager, logger, activityLog, taskLogger, timeService)
                .CreateAsync(new CreateSupplierRequest(
                    "TSTSUP02",
                    "Warwick Car Co",
                    new AddressDto("Fromeside Ind Est", "Doctor Newtons Way", "STROUD", "GL53JX", new GeometryDto(51.74259678708762, -2.217698852580117)), true));

        await new CreateSupplier(context, searchManager, logger, activityLog, taskLogger, timeService)
                .CreateAsync(new CreateSupplierRequest(
                    "TSTSUP03",
                    "Blake Services Stroud Ltd",
                    new AddressDto("Hopes Mill Business Centre", "", "STROUD", "GL52SE", new GeometryDto(51.72249258962455, -2.197510975726595)), true));

        await new CreateSupplier(context, searchManager, logger, activityLog, taskLogger, timeService)
                .CreateAsync(new CreateSupplierRequest(
                    "TSTSUP04",
                    "Lansdown Road Motors Ltd",
                    new AddressDto("Lansdown Rd", "", "STROUD", "GL51BW", new GeometryDto(51.74831757007313, -2.210593551187789)), true));

        await new CreateSupplier(context, searchManager, logger, activityLog, taskLogger, timeService)
                .CreateAsync(new CreateSupplierRequest(
                    "TSTSUP05",
                    "Kwik Fit - Dursley",
                    new AddressDto("Draycott Business Park", "Cam", "Dursley", "GL115DQ", new GeometryDto(51.74831757007313, -2.210593551187789)), true));

        foreach (var i in Enumerable.Range(1, 100))
        {
            var p = RandomPoint();

            await new CreateSupplier(context, searchManager, logger, activityLog, taskLogger, timeService)
                .CreateAsync(new CreateSupplierRequest(
                    $"SUP{i:D4}",
                    $"Supplier #{i}",
                    new AddressDto("", "", "", "", new GeometryDto(p.Coordinate.Y, p.Coordinate.X)),
                    true));
        }

        // assign suppliers to networks
        //var allSuppliers = context.ChangeTracker.Entries<Supplier>().Select(x => x.Entity).ToList();
        //var allNetworks = context.ChangeTracker.Entries<Network>().Select(x => x.Entity).ToList();

        //foreach(var network in allNetworks)
        //{
        //    var idx = rnd.Next(0, 5);
        //    while (idx < allSuppliers.Count)
        //    {
        //        var supplier = allSuppliers.ElementAt(idx);

        //        await new AssignSupplierToNetwork(context)
        //            .AssignAsync(supplier.Code, network.CompanyCode, network.Code);

        //        idx += rnd.Next(1, 5);
        //    }
        //}


        await Task.CompletedTask;
    }
}

class MakeInfo(string make, List<ModelInfo> models)
{
    public string Make { get; set; } = make ?? throw new ArgumentNullException(nameof(make));
    public List<ModelInfo> Models { get; set; } = models ?? throw new ArgumentNullException(nameof(models));
}
class ModelInfo(string model)
{
    public string Model { get; set; } = model ?? throw new ArgumentNullException(nameof(model));
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