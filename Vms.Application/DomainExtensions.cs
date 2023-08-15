namespace Vms.Application;

public static partial class DomainExtensions
{
    public static SupplierDto ToDto(this Supplier supplier)
        => new()
        {
            Code = supplier.Code,
            Name = supplier.Name,
            IsIndependent = supplier.IsIndependent,
            Address = supplier.Address.ToDto()
        };

    public static ActivityLogDto ToDto(this ActivityLog activityLog)
        => new(activityLog.Id, activityLog.Text, activityLog.EntryDate, activityLog.UserName);

    public static VehicleFullDto ToFullDto(this Vehicle vehicle, bool isFollowing)
    => new(
            vehicle.CompanyCode,
            vehicle.Id,
            vehicle.Vrm,
            vehicle.Make,
            vehicle.Model,
            vehicle.ChassisNumber,
            vehicle.DateFirstRegistered,
            //vehicle.Mot.Due,
            vehicle.MotEvents.FirstOrDefault()?.Due,
            vehicle.Address.ToFullDto(),
            vehicle.C is null ? null : new CustomerShortDto(vehicle.CompanyCode, vehicle.C.Code, vehicle.C.Name),
            vehicle.Fleet is null ? null : new FleetShortDto(vehicle.CompanyCode, vehicle.Fleet.Code, vehicle.Fleet.Name),
            vehicle.DriverVehicles.Select(x => x.Driver.ToShortDto()).ToList(),
            isFollowing
            );


    public static AddressFullDto ToFullDto(this Address address)
        => new(address.Street, address.Locality, address.Town, address.Postcode, address.Location.ToFullDto());

    public static GeometryFullDto ToFullDto(this Geometry geometry)
        => new(geometry.Coordinate.X, geometry.Coordinate.Y);

    public static VehicleDto ToDto(this Vehicle vehicle)
        => new(
            vehicle.CompanyCode,
            vehicle.Id,
            vehicle.Vrm,
            vehicle.Make,
            vehicle.Model,
            vehicle.ChassisNumber,
            vehicle.DateFirstRegistered,
            vehicle.Address.ToDto(),
            vehicle.CustomerCode,
            vehicle.FleetCode
            );

    public static AddressDto ToDto(this Address address)
        => new(address.Street, address.Locality, address.Town, address.Postcode, address.Location.ToDto());

    public static GeometryDto ToDto(this Geometry geometry)
        => new(geometry.Coordinate.X, geometry.Coordinate.Y);
    public static DriverShortDto ToShortDto(this Driver driver)
        => new(driver.Id, driver.CompanyCode, driver.EmailAddress, driver.FullName, driver.MobileNumber);
}
