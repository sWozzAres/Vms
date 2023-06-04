using Microsoft.Extensions.Logging;
using Vms.Domain.Entity;
using Vms.Domain.Exceptions;
using Vms.Domain.Infrastructure;

public class CreateServiceBooking
{
    readonly VmsDbContext DbContext;
    readonly ILogger<CreateServiceBooking> Logger;
    VehicleRole? Vehicle;
    //ServiceBooking? Booking;

    public CreateServiceBooking(VmsDbContext context, ILogger<CreateServiceBooking> logger)
        => (DbContext, Logger) = (context, logger);

    public async Task<ServiceBooking> CreateAsync(CreateBookingRequest request, CancellationToken cancellationToken = default)
    {
        Vehicle = new(await DbContext.Vehicles.FindAsync(request.VehicleId, cancellationToken)
            ?? throw new VmsDomainException($"Vehicle with id '{request.VehicleId}' not found."), this);

        return Vehicle.CreateBooking(request);
    }

    class VehicleRole(Vehicle self, CreateServiceBooking context)
    {
        public ServiceBooking CreateBooking(CreateBookingRequest request)
        {
            var booking = new ServiceBooking(
                self.Id,
                request.PreferredDate1,
                request.PreferredDate2,
                request.PreferredDate3,
                request.IncludeMot ? self.Mot.Due : null,
                self.HomeLocation);

            context.DbContext.ServiceBookings.Add(booking);

            context.Logger.LogDebug("Created ServiceBooking");

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