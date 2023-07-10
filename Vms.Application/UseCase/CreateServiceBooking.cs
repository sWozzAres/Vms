using Microsoft.Extensions.DependencyInjection;
using Vms.Application.Services;
using Vms.Web.Shared;

namespace Vms.Application.UseCase;

public interface ICreateServiceBooking
{
    Task<ServiceBooking> CreateAsync(CreateServiceBookingCommand request, CancellationToken cancellationToken = default);
}

public class CreateServiceBooking(VmsDbContext context, IAssignSupplierUseCase assignSupplierUseCase) : ICreateServiceBooking
{
    readonly VmsDbContext DbContext = context;
    readonly IAssignSupplierUseCase AssignSupplierUseCase = assignSupplierUseCase;
    VehicleRole? Vehicle;

    public async Task<ServiceBooking> CreateAsync(CreateServiceBookingCommand command, CancellationToken cancellationToken = default)
    {
        Vehicle = new(await DbContext.Vehicles.FindAsync(command.VehicleId, cancellationToken)
            ?? throw new VmsDomainException($"Vehicle not found."), this);

        var serviceBooking = await Vehicle.CreateBooking(command, cancellationToken);

        //await DbContext.SaveChangesAsync(cancellationToken);

        return serviceBooking;
    }

    class VehicleRole(Vehicle self, CreateServiceBooking context)
    {
        public async Task<ServiceBooking> CreateBooking(CreateServiceBookingCommand request, CancellationToken cancellationToken)
        {
            var booking = new ServiceBooking(
                self.CompanyCode,
                self.Id,
                request.PreferredDate1,
                request.PreferredDate2,
                request.PreferredDate3,
                null//request.IncludeMot ? self.Mot.Due : null
            );

            context.DbContext.ServiceBookings.Add(booking);

            if (request.MotId is not null)
            {
                var motEntry = await context.DbContext.MotEvents.SingleAsync(m => m.Id == request.MotId);
                //motEntry.ServiceBookingId = booking.Id;
                motEntry.ServiceBooking = booking;
            }

            if (request.AutoAssign)
            {
                var assigned = await context.AssignSupplierUseCase.Assign(booking.Id, cancellationToken);

                if (assigned)
                {

                }
            }

            //TODO

            return booking;
        }
    }
}

//public record CreateBookingRequest(
//    Guid VehicleId,
//    DateOnly? PreferredDate1,
//    DateOnly? PreferredDate2,
//    DateOnly? PreferredDate3,
//    Guid? MotId,
//    bool AutoAssign
//);