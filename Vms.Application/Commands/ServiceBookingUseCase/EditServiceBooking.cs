using Vms.Domain.ServiceBookingProcess;

namespace Vms.Application.Commands.ServiceBookingUseCase;

public class EditServiceBooking(
    VmsDbContext dbContext,
    IActivityLogger<VmsDbContext> activityLog,
    ITaskLogger<VmsDbContext> taskLogger,
    ILogger<EditServiceBooking> logger,
    ITimeService timeService) : ServiceBookingTaskBase(dbContext, activityLog)
{
    readonly ITimeService TimeService = timeService;
    ServiceBookingRole? ServiceBooking;
    ServiceBookingDto Command = null!;

    public async Task<bool> EditAsync(Guid serviceBookingId, ServiceBookingDto command, CancellationToken cancellationToken)
    {
        logger.LogInformation("Editing service booking: {servicebookingid}, command: {@servicebookingdto}.", serviceBookingId, command);
        
        Command = command;
        ServiceBooking = new(await Load(serviceBookingId, cancellationToken), this);

        var isModified = await ServiceBooking.ModifyDocument();
        if (isModified)
        {
            await LogActivity();
            taskLogger.Log(serviceBookingId, nameof(EditServiceBooking), command);
        }

        return isModified;
    }
    class ServiceBookingRole(ServiceBooking self, EditServiceBooking ctx) : ServiceBookingRoleBase<EditServiceBooking>(self, ctx)
    {
        public async Task<bool> ModifyDocument()
        {
            Ctx.SummaryText.AppendLine("# Edit");

            bool isModified = false;

            if (Self.PreferredDate1 != Ctx.Command.PreferredDate1)
            {
                Ctx.SummaryText.AppendLine($"* Preferred Date 1: {Ctx.Command.PreferredDate1}");
                Self.PreferredDate1 = Ctx.Command.PreferredDate1;
                isModified = true;
            }
            if (Self.PreferredDate2 != Ctx.Command.PreferredDate2)
            {
                Ctx.SummaryText.AppendLine($"* Preferred Date 2: {Ctx.Command.PreferredDate2}");
                Self.PreferredDate2 = Ctx.Command.PreferredDate2;
                isModified = true;
            }
            if (Self.PreferredDate3 != Ctx.Command.PreferredDate3)
            {
                Ctx.SummaryText.AppendLine($"* Preferred Date 3: {Ctx.Command.PreferredDate3}");
                Self.PreferredDate3 = Ctx.Command.PreferredDate3;
                isModified = true;
            }

            var sl = (ServiceLevel)Ctx.Command.ServiceLevel;
            if (Self.ServiceLevel != sl)
            {
                Ctx.SummaryText.AppendLine($"* Service Level: {sl.ToDisplayString()}");
                Self.ServiceLevel = sl;
                isModified = true;
            }

            if (Self.AssignedToUserId != Ctx.Command.AssignedToUserId)
            {
                if (Ctx.Command.AssignedToUserId is null)
                {
                    Ctx.SummaryText.AppendLine($"* Removed Assigned User");
                }
                else
                {
                    var user = await Ctx.DbContext.Users.AsNoTracking()
                        .SingleAsync(u => u.UserId == Ctx.Command.AssignedToUserId, Ctx.CancellationToken);

                    Ctx.SummaryText.AppendLine($"* Assigned To: {user.UserName}");
                }
                Self.AssignedToUserId = Ctx.Command.AssignedToUserId;
                isModified = true;
            }

            if (Self.Driver.Name != Ctx.Command.Driver_Name)
            {
                Ctx.SummaryText.AppendLine($"* Driver Name: {Ctx.Command.Driver_Name}");
                Self.Driver.Name = Ctx.Command.Driver_Name;
                isModified = true;
            }
            if (Self.Driver.EmailAddress != Ctx.Command.Driver_EmailAddress)
            {
                Ctx.SummaryText.AppendLine($"* Driver Email Address: [{Ctx.Command.Driver_EmailAddress}](mailto://{Ctx.Command.Driver_EmailAddress})");
                Self.Driver.EmailAddress = Ctx.Command.Driver_EmailAddress;
                isModified = true;
            }
            if (Self.Driver.MobileNumber != Ctx.Command.Driver_MobileNumber)
            {
                Ctx.SummaryText.AppendLine($"* Driver Mobile Number: {Ctx.Command.Driver_MobileNumber}");
                Self.Driver.MobileNumber = Ctx.Command.Driver_MobileNumber;
                isModified = true;
            }
            if (Self.Contact.Name != Ctx.Command.Contact_Name)
            {
                Ctx.SummaryText.AppendLine($"* Contact Name: {Ctx.Command.Contact_Name}");
                Self.Contact.Name = Ctx.Command.Contact_Name;
                isModified = true;
            }
            if (Self.Contact.EmailAddress != Ctx.Command.Contact_EmailAddress)
            {
                Ctx.SummaryText.AppendLine($"* Contact Email Address: [{Ctx.Command.Contact_EmailAddress}](mailto://{Ctx.Command.Contact_EmailAddress})");
                Self.Contact.EmailAddress = Ctx.Command.Contact_EmailAddress;
                isModified = true;
            }
            if (Self.Contact.MobileNumber != Ctx.Command.Contact_MobileNumber)
            {
                Ctx.SummaryText.AppendLine($"* Contact Mobile Number: {Ctx.Command.Contact_MobileNumber}");
                Self.Contact.MobileNumber = Ctx.Command.Contact_MobileNumber;
                isModified = true;
            }

            if (Self.Status == ServiceBookingStatus.None && Self.IsReady)
            {
                Self.ChangeStatus(ServiceBookingStatus.Assign, Ctx.TimeService.Now);
                Ctx.SummaryText.AppendLine($"* Status: {Self.Status.ToDisplayString()}");
                isModified = true;
            }

            return isModified;
        }
    }
}
