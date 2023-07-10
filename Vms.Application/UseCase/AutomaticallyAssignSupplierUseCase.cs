using Vms.Application.Services;
using Vms.Web.Shared;

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

            //self.SupplierCode = list.First().Code;
            await new AssignSupplierUseCase(context.DbContext)
                .Assign(self.Id, new TaskAssignSupplierCommand() { SupplierCode = list.First().Code }, cancellationToken);
            return true;
        }
    }
}

public class AssignSupplierUseCase
{
    readonly VmsDbContext DbContext;
    //ServiceBookingRole? ServiceBooking;

    public AssignSupplierUseCase(VmsDbContext dbContext)
        => DbContext = dbContext;

    public async Task Assign(Guid id, TaskAssignSupplierCommand command, CancellationToken cancellationToken = default)
    {
        var serviceBooking = await DbContext.ServiceBookings.FindAsync(id, cancellationToken)
            ?? throw new VmsDomainException("Service Booking not found.");

        if (serviceBooking.Status != ServiceBookingStatus.Assign && serviceBooking.Status != ServiceBookingStatus.Book)
            throw new VmsDomainException("Service Booking is not in Assign or Book status.");

        serviceBooking.Assign(command.SupplierCode);
        //ServiceBooking = new(await DbContext.ServiceBookings.FindAsync(id, cancellationToken)
        //    ?? throw new VmsDomainException("Service Booking not found."), this);

        //ServiceBooking.Assign(supplierCode);
    }

    //class ServiceBookingRole(ServiceBooking self, AssignSupplierUseCase context)
    //{
    //    public void Assign(string supplierCode)
    //    {
    //        self.Assign(supplierCode);
    //    }
    //}
}