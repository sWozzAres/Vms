namespace Vms.Web.Shared;

public class CompanyModel : ICopyable<CompanyModel>
{
    [Required]
    public string Code { get; set; } = string.Empty;
    [Required]
    public string Name { get; set; } = string.Empty;

    public void CopyFrom(CompanyModel source)
    {
        Code = source.Code;
        Name = source.Name;
    }
}
public record struct CompanyListResponse(List<CompanyListDto> List, int TotalCount);
public record CompanyShortDto(string Code, string Name);
public record CompanyListDto(string Code, string Name);
public enum CompanyListOptions
{
    All = 0,
}
public record ConfirmBookedRefusalReasonDto(string Code, string Name);
public record RefusalReasonDto(string Code, string Name);
public record NonArrivalReasonDto(string Code, string Name);
public record NotCompleteReasonDto(string Code, string Name);
public record RescheduleReasonDto(string Code, string Name);
