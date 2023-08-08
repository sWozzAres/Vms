using Vms.Domain.ServiceBookingProcess;

namespace Vms.Application.Commands.ServiceBookingUseCase;

public interface IAutomaticallyAssignSupplierUseCase
{
    Task<bool> Assign(Guid id, CancellationToken cancellationToken = default);
}

public class AutomaticallyAssignSupplierUseCase(VmsDbContext dbContext, ISupplierLocator locator,
    IActivityLogger activityLog, ITaskLogger taskLogger) : IAutomaticallyAssignSupplierUseCase
{
    readonly VmsDbContext DbContext = dbContext;
    readonly IActivityLogger ActivityLog = activityLog;
    readonly ITaskLogger TaskLogger = taskLogger;
    readonly ISupplierLocator SupplierLocator = locator;
    ServiceBookingRole? ServiceBooking;

    public async Task<bool> Assign(Guid id, CancellationToken cancellationToken = default)
    {
        ServiceBooking = new(await DbContext.ServiceBookings.FindAsync(new object[] { id }, cancellationToken)
            ?? throw new VmsDomainException("Service Booking not found."), this);

        return await ServiceBooking.AutoAssign(cancellationToken);
    }

    class ServiceBookingRole(ServiceBooking self, AutomaticallyAssignSupplierUseCase ctx)
    {
        public async Task<bool> AutoAssign(CancellationToken cancellationToken)
        {
            var list = await ctx.SupplierLocator.GetSuppliers(self, null, cancellationToken);

            // only include suppliers that have not previously refused this booking
            // TODO optimize by adding this to a parameter to .GetSuppliers()
            var notPreviouslyRefused = list.Where(s => s.RefusalCode is null);

            if (!notPreviouslyRefused.Any())
            {
                return false;
            }

            await new AssignSupplierUseCase(ctx.DbContext, ctx.ActivityLog, ctx.TaskLogger)
                .AssignAsync(self.Id, new TaskAssignSupplierCommand() { SupplierCode = notPreviouslyRefused.First().Code }, cancellationToken);

            return true;
        }
    }
}
