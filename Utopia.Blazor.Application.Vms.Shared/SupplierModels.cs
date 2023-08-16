namespace Utopia.Blazor.Application.Vms.Shared;

public record SupplierLocatorDto(string Code, string Name, double Distance, string? RefusalCode, string? RefusalName)
{
    public double DistanceInMiles => Distance / 1609.344d;
}

public record SupplierShortDto(string Code, string Name);

public record SupplierListDto(string Code, string Name);
public enum SupplierListOptions
{
    All = 0,
    Recent = 1,
    Following = 2,
}

public class SupplierFullDto(Guid id, string code, string name, bool isIndependant, AddressFullDto address, bool isFollowing)
{
    public Guid Id { get; set; } = id;
    public string Code { get; set; } = code;
    public string Name { get; set; } = name;
    public bool IsIndependant { get; set; } = isIndependant;
    public AddressFullDto Address { get; set; } = address;
    public bool IsFollowing { get; set; } = isFollowing;

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

public class CreateSupplierDto : ICopyable<CreateSupplierDto>
{
    [Required]
    [StringLength(8)]
    public string Code { get; set; } = null!;
    [Required]
    [StringLength(50)]
    public string Name { get; set; } = null!;

    public void CopyFrom(CreateSupplierDto source)
    {
        Code = source.Code;
        Name = source.Name;
    }
}