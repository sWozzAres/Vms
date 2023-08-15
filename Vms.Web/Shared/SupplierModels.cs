namespace Vms.Web.Shared;

public record SupplierLocatorDto(string Code, string Name, double Distance, string? RefusalCode, string? RefusalName)
{
    public double DistanceInMiles => Distance / 1609.344d;
}

public record SupplierShortDto(string Code, string Name);

public record SupplierListDto(string Code, string Name);
public enum SupplierListOptions
{
    All = 0,
}

public record SupplierFullDto(string Code, string Name, bool IsIndependant, AddressFullDto Address)
{
    public SupplierDto ToDto()
    => new()
    {
        Code = Code,
        Name = Name,
        IsIndependent = IsIndependant,
        Address = new AddressDto()
        {
            Street = Address.Street,
            Locality = Address.Locality,
            Town = Address.Town,
            Postcode = Address.Postcode,
            Location = new GeometryDto()
            {
                Latitude = Address.Location.Latitude,
                Longitude = Address.Location.Longitude,
            }
        },
    };
}

public class SupplierDto : ICopyable<SupplierDto>
{
    [Required]
    [StringLength(8)]
    public string Code { get; set; } = null!;
    [Required]
    [StringLength(50)]
    public string Name { get; set; } = null!;
    public bool IsIndependent { get; set; }
    public AddressDto Address { get; set; } = new();

    public void CopyFrom(SupplierDto source)
    {
        Code = source.Code;
        Name = source.Name;
        IsIndependent = source.IsIndependent;
        Address.CopyFrom(source.Address);
    }
}