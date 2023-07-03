namespace Vms.Web.Shared;

public record DriverShortDto(Guid Id, string companyCode, string EmailAddress, string FullName, string MobileNumber);

public record VehicleFullDto(string CompanyCode, Guid Id, string Vrm, string Make, string Model, string? ChassisNumber,
    DateOnly DateFirstRegistered, AddressFullDto Address, CustomerShortDto? Customer,
    FleetShortDto? Fleet, List<DriverShortDto> Drivers)
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

public record CustomerShortDto(string CompanyCode, string Code, string Name);
public record FleetShortDto(string Code, string Name);
public record AddressFullDto(string Street, string Locality, string Town, string Postcode, GeometryFullDto Location);
public record GeometryFullDto(double Latitude, double Longitude);
