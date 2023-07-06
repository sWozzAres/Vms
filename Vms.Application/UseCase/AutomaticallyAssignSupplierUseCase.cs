using Vms.Application.Services;

namespace Vms.Application.UseCase;

public interface IAssignSupplierUseCase
{
    Task<bool> Assign(Guid id, CancellationToken cancellationToken = default);
}

public class AutomaticallyAssignSupplierUseCase : IAssignSupplierUseCase
{
    readonly VmsDbContext DbContext;
    readonly ISupplierLocator Locator;
    ServiceBookingRole? ServiceBooking;

    public AutomaticallyAssignSupplierUseCase(VmsDbContext dbContext, ISupplierLocator locator)
        => (DbContext, Locator) = (dbContext, locator);

    public async Task<bool> Assign(Guid id, CancellationToken cancellationToken = default)
    {
        ServiceBooking = new(await DbContext.ServiceBookings.FindAsync(id, cancellationToken)
            ?? throw new VmsDomainException("Service Booking not found."), this);

        return await ServiceBooking.AutoAssign(cancellationToken);
    }

    class ServiceBookingRole(ServiceBooking self, AutomaticallyAssignSupplierUseCase context)
    {
        public async Task<bool> AutoAssign(CancellationToken cancellationToken)
        {
            var list = await context.Locator.GetSuppliers(self, cancellationToken);
            if (!list.Any())
            {
                return false;
            }

            self.SupplierCode = list.First().Code;
            return true;
        }
    }
}