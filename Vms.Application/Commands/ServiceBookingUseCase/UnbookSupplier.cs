using Vms.Domain.ServiceBookingProcess;

namespace Vms.Application.Commands.ServiceBookingUseCase;

public class UnbookSupplier(
    VmsDbContext dbContext,
    IActivityLogger<VmsDbContext> activityLog,
    ITaskLogger<VmsDbContext> taskLogger,
    ILogger<UnbookSupplier> logger,
    ITimeService timeService) : ServiceBookingTaskBase(dbContext, activityLog)
{
    readonly ITimeService TimeService = timeService;
    ServiceBookingRole? ServiceBooking;
    TaskUnbookSupplierCommand Command = null!;

    public async Task UnbookAsync(Guid serviceBookingId, TaskUnbookSupplierCommand command, CancellationToken cancellationToken)
    {
        logger.LogInformation("Unbooking supplier for service booking: {servicebookingid}, command: {@taskunbooksuppliercommand}.", serviceBookingId, command);

        Command = command;
        ServiceBooking = new(await Load(serviceBookingId, cancellationToken), this);

        SummaryText.AppendLine("# Unbook Supplier");

        ServiceBooking.Unbook();

        await LogActivity();
        taskLogger.Log(serviceBookingId, nameof(UnbookSupplier), command);
    }
    class ServiceBookingRole(ServiceBooking self, UnbookSupplier ctx) : ServiceBookingRoleBase<UnbookSupplier>(self, ctx)
    {
        public void Unbook()
        {
            Ctx.SummaryText.AppendLine($"* Reason: {Ctx.Command.Reason}");

            Self.Unbook();
            Self.ChangeStatus(ServiceBookingStatus.Book, Ctx.TimeService.Now);
        }
    }
}
