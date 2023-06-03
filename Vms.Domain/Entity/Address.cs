using NetTopologySuite.Geometries;
using Vms.Domain.Common;

namespace Vms.Domain.Entity;

/// <summary>
/// https://www.postoffice.co.uk/mail/how-to-address-mail
/// </summary>
public class Address : ValueObject
{
    // House number and street name
    public string Street { get; private set; }
    // Locality name (if needed)
    public string Locality { get; private set; }
    // Town (please print in capitals)
    public string Town { get; private set; }
    // Full postcode (please print in capitals)
    public string Postcode { get; private set; }
    public Point Location { get; private set; }

    public Address(string street, string locality, string town, string postcode, Point location)
    {
        Street = street ?? throw new ArgumentNullException(nameof(street));
        Locality = locality ?? throw new ArgumentNullException(nameof(locality));
        Town = town ?? throw new ArgumentNullException(nameof(town));
        Postcode = postcode ?? throw new ArgumentNullException(nameof(postcode));
        Location = location;
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
