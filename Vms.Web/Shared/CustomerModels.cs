namespace Vms.Web.Shared;

public record CustomerListDto(string CompanyCode, string Code, string Name);
public enum CustomerListOptions
{
    All = 0,
}