namespace Vms.Web.Shared;

public record NetworkListDto(string CompanyCode, string Code, string Name);
public enum NetworkListOptions
{
    All = 0,
}