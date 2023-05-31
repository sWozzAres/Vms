using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Vms.Domain.Entity;
using Vms.Domain.Exceptions;
using Vms.Domain.Infrastructure;

namespace Vms.Domain.UseCase;

public class CreateServiceBooking
{
    readonly VmsDbContext DbContext;
    readonly ILogger<CreateServiceBooking> Logger;
    VehicleRole? Vehicle;
    CancellationToken CancellationToken = default;


    public CreateServiceBooking(VmsDbContext context, ILogger<CreateServiceBooking> logger)
        => (DbContext, Logger) = (context, logger);

    public async Task<int> Create(CreateBookingRequest request, CancellationToken cancellationToken = default)
    {
        CancellationToken = cancellationToken;

        Vehicle = new(await DbContext.Vehicles.FindAsync(request.VehicleId, cancellationToken) 
            ?? throw new VmsDomainException($"Vehicle with id: {request.VehicleId} not found."), this);

        var booking = await Vehicle.CreateBookingAsync(request);

        await DbContext.SaveChangesAsync(cancellationToken);

        return booking.Id;
    }

    class VehicleRole(Vehicle self, CreateServiceBooking context)
    {
        public async Task<ServiceBooking> CreateBookingAsync(CreateBookingRequest request)
        {
            var booking = new ServiceBooking(
                self.Id,
                request.PreferredDate1,
                request.PreferredDate2,
                request.PreferredDate3,
                request.IncludeMot ? self.NextMot.Due : null);
            
            await context.DbContext.ServiceBookings.AddAsync(booking, context.CancellationToken);

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