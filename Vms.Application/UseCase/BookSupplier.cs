using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NetTopologySuite.Utilities;
using Vms.DomainApplication.Services;

namespace Vms.Application.UseCase;

public class BookSupplier(VmsDbContext context, IEmailSender emailSender)
{
    readonly VmsDbContext DBContext = context ?? throw new ArgumentNullException(nameof(context));
    readonly IEmailSender EmailSender = emailSender ?? throw new ArgumentNullException(nameof(emailSender));

    ServiceBookingRole? ServiceBooking;

    public async Task BookAsync(Guid id, DateOnly bookedDate, CancellationToken cancellationToken)
    {
        ServiceBooking = await Load(id, cancellationToken);
        await ServiceBooking.BookAsync(bookedDate, cancellationToken);
    }

    public async Task RefuseAsync(Guid id, string code, CancellationToken cancellationToken)
    {
        ServiceBooking = await Load(id, cancellationToken);
        await ServiceBooking.Refuse(code, cancellationToken);
    }

    public async Task RescheduleAsync(Guid id, DateTime rescheduleTime, CancellationToken cancellationToken)
    {
        ServiceBooking = await Load(id, cancellationToken);
        ServiceBooking.Reschedule(rescheduleTime);
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
            var rr = await context.DBContext.RefusalReasons.FindAsync(new[] { self.CompanyCode, code }, cancellationToken);
            if (rr is null)
            {
                throw new InvalidOperationException("Failed to load refusal reason.");
            }

            //self.Supplier.RefusalReasonCode = rr.Code;
            //self.Supplier.RefusalReasonName = rr.Name;
            self.RescheduleTime = null;
            self.SupplierCode = null;
            self.Status = ServiceBookingStatus.Assign;
        }

        public void Reschedule(DateTime rescheduleTime)
        {
            self.RescheduleTime = rescheduleTime;
        }
    }
}
