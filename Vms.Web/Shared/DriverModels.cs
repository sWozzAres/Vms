namespace Vms.Web.Shared;

public record DriverListDto(Guid Id, string FullName);
public enum DriverListOptions
{
    All = 0,
}

public record DriverShortDto(Guid Id, string CompanyCode, string EmailAddress, string FullName, string MobileNumber);