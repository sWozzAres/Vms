namespace Vms.Web.Shared;

public record DriverFullDto(string EmailAddress, string FullName, string MobileNumber);
public record VehicleFullDto(string CompanyCode, Guid Id, string Vrm, string Make, string Model, string? ChassisNumber,
    DateOnly DateFirstRegistered, AddressFullDto Address, CustomerSummaryResource? Customer,
    FleetSummaryResource? Fleet, List<DriverFullDto> Drivers)
{ 
    public VehicleDto ToDto()
        => new()
        {
            CompanyCode = CompanyCode,
            Id = Id,
            Vrm = Vrm,
            Make = Make,
            Model = Model,
            ChassisNumber = ChassisNumber,
            DateFirstRegistered = DateFirstRegistered,
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
            CustomerCode = Customer?.Code,
            FleetCode = Fleet?.Code,
        };
}

public record CustomerSummaryResource(string Code, string Name);
public record FleetSummaryResource(string Code, string Name);
public record AddressFullDto(string Street, string Locality, string Town, string Postcode, GeometryFullDto Location);
public record GeometryFullDto(double Latitude, double Longitude);
