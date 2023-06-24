using System.IO;
using Vms.Web.Shared;

namespace Vms.Web.Shared;

public record VehicleListModel(Guid Id, string CompanyCode, string Vrm, string Make, string Model);

public class VehicleDto : ICopyable<VehicleDto>
{
    [Required, StringLength(10)]
    public string CompanyCode { get; set; } = string.Empty;
    public Guid Id { get; set; }
    [Required, StringLength(30)]
    public string Make { get; set; } = string.Empty;
    [Required, StringLength(50)]
    public string Model { get; set; } = string.Empty;
    [StringLength(18)]
    public string? ChassisNumber { get; set; }
    public DateOnly DateFirstRegistered { get; set; }
    public AddressDto Address { get; set; } = new("", "", "", "", new(0, 0));
    [StringLength(10)]
    public string? CustomerCode { get; set; }
    [StringLength(10)]
    public string? FleetCode { get; set; }
    public VehicleDto() { }
    public VehicleDto(string companyCode, Guid id, string make, string model, string? chassisNumber,
        DateOnly dateFirstRegistered, AddressDto address,
        string? customerCode, string? fleetCode)
    {
        CompanyCode = companyCode ?? throw new ArgumentNullException(nameof(companyCode));
        Id = id;
        Make = make ?? throw new ArgumentNullException(nameof(make));
        Model = model ?? throw new ArgumentNullException(nameof(model));
        ChassisNumber = chassisNumber;
        DateFirstRegistered = dateFirstRegistered;
        Address = address ?? throw new ArgumentNullException(nameof(address));
        CustomerCode = customerCode;
        FleetCode = fleetCode;
    }

    public void CopyFrom(VehicleDto source)
    {
        CompanyCode = source.CompanyCode;
        Id = source.Id;
        Make = source.Make;
        Model = source.Model;
        ChassisNumber = source.ChassisNumber;
        DateFirstRegistered = source.DateFirstRegistered;
        Address.CopyFrom(source.Address);
        CustomerCode = source.CustomerCode;
        FleetCode = source.FleetCode;
    }
}

public class AddressDto : ICopyable<AddressDto>
{
    [Required, StringLength(50)]
    public string Street { get; set; } = string.Empty;
    [Required, StringLength(50)]
    public string Locality { get; set; } = string.Empty;
    [Required, StringLength(50)]
    public string Town { get; set; } = string.Empty;
    [Required, StringLength(8)]
    public string Postcode { get; set; } = string.Empty;
    public GeometryDto Location { get; set; } = new(0, 0);

    public AddressDto(string street, string locality, string town, string postcode, GeometryDto location)
    {
        Street = street ?? throw new ArgumentNullException(nameof(street));
        Locality = locality ?? throw new ArgumentNullException(nameof(locality));
        Town = town ?? throw new ArgumentNullException(nameof(town));
        Postcode = postcode ?? throw new ArgumentNullException(nameof(postcode));
        Location = location ?? throw new ArgumentNullException(nameof(location));
    }
    public void CopyFrom(AddressDto source)
    {
        Street = source.Street;
        Locality = source.Locality;
        Town = source.Town;
        Postcode = source.Postcode;
        Location.CopyFrom(source.Location);
    }
}


public class GeometryDto : ICopyable<GeometryDto>
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }

    public GeometryDto(double latitude, double longitude)
    {
        Latitude = latitude;
        Longitude = longitude;
    }

    public void CopyFrom(GeometryDto source)
        => (Latitude, Longitude) = (source.Latitude, source.Longitude);
}