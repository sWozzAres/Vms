namespace Vms.Application.Extensions;

public static partial class DomainExtensions
{
    public static FleetShortDto ToShortDto(this Fleet fleet) 
        => new(fleet.CompanyCode, fleet.Code, fleet.Name);

    public static SupplierDto ToDto(this Supplier supplier)
        => new()
        {
            Code = supplier.Code,
            Name = supplier.Name,
            IsIndependent = supplier.IsIndependent,
            Address = supplier.Address.ToDto()
        };

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

    /// <summary>
    /// Adds a description of requested address changes to the supplied StringBuilder object.
    /// </summary>
    /// <returns>True if the address was modified.</returns>
    public static bool AddModificationSummary(this Address current, AddressDto requested, StringBuilder summary)
    {
        bool addressModified = false;
        if (current.Street != requested.Street)
        {
            summary.AppendLine($"* Street: {requested.Street}");
            addressModified = true;
        }
        if (current.Locality != requested.Locality)
        {
            summary.AppendLine($"* Locality: {requested.Locality}");
            addressModified = true;
        }
        if (current.Town != requested.Town)
        {
            summary.AppendLine($"* Town: {requested.Town}");
            addressModified = true;
        }
        if (current.Postcode != requested.Postcode)
        {
            summary.AppendLine($"* Postcode: {requested.Postcode}");
            addressModified = true;
        }
        if (current.Location.Coordinate.Y != requested.Location.Latitude)
        {
            summary.AppendLine($"* Latitude: {requested.Location.Latitude}");
            addressModified = true;
        }
        if (current.Location.Coordinate.X != requested.Location.Longitude)
        {
            summary.AppendLine($"* Longitude: {requested.Location.Longitude}");
            addressModified = true;
        }
        return addressModified;
    }
}
