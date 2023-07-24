using System.Text;
using Vms.Domain.Entity.ServiceBookingEntity;
using Vms.Domain.Services;
using Vms.Web.Shared;

namespace Vms.Application.UseCase.ServiceBookingUseCase;

public interface IAssignSupplierUseCase
{
    Task Assign(Guid id, TaskAssignSupplierCommand command, CancellationToken cancellationToken = default);
}

public class AssignSupplierUseCase(VmsDbContext dbContext, IUserProvider userProvider) : IAssignSupplierUseCase
{
    readonly VmsDbContext DbContext = dbContext;
    readonly StringBuilder SummaryText = new();
    readonly IUserProvider UserProvider = userProvider;

    public async Task Assign(Guid id, TaskAssignSupplierCommand command, CancellationToken cancellationToken = default)
    {
        var serviceBooking = await DbContext.ServiceBookings.FindAsync(id, cancellationToken)
            ?? throw new InvalidOperationException("Service Booking not found.");

        if (serviceBooking.Status != ServiceBookingStatus.Assign && serviceBooking.Status != ServiceBookingStatus.Book)
            throw new VmsDomainException("Service Booking is not in Assign or Book status.");

        SummaryText.AppendLine("# Assign Supplier");

        var supplier = await DbContext.Suppliers.FindAsync(command.SupplierCode, cancellationToken)
            ?? throw new InvalidOperationException("Supplier not found.");

        SummaryText.AppendLine($"* Code: {supplier.Code}");
        SummaryText.AppendLine($"* Name: {supplier.Name}");

        serviceBooking.Assign(command.SupplierCode);

        DbContext.ActivityLog.Add(new ActivityLog(id, SummaryText.ToString(), UserProvider.UserId, UserProvider.UserName));
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