using Azure.Core;
using NetTopologySuite.Geometries;
using Vms.Domain.Entity;
using Vms.Domain.Exceptions;
using Vms.Domain.Infrastructure;

namespace Vms.Application.UseCase;

public class CreateDriver
{
    readonly VmsDbContext DbContext;
    VehicleRole? Vehicle;
    public CreateDriver(VmsDbContext dbContext)
       => DbContext = dbContext;

    public async Task<Driver> CreateAsync(CreateDriverRequest request, CancellationToken cancellationToken = default)
    {
        Vehicle = new(await DbContext.Vehicles.FindAsync(request.VehicleId, cancellationToken)
            ?? throw new VmsDomainException("Vehicle not found."), this);

        return Vehicle.CreateDriver(request);
    }

    public class VehicleRole(Vehicle self, CreateDriver context)
    {
        public Driver CreateDriver(CreateDriverRequest request)
        {
            var driver = new Driver(request.CompanyCode, request.Salutation, request.FirstName, request.MiddleNames, request.LastName,
                request.EmailAddress, request.MobileNumber, request.HomeLocation);

            context.DbContext.Drivers.Add(driver);

            AddDriver(driver);

            return driver;
        }

        void AddDriver(Driver driver)
        {
            var driverVehicleLink = new DriverVehicle(driver.CompanyCode, self.Id, driver.EmailAddress);
            context.DbContext.DriverVehicles.Add(driverVehicleLink);
        }
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
