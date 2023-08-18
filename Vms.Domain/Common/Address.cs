namespace Vms.Domain.Common;

/// <summary>
/// https://www.postoffice.co.uk/mail/how-to-address-mail
/// </summary>
public class Address : ValueObject
{
    public const int Street_MaxLength = 50;
    public const int Locality_MaxLength = 50;
    public const int Town_MaxLength = 50;
    public const int Postcode_MaxLength = 8;

    // House number and street name
    [StringLength(Street_MaxLength)]
    public string Street { get; private set; } = null!;

    // Locality name (if needed)
    [StringLength(Locality_MaxLength)]
    public string Locality { get; private set; } = null!;

    // Town (please print in capitals)
    [StringLength(Town_MaxLength)]
    public string Town { get; private set; } = null!;

    // Full postcode (please print in capitals)
    [StringLength(Postcode_MaxLength)]
    public string Postcode { get; private set; } = null!;

    public Geometry Location { get; private set; } = null!;
    private Address() { }
    public Address(string street, string locality, string town, string postcode, Geometry location)
    {
        Street = street;
        Locality = locality;
        Town = town;
        Postcode = postcode.ToUpper();
        Location = location.Copy();
    }
    public Address(Address source)
    {
        Street = source.Street;
        Locality = source.Locality;
        Town = source.Town;
        Postcode = source.Postcode;
        Location = source.Location.Copy();
    }
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Street;
        yield return Locality;
        yield return Town;
        yield return Postcode;
        yield return Location;
    }
}
