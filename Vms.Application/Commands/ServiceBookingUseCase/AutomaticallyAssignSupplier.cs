using Vms.Domain.ServiceBookingProcess;

namespace Vms.Application.Commands.ServiceBookingUseCase;

public class AutomaticallyAssignSupplier(
    VmsDbContext dbContext,
    IActivityLogger<VmsDbContext> activityLog,
    SupplierLocator locator,
    ILogger<AutomaticallyAssignSupplier> logger,
    AssignSupplier assignSupplier) : ServiceBookingTaskBase(dbContext, activityLog)
{
    readonly SupplierLocator SupplierLocator = locator;
    readonly ILogger Logger = logger;
    readonly AssignSupplier AssignSupplier = assignSupplier;
    ServiceBookingRole? ServiceBooking;

    public async Task<bool> AutoAssign(Guid serviceBookingId, CancellationToken cancellationToken = default)
    {
        Logger.LogInformation("Automatically assigning supplier service booking: {servicebookingid}.", serviceBookingId);

        ServiceBooking = new(await Load(serviceBookingId, cancellationToken), this);

        var assigned = await ServiceBooking.AutoAssign();

        await LogActivity();

        return assigned;
    }

    class ServiceBookingRole(ServiceBooking self, AutomaticallyAssignSupplier ctx) : ServiceBookingRoleBase<AutomaticallyAssignSupplier>(self, ctx)
    {
        public async Task<bool> AutoAssign()
        {
            var list = await Ctx.SupplierLocator.GetSuppliers(Self, null, Ctx.CancellationToken);

            // only include suppliers that have not previously refused this booking
            // TODO optimize by adding this to a parameter to .GetSuppliers()
            var notPreviouslyRefused = list.Where(s => s.RefusalCode is null);

            if (!notPreviouslyRefused.Any())
            {
                return false;
            }

            Ctx.DbContext.ThrowIfNoTransaction();
            await Ctx.AssignSupplier
                .AssignAsync(Self.Id, new TaskAssignSupplierCommand() { SupplierCode = notPreviouslyRefused.First().Code }, Ctx.CancellationToken);

            return true;
        }
    }
}
