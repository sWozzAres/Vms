namespace Vms.Web.Shared;

public record DriverListDto(Guid Id, string FullName);
public enum DriverListOptions
{
    All = 0,
}