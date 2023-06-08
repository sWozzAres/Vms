using Vms.Application.Services;

namespace Vms.Application.UseCase;

public class AssignSupplierUseCase : IAssignSupplierUseCase
{
    readonly VmsDbContext DbContext;
    readonly ISupplierLocator Locator;
    ServiceBookingRole? ServiceBooking;

    public AssignSupplierUseCase(VmsDbContext dbContext, ISupplierLocator locator)
        => (DbContext, Locator) = (dbContext, locator);

    public async Task<AssignSupplierResponse> Assign(AssignSupplierRequest request, CancellationToken cancellationToken = default)
    {
        ServiceBooking = new(await DbContext.ServiceBookings.FindAsync(request.ServiceBookingId, cancellationToken)
            ?? throw new VmsDomainException("Service Booking not found."), this);

        return new AssignSupplierResponse(await ServiceBooking.Assign(cancellationToken));
    }

    class ServiceBookingRole(ServiceBooking self, AssignSupplierUseCase context)
    {
        public async Task<bool> Assign(CancellationToken cancellationToken)
        {
            var list = await context.Locator.GetSuppliers(self, cancellationToken);
            if (!list.Any())
            {
                return false;
            }

            self.AssignSupplier(list.First().Code);
            return true;
        }
    }
}

public interface IAssignSupplierUseCase
{
    Task<AssignSupplierResponse> Assign(AssignSupplierRequest request, CancellationToken cancellationToken = default);
}

public record AssignSupplierRequest(Guid ServiceBookingId);
public record AssignSupplierResponse(bool success);