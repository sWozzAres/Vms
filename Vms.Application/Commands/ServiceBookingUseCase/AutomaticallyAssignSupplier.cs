using Microsoft.Extensions.Logging;
using Vms.Domain.ServiceBookingProcess;

namespace Vms.Application.Commands.ServiceBookingUseCase;

public interface IAutomaticallyAssignSupplier
{
    Task<bool> Assign(Guid id, CancellationToken cancellationToken = default);
}

public class AutomaticallyAssignSupplier(VmsDbContext dbContext, ISupplierLocator locator,
    IActivityLogger activityLog, ITaskLogger taskLogger, 
    ILogger<AutomaticallyAssignSupplier> logger,
    IAssignSupplier assignSupplier) : IAutomaticallyAssignSupplier
{
    readonly VmsDbContext DbContext = dbContext;
    readonly IActivityLogger ActivityLog = activityLog;
    readonly ITaskLogger TaskLogger = taskLogger;
    readonly ISupplierLocator SupplierLocator = locator;
    readonly ILogger Logger = logger;
    readonly IAssignSupplier AssignSupplier = assignSupplier;
    ServiceBookingRole? ServiceBooking;

    public async Task<bool> Assign(Guid id, CancellationToken cancellationToken = default)
    {
        Logger.LogInformation("AutomaticallyAssignSupplier task for service booking {servicebookingid}.", id);

        ServiceBooking = new(await DbContext.ServiceBookings.FindAsync(new object[] { id }, cancellationToken)
            ?? throw new VmsDomainException("Service Booking not found."), this);

        return await ServiceBooking.AutoAssign(cancellationToken);
    }

    class ServiceBookingRole(ServiceBooking self, AutomaticallyAssignSupplier ctx)
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

            await ctx.AssignSupplier// new AssignSupplier(ctx.DbContext, ctx.ActivityLog, ctx.TaskLogger, ctx.Logger)
                .AssignAsync(self.Id, new TaskAssignSupplierCommand() { SupplierCode = notPreviouslyRefused.First().Code }, cancellationToken);

            return true;
        }
    }
}
