namespace Vms.Web.Shared;

public record ActivityLogDto(Guid Id, string Text, DateTimeOffset EntryDate, string UserName);
public record AddNoteDto(string Text);
public record AddressFullDto(string Street, string Locality, string Town, string Postcode, GeometryFullDto Location);
public class AddressDto : ICopyable<AddressDto>
{
    [StringLength(50)]
    public string Street { get; set; } = string.Empty;
    [StringLength(50)]
    public string Locality { get; set; } = string.Empty;
    [StringLength(50)]
    public string Town { get; set; } = string.Empty;
    [StringLength(8)]
    public string Postcode { get; set; } = string.Empty;
    [ValidateComplexType]
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
public record GeometryFullDto(double Latitude, double Longitude);
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
