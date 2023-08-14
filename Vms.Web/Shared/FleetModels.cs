namespace Vms.Web.Shared;

public record FleetListDto(string CompanyCode, string Code, string Name);
public enum FleetListOptions
{
    All = 0,
}

public record FleetShortDto(string CompanyCode, string Code, string Name);