namespace Vms.Web.Shared;

public record AssignCustomerToVehicleDto(string CustomerCode);
public record AddDriverToVehicleDto(Guid Id);
public record VehicleListDto(Guid Id, string CompanyCode, string Vrm, string Make, string Model);

public partial class VehicleDto : ICopyable<VehicleDto>
{
    public Guid Id { get; set; }
    [Required, StringLength(10)]
    public string? CompanyCode { get; set; } = string.Empty;
    [Required, StringLength(12)]
    public string Vrm { get; set; } = string.Empty;
    [Required, StringLength(30)]
    public string? Make { get; set; } = string.Empty;
    [Required, StringLength(50)]
    public string? Model { get; set; } = string.Empty;
    [StringLength(18)]
    public string? ChassisNumber { get; set; }
    public DateOnly DateFirstRegistered { get; set; }
    public AddressDto Address { get; set; } = new();//"", "", "", "", new(0, 0));
    [StringLength(10)]
    public string? CustomerCode { get; set; }
    [StringLength(10)]
    public string? FleetCode { get; set; }
    public VehicleDto() { }
    public VehicleDto(string companyCode, Guid id, string vrm, string make, string model, string? chassisNumber,
        DateOnly dateFirstRegistered, AddressDto address,
        string? customerCode, string? fleetCode)
    {
        CompanyCode = companyCode ?? throw new ArgumentNullException(nameof(companyCode));
        Id = id;
        Vrm = vrm;
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
        Vrm = source.Vrm;
        Make = source.Make;
        Model = source.Model;
        ChassisNumber = source.ChassisNumber;
        DateFirstRegistered = source.DateFirstRegistered;
        Address.CopyFrom(source.Address);
        CustomerCode = source.CustomerCode;
        FleetCode = source.FleetCode;
    }
}

public partial class AddressDto : ICopyable<AddressDto>
{
    [StringLength(50)]
    public string Street { get; set; } = string.Empty;
    [StringLength(50)]
    public string Locality { get; set; } = string.Empty;
    [StringLength(50)]
    public string Town { get; set; } = string.Empty;
    [StringLength(8)]
    public string Postcode { get; set; } = string.Empty;
    public GeometryDto Location { get; set; } = new();// (0, 0);
    public AddressDto() { }
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


public partial class GeometryDto : ICopyable<GeometryDto>
{
    [Range(-90, 90)]
    public double Latitude { get; set; }
    [Range(-180, 180)]
    public double Longitude { get; set; }
    public GeometryDto() { }
    public GeometryDto(double latitude, double longitude)
    {
        Latitude = latitude;
        Longitude = longitude;
    }

    public void CopyFrom(GeometryDto source)
        => (Latitude, Longitude) = (source.Latitude, source.Longitude);
}