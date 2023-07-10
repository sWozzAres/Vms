using Vms.DomainApplication.Services;
using Vms.Web.Shared;

namespace Vms.Application.UseCase;

public interface IBookSupplier
{
    Task BookAsync(Guid id, TaskBookSupplierCommand request, CancellationToken cancellationToken);
}

public class BookSupplier(VmsDbContext context, IEmailSender emailSender) : IBookSupplier
{
    readonly VmsDbContext DBContext = context ?? throw new ArgumentNullException(nameof(context));
    readonly IEmailSender EmailSender = emailSender ?? throw new ArgumentNullException(nameof(emailSender));

    ServiceBookingRole? ServiceBooking;

    public async Task BookAsync(Guid id, TaskBookSupplierCommand command, CancellationToken cancellationToken)
    {
        ServiceBooking = await Load(id, cancellationToken);

        switch (command.Result)
        {
            case TaskBookSupplierCommand.TaskResult.Booked:
                await ServiceBooking.BookAsync(command.BookedDate!.Value, cancellationToken);
                break;
            case TaskBookSupplierCommand.TaskResult.Refused:
                await ServiceBooking.Refuse(command.RefusalReason!, cancellationToken);
                break;
            case TaskBookSupplierCommand.TaskResult.Rescheduled:
                ServiceBooking.Reschedule(Helper.CombineDateAndTime(command.RescheduleDate!.Value, command.RescheduleTime!));
                break;
        }
    }

    async Task<ServiceBookingRole> Load(Guid id, CancellationToken cancellationToken)
        => new(await DBContext.ServiceBookings.FindAsync(id, cancellationToken)
            ?? throw new InvalidOperationException("Failed to load service booking."), this);

    class ServiceBookingRole(ServiceBooking self, BookSupplier context)
    {
        public async Task BookAsync(DateOnly bookedDate, CancellationToken cancellationToken)
        {
            self.Book(bookedDate);

            var supplier = await context.DBContext.Suppliers.FindAsync(self.SupplierCode, cancellationToken)
                ?? throw new VmsDomainException("Failed to load supplier.");

            var drivers = await context.DBContext.DriverVehicles
                .Include(d => d.Driver)
                .Where(d => d.VehicleId == self.VehicleId)
                .Select(dv => dv.Driver)
                .ToListAsync(cancellationToken);

            var recipients = string.Join(";", drivers.Select(d => d.EmailAddress));
            context.EmailSender.Send(recipients, "Your service is booked",
                $"Your service is booked with {supplier.Name} on {bookedDate}.");
        }

        public async Task Refuse(string code, CancellationToken cancellationToken)
        {
            if (self.SupplierCode is null)
                throw new VmsDomainException("Service Booking is not assigned.");

            // TODO
            var rr = await context.DBContext.RefusalReasons.FindAsync(new[] { self.CompanyCode, code }, cancellationToken)
                ?? throw new InvalidOperationException("Failed to load refusal reason.");

            //self.Supplier.RefusalReasonCode = rr.Code;
            //self.Supplier.RefusalReasonName = rr.Name;
            self.Refuse();
        }

        public void Reschedule(DateTime rescheduleTime)
        {
            self.RescheduleTime = rescheduleTime;
        }
    }
}