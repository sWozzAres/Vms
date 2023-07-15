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
public record struct CompanyListResponse(List<CompanyListModel> List, int TotalCount);
public record CompanyListModel(string Code, string Name);
public record ConfirmBookedRefusalReasonDto(string Code, string Name);
public record RefusalReasonDto(string Code, string Name);
public record NonArrivalReasonDto(string Code, string Name);
public record RescheduleReasonDto(string Code, string Name);
