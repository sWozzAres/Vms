namespace Vms.Web.Shared;

public class TaskBookSupplierDto 
{
    public enum TaskResult { None, Booked, Refused, Rescheduled }

    [Required]
    [Range(typeof(TaskResult), nameof(TaskResult.Booked), nameof(TaskResult.Refused), 
        ErrorMessage = "You must select an option.")]
    public TaskResult Result { get; set; }
    
    [Required]
    public DateOnly BookedDate { get; set; }
    
    [Required]
    public string? RefusalReason { get; set; } = null!;
    
    [RequiredIf(nameof(Result), TaskResult.Rescheduled)]
    public string? RescheduleReason { get; set; } = null!;
    public DateOnly RescheduleDate { get; set; }
    public string RescheduleTime { get; set; } = null!;

    public string? Callee { get; set; } = null!;
}

public record CreateServiceBookingDto(Guid VehicleId);
public class ServiceBookingDto : ICopyable<ServiceBookingDto>
{
    public Guid Id { get; set; }
    public Guid VehicleId { get; set; }
    [Required, StringLength(10)]
    public string CompanyCode { get; set; } = string.Empty;
    public DateOnly? PreferredDate1 { get; set; }
    public DateOnly? PreferredDate2 { get; set; }
    public DateOnly? PreferredDate3 { get; set; }

    public void CopyFrom(ServiceBookingDto source)
    {
        Id = source.Id;
        VehicleId = source.VehicleId;
        CompanyCode = source.CompanyCode;
        PreferredDate1 = source.PreferredDate1;
        PreferredDate2 = source.PreferredDate2;
        PreferredDate3 = source.PreferredDate3;
    }
}

public record ServiceBookingFullDto(Guid Id, Guid VehicleId, string CompanyCode,
    string Vrm, string Make, string Model,
    DateOnly? PreferredDate1, DateOnly? PreferredDate2, DateOnly? PreferredDate3,
    SupplierShortDto? Supplier)
{
    public ServiceBookingDto ToDto()
        => new()
        {
            Id = Id,
            VehicleId = VehicleId,
            CompanyCode = CompanyCode,
            PreferredDate1 = PreferredDate1,
            PreferredDate2 = PreferredDate2,
            PreferredDate3 = PreferredDate3
        };

}