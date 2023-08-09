using Microsoft.Extensions.Logging;
using Vms.Application.Commands.VehicleUseCase;

namespace Vms.Application.Commands;

public class CreateDriver(VmsDbContext dbContext, ISearchManager searchManager, ILogger logger)
{
    readonly VmsDbContext DbContext = dbContext;
    VehicleRole? Vehicle;

    public async Task<Driver> CreateAsync(CreateDriverRequest request, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Creating driver {driverfirstname} {driverlastname}", request.FirstName, request.LastName);

        Vehicle = new(await DbContext.Vehicles.FindAsync(new object[] { request.VehicleId }, cancellationToken)
            ?? throw new VmsDomainException("Vehicle not found."), this);

        var driver = await Vehicle.CreateDriverAsync(request, cancellationToken);

        searchManager.Add(driver.CompanyCode, driver.Id.ToString(), EntityKind.Driver, driver.FullName,
            driver.FullName);

        return driver;
    }

    class VehicleRole(Vehicle self, CreateDriver context)
    {
        public async Task<Driver> CreateDriverAsync(CreateDriverRequest request, CancellationToken cancellationToken)
        {
            var driver = new Driver(request.CompanyCode, request.Salutation, request.FirstName, request.MiddleNames, request.LastName,
                request.EmailAddress, request.MobileNumber, request.HomeLocation);

            context.DbContext.Drivers.Add(driver);

            await new AddDriverToVehicle(context.DbContext)
                .AddAsync(self.Id, new AddDriverToVehicleCommand(driver.Id), cancellationToken);

            //AddDriver(driver);

            return driver;
        }

        //void AddDriver(Driver driver)
        //{
        //    var driverVehicleLink = new DriverVehicle(driver.CompanyCode, driver.Id, self.Id);
        //    context.DbContext.DriverVehicles.Add(driverVehicleLink);
        //}
    }
}

public record CreateDriverRequest(
    string CompanyCode,
    Guid VehicleId,
    string? Salutation,
    string? FirstName,
    string? MiddleNames,
    string LastName,
    string EmailAddress,
    string MobileNumber,
    Geometry HomeLocation);
