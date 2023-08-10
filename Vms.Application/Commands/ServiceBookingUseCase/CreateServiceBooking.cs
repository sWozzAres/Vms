using Vms.Domain.ServiceBookingProcess;

namespace Vms.Application.Commands.ServiceBookingUseCase;

public interface ICreateServiceBooking
{
    Task<ServiceBooking> CreateAsync(CreateServiceBookingCommand request, CancellationToken cancellationToken = default);
}

public class CreateServiceBooking(VmsDbContext dbContext,
    IUserProvider userProvider,
    IAutomaticallyAssignSupplier automaticallyAssignSupplier,
    IActivityLogger activityLog,
    ITaskLogger taskLogger,
    ISearchManager searchManager,
    ITimeService timeService,
    ILogger<CreateServiceBooking> logger) : ICreateServiceBooking
{
    readonly VmsDbContext DbContext = dbContext;
    readonly IUserProvider UserProvider = userProvider;
    readonly IAutomaticallyAssignSupplier AutomaticallyAssignSupplier = automaticallyAssignSupplier;
    readonly StringBuilder SummaryText = new();
    VehicleRole? Vehicle;

    public async Task<ServiceBooking> CreateAsync(CreateServiceBookingCommand command, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Creating service booking: {@createservicebookingcommand}.", command);

        Vehicle = new(await DbContext.Vehicles.FindAsync(new object[] { command.VehicleId }, cancellationToken)
            ?? throw new VmsDomainException($"Vehicle not found."), this);

        // remember task time, or the included use case (assign) will end up in the log
        // before the creation of the booking 
        var taskTime = timeService.GetTime();

        SummaryText.AppendLine("# Create Service Booking");

        var serviceBooking = await Vehicle.CreateBooking(command, cancellationToken);

        logger.LogInformation("Created service booking: {servicebookingid}.", serviceBooking.Id);

        searchManager.Add(serviceBooking.CompanyCode, serviceBooking.Id.ToString(), EntityKind.ServiceBooking, serviceBooking.Ref,
            string.Join(" ", Vehicle.Vrm, serviceBooking.Ref));

        _ = await activityLog.AddAsync(serviceBooking.Id, SummaryText, taskTime, cancellationToken);
        taskLogger.Log(serviceBooking.Id, "Create Service Booking", command);

        return serviceBooking;
    }

    class VehicleRole(Vehicle self, CreateServiceBooking ctx)
    {
        public string Vrm => self.Vrm;

        public async Task<ServiceBooking> CreateBooking(CreateServiceBookingCommand request, CancellationToken cancellationToken)
        {
            // load default driver
            var driver = await (from dv in ctx.DbContext.DriverVehicles
                                where dv.VehicleId == self.Id
                                select dv.Driver).FirstOrDefaultAsync(cancellationToken);

            // create the booking
            var booking = new ServiceBooking(
                self.CompanyCode,
                self.Id,
                request.PreferredDate1,
                request.PreferredDate2,
                request.PreferredDate3,
                null,
                ctx.UserProvider.UserId
            );

            if (driver is not null)
            {
                booking.SetDriver(string.Join(" ", driver.FirstName, driver.LastName), driver.EmailAddress, driver.MobileNumber);
            }

            ctx.DbContext.ServiceBookings.Add(booking);

            // add Mot
            if (request.MotId is not null)
            {
                var motEntry = await ctx.DbContext.MotEvents.SingleAsync(m => m.Id == request.MotId, cancellationToken);
                //motEntry.ServiceBookingId = booking.Id;
                motEntry.ServiceBooking = booking;
            }

            // auto assign
            if (request.AutoAssign)
            {
                booking.ChangeStatus(ServiceBookingStatus.Assign, DateTime.Now);
                var assigned = await ctx.AutomaticallyAssignSupplier.Assign(booking.Id, cancellationToken);

                if (assigned)
                {
                    ctx.SummaryText.AppendLine("\r\n**Supplier was automatically assigned.**");
                }
            }

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