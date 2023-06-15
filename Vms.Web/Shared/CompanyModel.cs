namespace Vms.Web.Shared;

public record CompanyModel : ICopyable<CompanyModel>
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
public record CompanyListModel(string Code, string Name);