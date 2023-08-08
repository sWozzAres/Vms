using Vms.Domain.ServiceBookingProcess;

namespace Vms.Application.Commands.ServiceBookingUseCase;

public interface IAssignSupplierUseCase
{
    Task AssignAsync(Guid id, TaskAssignSupplierCommand command, CancellationToken cancellationToken = default);
}

public class AssignSupplierUseCase(VmsDbContext dbContext, IActivityLogger activityLog, ITaskLogger taskLogger) : IAssignSupplierUseCase
{
    readonly VmsDbContext DbContext = dbContext;
    readonly StringBuilder SummaryText = new();

    ServiceBookingRole? ServiceBooking;
    Guid Id;
    TaskAssignSupplierCommand Command = null!;
    CancellationToken CancellationToken;

    public async Task AssignAsync(Guid id, TaskAssignSupplierCommand command, CancellationToken cancellationToken)
    {
        Id = id;
        Command = command ?? throw new ArgumentNullException(nameof(command));
        CancellationToken = cancellationToken;

        ServiceBooking = new(await DbContext.ServiceBookings.FindAsync(new object[] { Id }, CancellationToken)
            ?? throw new InvalidOperationException("Service Booking not found."), this);

        await ServiceBooking.Assign();

        await activityLog.AddAsync(id, SummaryText, CancellationToken);
        taskLogger.Log(id, "Assign Supplier", Command);
    }

    class ServiceBookingRole(ServiceBooking self, AssignSupplierUseCase ctx)
    {
        public async Task Assign()
        {
            if (self.Status != ServiceBookingStatus.Assign && self.Status != ServiceBookingStatus.Book)
                throw new VmsDomainException("Service Booking is not in Assign or Book status.");

            ctx.SummaryText.AppendLine("# Assign Supplier");

            var supplier = await ctx.DbContext.Suppliers.FindAsync(new object[] { ctx.Command.SupplierCode }, ctx.CancellationToken)
                ?? throw new InvalidOperationException("Supplier not found.");

            ctx.SummaryText.AppendLine($"* Code: {supplier.Code}");
            ctx.SummaryText.AppendLine($"* Name: {supplier.Name}");

            self.Assign(ctx.Command.SupplierCode);
        }
    }
}