namespace Utopia.Blazor.Application.Vms.Shared;
public record MotEventShortDto(Guid Id, DateOnly Due);
public record VehicleEvents(List<MotEventDto> Mots);
public record MotEventDto(Guid Id, DateOnly Due);

public record AssignCustomerToVehicleCommand(string CustomerCode);
public record AssignFleetToVehicleCommand(string FleetCode);
public record AddDriverToVehicleCommand(Guid DriverId);
public enum VehicleListOptions
{
    All = 0,
    Recent = 1,
    Following = 2,
    DueMot = 3
}
public record VehicleListDto(Guid Id, string CompanyCode, string Vrm, string Make, string Model,
    string? CustomerCode, string? CustomerName,
    string? FleetCode, string? FleetName,
    DateOnly MotDue);








//public record VehicleFullDto(string CompanyCode, Guid Id, string Vrm, string Make, string Model, string? ChassisNumber,
//    DateOnly DateFirstRegistered, DateOnly? MotDue, AddressFullDto Address, CustomerShortDto? Customer,
//    FleetShortDto? Fleet, List<DriverShortDto> Drivers, bool IsFollowing)
//{
//    public bool IsFollowing { get; set; } = IsFollowing;

//    public VehicleDto ToDto()
//        => new()
//        {
//            CompanyCode = CompanyCode,
//            Id = Id,
//            Vrm = Vrm,
//            Make = Make,
//            Model = Model,
//            ChassisNumber = ChassisNumber,
//            DateFirstRegistered = DateFirstRegistered,
//            MotDue = MotDue,
//            Address = new AddressDto()
//            {
//                Street = Address.Street,
//                Locality = Address.Locality,
//                Town = Address.Town,
//                Postcode = Address.Postcode,
//                Location = new GeometryDto()
//                {
//                    Latitude = Address.Location.Latitude,
//                    Longitude = Address.Location.Longitude,
//                }
//            },
//            CustomerCode = Customer?.Code,
//            FleetCode = Fleet?.Code,
//        };
//}
public class VehicleFullDto(string companyCode, Guid id, string vrm, string make, string model, string? chassisNumber, DateOnly dateFirstRegistered, DateOnly? motDue, AddressFullDto address, CustomerShortDto? customer, FleetShortDto? fleet, List<DriverShortDto> drivers, bool isFollowing)
{
    public string CompanyCode { get; set; } = companyCode;
    public Guid Id { get; set; } = id;
    public string Vrm { get; set; } = vrm;
    public string Make { get; set; } = make;
    public string Model { get; set; } = model;
    public string? ChassisNumber { get; set; } = chassisNumber;
    public DateOnly DateFirstRegistered { get; set; } = dateFirstRegistered;
    public DateOnly? MotDue { get; set; } = motDue;
    public AddressFullDto Address { get; set; } = address;
    public CustomerShortDto? Customer { get; set; } = customer;
    public FleetShortDto? Fleet { get; set; } = fleet;
    public List<DriverShortDto> Drivers { get; set; } = drivers;
    public bool IsFollowing { get; set; } = isFollowing;

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
            MotDue = MotDue,
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
    [DateOnlyRange(2000, 1, 1, 2050, 1, 1, true)]
    public DateOnly? MotDue { get; set; }
    [ValidateComplexType]
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