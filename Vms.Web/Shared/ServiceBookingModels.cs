using System.Text.Json.Serialization;

namespace Vms.Web.Shared;

public class TaskUnbookSupplierCommand
{
    public enum TaskResult { None, Unbooked }
    [Required]
    [Range(typeof(TaskResult), nameof(TaskResult.Unbooked), nameof(TaskResult.Unbooked),
        ErrorMessage = "You must select an option.")]
    public TaskResult Result { get; set; }

    [RequiredIf(nameof(Result), TaskResult.Unbooked, ErrorMessage = "You must supply a reason.")]
    public string? Reason { get; set; }
}

public class TaskChaseDriverCommand
{
    public enum TaskResult { None, StillGoing, NotGoing, Rescheduled }
    [Required]
    [Range(typeof(TaskResult), nameof(TaskResult.StillGoing), nameof(TaskResult.Rescheduled),
        ErrorMessage = "You must select an option.")]
    public TaskResult Result { get; set; }

    [RequiredIf(nameof(Result), TaskResult.Rescheduled, ErrorMessage = "The Reschedule Reason is required.")]
    public string? RescheduleReason { get; set; }

    [RequiredIf(nameof(Result), TaskResult.Rescheduled, ErrorMessage = "The Reschedule Date is required.")]
    public DateOnly? RescheduleDate { get; set; }

    [RequiredIf(nameof(Result), TaskResult.Rescheduled, ErrorMessage = "The Reschedule Time is required.")]
    public TimeOnly? RescheduleTime { get; set; }

    public string? Callee { get; set; }
}

public class TaskRebookDriverCommand
{
    public enum TaskResult { None, StillGoing, NotGoing, StillGoingToday, Rescheduled }
    [Required]
    [Range(typeof(TaskResult), nameof(TaskResult.StillGoing), nameof(TaskResult.Rescheduled),
        ErrorMessage = "You must select an option.")]
    public TaskResult Result { get; set; }

    [RequiredIf(nameof(Result), TaskResult.StillGoingToday, ErrorMessage = "The Arrival Time is required.")]
    public TimeOnly? ArrivalTime { get; set; }

    [RequiredIf(nameof(Result), TaskResult.Rescheduled, ErrorMessage = "The Reschedule Reason is required.")]
    public string? RescheduleReason { get; set; }

    [RequiredIf(nameof(Result), TaskResult.Rescheduled, ErrorMessage = "The Reschedule Date is required.")]
    public DateOnly? RescheduleDate { get; set; }

    [RequiredIf(nameof(Result), TaskResult.Rescheduled, ErrorMessage = "The Reschedule Time is required.")]
    public TimeOnly? RescheduleTime { get; set; }

    public string? Callee { get; set; }
}

public class TaskAssignSupplierCommand
{
    [Required]
    public string SupplierCode { get; set; } = null!;
}

public class TaskBookSupplierCommand
{
    public enum TaskResult { None, Booked, Refused, Rescheduled }

    [Required]
    [Range(typeof(TaskResult), nameof(TaskResult.Booked), nameof(TaskResult.Rescheduled),
        ErrorMessage = "You must select an option.")]
    public TaskResult Result { get; set; }

    [RequiredIf(nameof(Result), TaskResult.Booked, ErrorMessage = "The Booked Date is required.")]
    public DateOnly? BookedDate { get; set; }

    [RequiredIf(nameof(Result), TaskResult.Refused, ErrorMessage = "The Refusal Reason is required.")]
    public string? RefusalReason { get; set; }

    [RequiredIf(nameof(Result), TaskResult.Rescheduled, ErrorMessage = "The Reschedule Reason is required.")]
    public string? RescheduleReason { get; set; }

    [RequiredIf(nameof(Result), TaskResult.Rescheduled, ErrorMessage = "The Reschedule Date is required.")]
    public DateOnly? RescheduleDate { get; set; }

    [RequiredIf(nameof(Result), TaskResult.Rescheduled, ErrorMessage = "The Reschedule Time is required.")]
    public TimeOnly? RescheduleTime { get; set; }

    public string? Callee { get; set; }
}

public class TaskCheckWorkStatusCommand
{
    public enum TaskResult { None, Complete, NotComplete, Rescheduled }

    [Required]
    [Range(typeof(TaskResult), nameof(TaskResult.Complete), nameof(TaskResult.Rescheduled),
        ErrorMessage = "You must select an option.")]
    public TaskResult Result { get; set; }

    [RequiredIf(nameof(Result), TaskResult.Complete, ErrorMessage = "The Completion Date is required.")]
    public DateOnly? CompletionDate { get; set; }

    [RequiredIf(nameof(Result), TaskResult.NotComplete, ErrorMessage = "The Not Complete Reason is required.")]
    public string? NotCompleteReason { get; set; }

    [RequiredIf(nameof(Result), TaskResult.NotComplete, ErrorMessage = "The Next Chase Date is required.")]
    public DateOnly? NextChaseDate { get; set; }

    [RequiredIf(nameof(Result), TaskResult.NotComplete, ErrorMessage = "The Next Chase Time is required.")]
    public TimeOnly? NextChaseTime { get; set; }

    [RequiredIf(nameof(Result), TaskResult.Rescheduled, ErrorMessage = "The Reschedule Reason is required.")]
    public string? RescheduleReason { get; set; }

    [RequiredIf(nameof(Result), TaskResult.Rescheduled, ErrorMessage = "The Reschedule Date is required.")]
    public DateOnly? RescheduleDate { get; set; }

    [RequiredIf(nameof(Result), TaskResult.Rescheduled, ErrorMessage = "The Reschedule Time is required.")]
    public TimeOnly? RescheduleTime { get; set; }

    public string? Callee { get; set; }
}

public class TaskCheckArrivalCommand
{
    public enum TaskResult { None, Arrived, NotArrived, Rescheduled }

    [Required]
    [Range(typeof(TaskResult), nameof(TaskResult.Arrived), nameof(TaskResult.Rescheduled),
        ErrorMessage = "You must select an option.")]
    public TaskResult Result { get; set; }

    [RequiredIf(nameof(Result), TaskResult.Arrived, ErrorMessage = "The Arrival Date is required.")]
    public DateOnly? ArrivalDate { get; set; }

    [RequiredIf(nameof(Result), TaskResult.Arrived, ErrorMessage = "The Arrival Time is required.")]
    public TimeOnly? ArrivalTime { get; set; }

    [RequiredIf(nameof(Result), TaskResult.NotArrived, ErrorMessage = "The Non Arrival Reason is required.")]
    public string? NonArrivalReason { get; set; }

    [RequiredIf(nameof(Result), TaskResult.Rescheduled, ErrorMessage = "The Reschedule Reason is required.")]
    public string? RescheduleReason { get; set; }

    [RequiredIf(nameof(Result), TaskResult.Rescheduled, ErrorMessage = "The Reschedule Date is required.")]
    public DateOnly? RescheduleDate { get; set; }

    [RequiredIf(nameof(Result), TaskResult.Rescheduled, ErrorMessage = "The Reschedule Time is required.")]
    public TimeOnly? RescheduleTime { get; set; }

    public string? Callee { get; set; }
}

public class TaskConfirmBookedCommand
{
    public enum TaskResult { None, Confirmed, Refused, Rescheduled }

    [Required]
    [Range(typeof(TaskResult), nameof(TaskResult.Confirmed), nameof(TaskResult.Rescheduled),
        ErrorMessage = "You must select an option.")]
    public TaskResult Result { get; set; }

    [RequiredIf(nameof(Result), TaskResult.Refused, ErrorMessage = "The Refusal Reason is required.")]
    public string? RefusalReason { get; set; }

    [RequiredIf(nameof(Result), TaskResult.Rescheduled, ErrorMessage = "The Reschedule Reason is required.")]
    public string? RescheduleReason { get; set; }

    [RequiredIf(nameof(Result), TaskResult.Rescheduled, ErrorMessage = "The Reschedule Date is required.")]
    public DateOnly? RescheduleDate { get; set; }

    [RequiredIf(nameof(Result), TaskResult.Rescheduled, ErrorMessage = "The Reschedule Time is required.")]
    public TimeOnly? RescheduleTime { get; set; }

    public string? Callee { get; set; }
}

public class TaskNotifyCustomerCommand
{
    public enum TaskResult { None, Notified, Rescheduled }

    [Required]
    [Range(typeof(TaskResult), nameof(TaskResult.Notified), nameof(TaskResult.Rescheduled),
        ErrorMessage = "You must select an option.")]
    public TaskResult Result { get; set; }

    [RequiredIf(nameof(Result), TaskResult.Rescheduled, ErrorMessage = "The Reschedule Reason is required.")]
    public string? RescheduleReason { get; set; }

    [RequiredIf(nameof(Result), TaskResult.Rescheduled, ErrorMessage = "The Reschedule Date is required.")]
    public DateOnly? RescheduleDate { get; set; }

    [RequiredIf(nameof(Result), TaskResult.Rescheduled, ErrorMessage = "The Reschedule Time is required.")]
    public TimeOnly? RescheduleTime { get; set; }

    public string? Callee { get; set; }
}
public class TaskNotifyCustomerDelayCommand
{
    public enum TaskResult { None, Notified, Rescheduled }

    [Required]
    [Range(typeof(TaskResult), nameof(TaskResult.Notified), nameof(TaskResult.Rescheduled),
        ErrorMessage = "You must select an option.")]
    public TaskResult Result { get; set; }

    [RequiredIf(nameof(Result), TaskResult.Rescheduled, ErrorMessage = "The Reschedule Reason is required.")]
    public string? RescheduleReason { get; set; }

    [RequiredIf(nameof(Result), TaskResult.Rescheduled, ErrorMessage = "The Reschedule Date is required.")]
    public DateOnly? RescheduleDate { get; set; }

    [RequiredIf(nameof(Result), TaskResult.Rescheduled, ErrorMessage = "The Reschedule Time is required.")]
    public TimeOnly? RescheduleTime { get; set; }

    public string? Callee { get; set; }
}

public class TaskChangeMotStatusCommand
{
    public enum MotStatus { None, Active, Complete }
    public enum TaskResult { None, Change }

    [Required]
    [Range(typeof(TaskResult), nameof(TaskResult.Change), nameof(TaskResult.Change),
        ErrorMessage = "You must select an option.")]
    public TaskResult Result { get; set; }


    [RequiredIf(nameof(Result), TaskResult.Change, ErrorMessage = "The Status is required.")]
    public MotStatus? Status { get; set; }
}

public record CreateServiceBookingCommand(
    Guid VehicleId,
    DateOnly? PreferredDate1,
    DateOnly? PreferredDate2,
    DateOnly? PreferredDate3,
    bool AutoAssign,
    Guid? MotId,
    Guid? ServiceId,
    List<Guid>? RepairIds);

public enum ServiceLevel : int
{
    None = 0,
    Mobile = 1,
    Collection = 2,
    DropOff = 3,
    WhileYouWait = 4,
    DropOffWithCourtesyCar = 5,
};

public class ServiceBookingDto : ICopyable<ServiceBookingDto>
{
    public Guid Id { get; set; }
    public Guid VehicleId { get; set; }
    [Required, StringLength(10)]
    public string CompanyCode { get; set; } = string.Empty;
    public DateOnly? PreferredDate1 { get; set; }
    public DateOnly? PreferredDate2 { get; set; }
    public DateOnly? PreferredDate3 { get; set; }
    [Range(typeof(ServiceLevel), nameof(ServiceLevel.Mobile), nameof(ServiceLevel.DropOffWithCourtesyCar), 
        ErrorMessage = "You must select a service level.")]
    public ServiceLevel ServiceLevel { get; set; }
    public void CopyFrom(ServiceBookingDto source)
    {
        Id = source.Id;
        VehicleId = source.VehicleId;
        CompanyCode = source.CompanyCode;
        PreferredDate1 = source.PreferredDate1;
        PreferredDate2 = source.PreferredDate2;
        PreferredDate3 = source.PreferredDate3;
        ServiceLevel = source.ServiceLevel;
    }
}


public record ServiceBookingFullDto(Guid Id, Guid VehicleId, string CompanyCode,
    string Vrm, string Make, string Model,
    DateOnly? PreferredDate1, DateOnly? PreferredDate2, DateOnly? PreferredDate3,
    int Status, ServiceLevel ServiceLevel,
    SupplierShortDto? Supplier, MotEventShortDto? MotEvent)
{
    [JsonIgnore]
    public string StatusText => Status switch
    {
        0 => "None",
        1 => "Assign",
        2 => "Book",
        3 => "Confirm",
        4 => "Check Arrival",
        5 => "Check Work Status",
        6 => "Chase Driver",
        7 => "Rebook Driver",
        8 => "Notify Customer",
        9 => "Notify Customer Delay",
        _ => "Unknown"
    };

    public ServiceBookingDto ToDto()
        => new()
        {
            Id = Id,
            VehicleId = VehicleId,
            CompanyCode = CompanyCode,
            PreferredDate1 = PreferredDate1,
            PreferredDate2 = PreferredDate2,
            PreferredDate3 = PreferredDate3,
            ServiceLevel = ServiceLevel
        };
}