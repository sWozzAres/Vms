namespace Vms.Web.Shared;

public record SupplierLocatorDto(string Code, string Name, double Distance, string? RefusalCode, string? RefusalName)
{
    public double DistanceInMiles => Distance / 1609.344d;
}

public record SupplierShortDto(string Code, string Name);