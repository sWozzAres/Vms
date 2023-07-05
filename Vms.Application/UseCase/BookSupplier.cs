using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NetTopologySuite.Utilities;

namespace Vms.Application.UseCase;

public class BookSupplier(VmsDbContext context)
{
    readonly VmsDbContext _context = context ?? throw new ArgumentNullException(nameof(context));
    ServiceBookingRole? ServiceBooking;

    public async Task BookAsync(Guid id, DateOnly bookedDate, CancellationToken cancellationToken)
    {
        ServiceBooking = await Load(id, cancellationToken);
        ServiceBooking.Book(bookedDate);
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
        => new(await _context.ServiceBookings.FindAsync(id, cancellationToken)
            ?? throw new InvalidOperationException("Failed to load service booking."), this);

    class ServiceBookingRole(ServiceBooking self, BookSupplier context)
    {
        public void Book(DateOnly bookedDate)
        {
            if (self.SupplierCode is null)
                throw new VmsDomainException("Service Booking is not assigned.");

            self.RescheduleTime = null;
            self.BookedDate = bookedDate;
            self.Status = ServiceBookingStatus.Confirm;
        }

        public async Task Refuse(string code, CancellationToken cancellationToken)
        {
            if (self.SupplierCode is null)
                throw new VmsDomainException("Service Booking is not assigned.");

            // TODO
            var rr = await context._context.RefusalReasons.FindAsync(new[] { self.CompanyCode, code }, cancellationToken);
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
