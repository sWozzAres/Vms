using Microsoft.Extensions.Logging;
using Vms.Domain.Entity;
using Vms.Domain.Exceptions;
using Vms.Domain.Infrastructure;

public class CreateServiceBooking
{
    readonly VmsDbContext DbContext;
    readonly ILogger<CreateServiceBooking> Logger;
    VehicleRole? Vehicle;
    ServiceBooking? Booking;

    public CreateServiceBooking(VmsDbContext context, ILogger<CreateServiceBooking> logger)
        => (DbContext, Logger) = (context, logger);

    public async Task<CreateBookingResponse> CreateAsync(CreateBookingRequest request, CancellationToken cancellationToken = default)
    {
        Vehicle = new(await DbContext.Vehicles.FindAsync(request.VehicleId, cancellationToken)
            ?? throw new VmsDomainException($"Vehicle with id: {request.VehicleId} not found."), this);

        Booking = await Vehicle.CreateBookingAsync(request, cancellationToken);

        await DbContext.SaveChangesAsync(cancellationToken);

        return new(Booking.Id);
    }

    class VehicleRole(Vehicle self, CreateServiceBooking context)
    {
        public async Task<ServiceBooking> CreateBookingAsync(CreateBookingRequest request, CancellationToken cancellationToken)
        {
            var booking = new ServiceBooking(
                self.Id,
                request.PreferredDate1,
                request.PreferredDate2,
                request.PreferredDate3,
                request.IncludeMot ? self.Mot.Due : null);

            await context.DbContext.AddAsync(booking, cancellationToken);

            context.Logger.LogDebug("Created ServiceBooking");

            return booking;
        }
    }
}


public record CreateBookingRequest(
    int VehicleId,
    DateOnly PreferredDate1,
    DateOnly? PreferredDate2,
    DateOnly? PreferredDate3,
    bool IncludeMot
);

public record CreateBookingResponse(int ServiceBookingId);
