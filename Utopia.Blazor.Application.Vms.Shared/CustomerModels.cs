namespace Utopia.Blazor.Application.Vms.Shared;

public record CustomerListDto(string CompanyCode, string Code, string Name);
public enum CustomerListOptions
{
    All = 0,
}

public record CustomerShortDto(string CompanyCode, string Code, string Name);