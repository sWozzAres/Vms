using System.Text;
using Vms.Application.Services;
using Vms.Domain.Entity.ServiceBookingEntity;
using Vms.Web.Shared;

namespace Vms.Application.UseCase.ServiceBookingUseCase;

public interface IAssignSupplierUseCase
{
    Task AssignAsync(Guid id, TaskAssignSupplierCommand command, CancellationToken cancellationToken = default);
}

public class AssignSupplierUseCase(VmsDbContext dbContext, IActivityLogger activityLog, ITaskLogger taskLogger) : IAssignSupplierUseCase
{
    readonly VmsDbContext DbContext = dbContext;
    readonly IActivityLogger ActivityLog = activityLog;
    readonly ITaskLogger TaskLogger = taskLogger;
    readonly StringBuilder SummaryText = new();

    public async Task AssignAsync(Guid id, TaskAssignSupplierCommand command, CancellationToken cancellationToken = default)
    {
        var serviceBooking = await DbContext.ServiceBookings.FindAsync(new object[] { id }, cancellationToken)
            ?? throw new InvalidOperationException("Service Booking not found.");

        if (serviceBooking.Status != ServiceBookingStatus.Assign && serviceBooking.Status != ServiceBookingStatus.Book)
            throw new VmsDomainException("Service Booking is not in Assign or Book status.");

        SummaryText.AppendLine("# Assign Supplier");

        var supplier = await DbContext.Suppliers.FindAsync(new object[] { command.SupplierCode }, cancellationToken)
            ?? throw new InvalidOperationException("Supplier not found.");

        SummaryText.AppendLine($"* Code: {supplier.Code}");
        SummaryText.AppendLine($"* Name: {supplier.Name}");

        serviceBooking.Assign(command.SupplierCode);

        await ActivityLog.LogAsync(id, SummaryText, cancellationToken);
        TaskLogger.Log(id, "Assign Supplier", command);
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