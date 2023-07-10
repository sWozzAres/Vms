using Azure.Core;
using NetTopologySuite.Geometries;
using Vms.Domain.Entity;
using Vms.Domain.Exceptions;
using Vms.Domain.Infrastructure;
using Vms.Web.Shared;

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

        var driver = await Vehicle.CreateDriverAsync(request, cancellationToken);

        //await DbContext.SaveChangesAsync(cancellationToken);

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
