namespace Utopia.Blazor.Application.Vms.Shared;

public record NetworkListDto(string CompanyCode, string Code, string Name);
public enum NetworkListOptions
{
    All = 0,
}