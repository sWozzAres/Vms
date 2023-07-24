using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Vms.Application.Services;
using Vms.Domain.Entity.ServiceBookingEntity;
using Vms.Domain.Services;
using Vms.Web.Shared;

namespace Vms.Application.UseCase.ServiceBookingUseCase;

public interface ICreateServiceBooking
{
    Task<ServiceBooking> CreateAsync(CreateServiceBookingCommand request, CancellationToken cancellationToken = default);
}

public class CreateServiceBooking(VmsDbContext context, IAutomaticallyAssignSupplierUseCase assignSupplierUseCase, IUserProvider userProvider) : ICreateServiceBooking
{
    readonly VmsDbContext DbContext = context;
    readonly IAutomaticallyAssignSupplierUseCase AssignSupplierUseCase = assignSupplierUseCase;
    readonly IUserProvider UserProvider = userProvider;
    readonly StringBuilder SummaryText = new(); 
    VehicleRole? Vehicle;

    public async Task<ServiceBooking> CreateAsync(CreateServiceBookingCommand command, CancellationToken cancellationToken = default)
    {
        Vehicle = new(await DbContext.Vehicles.FindAsync(command.VehicleId, cancellationToken)
            ?? throw new VmsDomainException($"Vehicle not found."), this);

        SummaryText.AppendLine("# Create Service Booking");

        var serviceBooking = await Vehicle.CreateBooking(command, cancellationToken);

        //await DbContext.SaveChangesAsync(cancellationToken);

        DbContext.ActivityLog.Add(new ActivityLog(serviceBooking.Id, SummaryText.ToString(), UserProvider.UserId, UserProvider.UserName));

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