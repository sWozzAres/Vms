using Microsoft.Extensions.Logging;
using Vms.Application.Services;
using Vms.Domain.Entity;
using Vms.Domain.Exceptions;
using Vms.Domain.Infrastructure;

namespace Vms.Application.UseCase;

public class CreateServiceBooking
{
    readonly VmsDbContext DbContext;
    VehicleRole? Vehicle;
    //ServiceBooking? Booking;

    public CreateServiceBooking(VmsDbContext context)
        => (DbContext) = (context);

    public async Task<ServiceBooking> CreateAsync(CreateBookingRequest request, CancellationToken cancellationToken = default)
    {
        Vehicle = new(await DbContext.Vehicles.FindAsync(request.VehicleId, cancellationToken)
            ?? throw new VmsDomainException($"Vehicle with id '{request.VehicleId}' not found."), this);

        return await Vehicle.CreateBooking(request, cancellationToken);
    }

    class VehicleRole(Vehicle self, CreateServiceBooking context)
    {
        public async Task<ServiceBooking> CreateBooking(CreateBookingRequest request, CancellationToken cancellationToken)
        {
            var booking = new ServiceBooking(
                self.Id,
                request.PreferredDate1,
                request.PreferredDate2,
                request.PreferredDate3,
                request.IncludeMot ? self.Mot.Due : null
            );

            context.DbContext.ServiceBookings.Add(booking);

            var assigned = await new AssignSupplier(context.DbContext, new SupplierLocator(context.DbContext))
                .Assign(new(booking.Id), cancellationToken);

            return booking;
        }
    }
}

public record CreateBookingRequest(
    Guid VehicleId,
    DateOnly PreferredDate1,
    DateOnly? PreferredDate2,
    DateOnly? PreferredDate3,
    bool IncludeMot
);