namespace Vms.Web.Shared;

public record SupplierLocatorDto(string Code, string Name, double Distance)
{
    public double DistanceInMiles => Distance / 1609.344d;
}

public record SupplierShortDto(string Code, string Name);