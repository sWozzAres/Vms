namespace Vms.Web.Shared;

public enum EntityTagKindDto
{
    None = 0,
    Company = 1,
    Fleet = 2,
    Vehicle = 3,
    Customer = 4,
    Network = 5,
    Supplier = 6,
    ServiceBooking = 7,
    Driver = 8
}
public record EntityTagDto(string Key, EntityTagKindDto Kind, string Name)
{
    public string KindString => Kind switch
    {
        EntityTagKindDto.None => "None",
        EntityTagKindDto.Company => "Company",
        EntityTagKindDto.Fleet => "Fleet",
        EntityTagKindDto.Vehicle => "Vehicle",
        EntityTagKindDto.Customer => "Customer",
        EntityTagKindDto.Network => "Network",
        EntityTagKindDto.Supplier => "Supplier",
        EntityTagKindDto.Driver => "Driver",
        EntityTagKindDto.ServiceBooking => "Service Booking",
        _ => "Unknown"
    };
}