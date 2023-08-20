namespace Vms.Application.Extensions;

public static partial class DomainExtensions
{
    public static FleetShortDto ToShortDto(this Fleet fleet) 
        => new(fleet.CompanyCode, fleet.Code, fleet.Name);

    public static CompanyModel ToDto(this Company company)
        => new()
        {
            Code = company.Code,
            Name = company.Name,
        };

    public static SupplierDto ToDto(this Supplier supplier)
        => new()
        {
            Code = supplier.Code,
            Name = supplier.Name,
            IsIndependent = supplier.IsIndependent,
            Address = supplier.Address.ToDto()
        };

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
