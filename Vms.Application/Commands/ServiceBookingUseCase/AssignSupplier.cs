using Vms.Domain.ServiceBookingProcess;

namespace Vms.Application.Commands.ServiceBookingUseCase;

public class AssignSupplier(VmsDbContext dbContext,
    IActivityLogger<VmsDbContext> activityLog,
    ITaskLogger<VmsDbContext> taskLogger,
    ILogger<AssignSupplier> logger,
    ITimeService timeService) : ServiceBookingTaskBase(dbContext, activityLog)
{
    readonly ITimeService TimeService = timeService;
    ServiceBookingRole? ServiceBooking;
    TaskAssignSupplierCommand Command = null!;

    public async Task AssignAsync(Guid serviceBookingId, TaskAssignSupplierCommand command, CancellationToken cancellationToken)
    {
        logger.LogInformation("Assigning supplier for service booking: {servicebookingid}, command: {@taskassignsuppliercommand}", serviceBookingId, command);

        Command = command;
        ServiceBooking = new(await Load(serviceBookingId, cancellationToken), this);

        await ServiceBooking.Assign();

        await LogActivity();
        taskLogger.Log(serviceBookingId, nameof(AssignSupplier), Command);
    }

    class ServiceBookingRole(ServiceBooking self, AssignSupplier ctx) : ServiceBookingRoleBase<AssignSupplier>(self, ctx)
    {
        public async Task Assign()
        {
            if (Self.Status != ServiceBookingStatus.Assign && Self.Status != ServiceBookingStatus.Book)
                throw new VmsDomainException("Service Booking is not in Assign or Book status.");

            Ctx.SummaryText.AppendLine("# Assign Supplier");

            var supplier = await Ctx.DbContext.Suppliers.AsNoTracking()
                .SingleAsync(s => s.Code == Ctx.Command.SupplierCode, Ctx.CancellationToken);

            Ctx.SummaryText.AppendLine($"* Code: {supplier.Code}");
            Ctx.SummaryText.AppendLine($"* Name: {supplier.Name}");

            Self.Assign(Ctx.Command.SupplierCode);
            Self.ChangeStatus(ServiceBookingStatus.Book, Ctx.TimeService.Now);
        }
    }
}