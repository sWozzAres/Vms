using System.Text.Json.Serialization;

namespace Utopia.Blazor.Application.Vms.Shared;

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

public partial class TaskRebookDriverCommand
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
    ServiceLevelDto ServiceLevel,
    bool AutoAssign,
    Guid? MotId,
    Guid? ServiceId,
    List<Guid>? RepairIds);

public enum ServiceLevelDto : int
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
    [Range(typeof(ServiceLevelDto), nameof(ServiceLevelDto.Mobile), nameof(ServiceLevelDto.DropOffWithCourtesyCar),
        ErrorMessage = "You must select a service level.")]
    public ServiceLevelDto ServiceLevel { get; set; }
    public string? AssignedToUserId { get; set; }
    [StringLength(41)]
    public string? Driver_Name { get; set; }
    [StringLength(128)]
    [EmailAddress]
    public string? Driver_EmailAddress { get; set; }
    [StringLength(12)]
    public string? Driver_MobileNumber { get; set; }
    [StringLength(41)]
    public string? Contact_Name { get; set; }
    [StringLength(128)]
    [EmailAddress]
    public string? Contact_EmailAddress { get; set; }
    [StringLength(12)]
    public string? Contact_MobileNumber { get; set; }
    public void CopyFrom(ServiceBookingDto source)
    {
        Id = source.Id;
        VehicleId = source.VehicleId;
        CompanyCode = source.CompanyCode;
        PreferredDate1 = source.PreferredDate1;
        PreferredDate2 = source.PreferredDate2;
        PreferredDate3 = source.PreferredDate3;
        ServiceLevel = source.ServiceLevel;
        Driver_Name = source.Driver_Name;
        Driver_EmailAddress = source.Driver_EmailAddress;
        Driver_MobileNumber = source.Driver_MobileNumber;
        Contact_Name = source.Contact_Name;
        Contact_EmailAddress = source.Contact_EmailAddress;
        Contact_MobileNumber = source.Contact_MobileNumber;
    }
}


//public record ServiceBookingFullDto(Guid Id, Guid VehicleId, string CompanyCode,
//    string Vrm, string Make, string Model,
//    DateOnly? PreferredDate1, DateOnly? PreferredDate2, DateOnly? PreferredDate3,
//    int Status, ServiceLevel ServiceLevel,
//    SupplierShortDto? Supplier, MotEventShortDto? MotEvent, bool IsFollowing)
//{
//}
public enum ServiceBookingListOptions
{
    All = 0,
    Recent = 1,
    Following = 2,
    Assigned = 3,
    Due = 4
}
public class ServiceBookingListDto(Guid id, Guid vehicleId, string @ref, string vrm,
    DateTime? rescheduleTime, ServiceBookingDtoStatus status,
    string? workerName, DateTime? workStarted)
{
    public Guid Id { get; set; } = id;
    public Guid VehicleId { get; set; } = vehicleId;
    public string Ref { get; set; } = @ref;
    public string Vrm { get; set; } = vrm;
    public DateTime? RescheduleTime { get; set; } = rescheduleTime;
    public ServiceBookingDtoStatus Status { get; set; } = status;
    public string? WorkerName { get; set; } = workerName;
    public DateTime? WorkStarted { get; set; } = workStarted;
    [JsonIgnore]
    public string StatusText => ServiceBookingHelper.StatusText(Status);
}
public enum ServiceBookingDtoStatus : int
{
    Cancelled = -2,
    Complete = -1,
    None = 0,
    Assign = 1,
    Book = 2,
    Confirm = 3,
    CheckArrival = 4,
    CheckWorkStatus = 5,
    ChaseDriver = 6,
    RebookDriver = 7,
    NotifyCustomer = 8,
    NotifyCustomerDelay = 9
};
public static class ServiceBookingHelper
{
    public static string StatusText(ServiceBookingDtoStatus status) => status switch
    {
        ServiceBookingDtoStatus.Cancelled => "Cancelled",
        ServiceBookingDtoStatus.Complete => "Complete",
        ServiceBookingDtoStatus.None => "None",
        ServiceBookingDtoStatus.Assign => "Assign",
        ServiceBookingDtoStatus.Book => "Book",
        ServiceBookingDtoStatus.Confirm => "Confirm",
        ServiceBookingDtoStatus.CheckArrival => "Check Arrival",
        ServiceBookingDtoStatus.CheckWorkStatus => "Check Work Status",
        ServiceBookingDtoStatus.ChaseDriver => "Chase Driver",
        ServiceBookingDtoStatus.RebookDriver => "Rebook Driver",
        ServiceBookingDtoStatus.NotifyCustomer => "Notify Customer",
        ServiceBookingDtoStatus.NotifyCustomerDelay => "Notify Customer Delay",
        _ => "Unknown"
    };

}
public class ServiceBookingFullDto
{
    public Guid Id { get; set; }
    public Guid VehicleId { get; set; }
    public string Ref { get; set; } = null!;
    public string CompanyCode { get; set; } = null!;
    public string Vrm { get; set; } = null!;
    public string Make { get; set; } = null!;
    public string Model { get; set; } = null!;
    public DateOnly? PreferredDate1 { get; set; }
    public DateOnly? PreferredDate2 { get; set; }
    public DateOnly? PreferredDate3 { get; set; }
    public ServiceBookingDtoStatus Status { get; set; }
    public ServiceLevelDto ServiceLevel { get; set; }
    public string? Supplier_Code { get; set; }
    public string? Supplier_Name { get; set; }
    public Guid? MotEvent_Id { get; set; }
    public DateOnly? MotEvent_Due { get; set; }
    public bool IsFollowing { get; set; }
    public string AssignedToUserId { get; set; } = null!;
    public DateTime? RescheduleTime { get; set; }
    public string? Driver_Name { get; set; }
    public string? Driver_EmailAddress { get; set; }
    public string? Driver_MobileNumber { get; set; }
    public string? Contact_Name { get; set; }
    public string? Contact_EmailAddress { get; set; }
    public string? Contact_MobileNumber { get; set; }
    public string? Worker_Name { get; set; }
    public DateTime? WorkStarted { get; set; }
    public string? CustomerCode { get; set; }
    public string? FleetCode { get; set; }
    [JsonIgnore]
    public string StatusText => ServiceBookingHelper.StatusText(Status);

    public ServiceBookingDto ToDto()
        => new()
        {
            Id = Id,
            VehicleId = VehicleId,
            CompanyCode = CompanyCode,
            PreferredDate1 = PreferredDate1,
            PreferredDate2 = PreferredDate2,
            PreferredDate3 = PreferredDate3,
            ServiceLevel = ServiceLevel,
            AssignedToUserId = AssignedToUserId,
            Driver_Name = Driver_Name,
            Driver_EmailAddress = Driver_EmailAddress,
            Driver_MobileNumber = Driver_MobileNumber,
            Contact_Name = Contact_Name,
            Contact_EmailAddress = Contact_EmailAddress,
            Contact_MobileNumber = Contact_MobileNumber
        };
}

