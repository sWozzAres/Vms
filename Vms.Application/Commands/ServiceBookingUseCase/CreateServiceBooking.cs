using Vms.Domain.ServiceBookingProcess;

namespace Vms.Application.Commands.ServiceBookingUseCase;

public interface ICreateServiceBooking
{
    Task<ServiceBooking> CreateAsync(CreateServiceBookingCommand request, CancellationToken cancellationToken = default);
}

public class CreateServiceBooking(VmsDbContext dbContext,
    IUserProvider userProvider,
    IAutomaticallyAssignSupplier automaticallyAssignSupplier,
    IActivityLogger<VmsDbContext> activityLog,
    ITaskLogger<VmsDbContext> taskLogger,
    ISearchManager searchManager,
    ITimeService timeService,
    ILogger<CreateServiceBooking> logger) : ICreateServiceBooking
{
    readonly VmsDbContext DbContext = dbContext;
    readonly IUserProvider UserProvider = userProvider;
    readonly IAutomaticallyAssignSupplier AutomaticallyAssignSupplier = automaticallyAssignSupplier;
    readonly StringBuilder SummaryText = new();
    readonly ILogger<CreateServiceBooking> Logger = logger;
    readonly ISearchManager SearchManager = searchManager;
    VehicleRole? Vehicle;
    CreateServiceBookingCommand Command = null!;
    CancellationToken CancellationToken;

    public async Task<ServiceBooking> CreateAsync(CreateServiceBookingCommand command, CancellationToken cancellationToken = default)
    {
        Command = command;
        CancellationToken = cancellationToken;

        Logger.LogInformation("Creating service booking: {@createservicebookingcommand}.", Command);

        // load vehicle
        Vehicle = new(await DbContext.Vehicles.FindAsync(new object[] { Command.VehicleId }, cancellationToken)
            ?? throw new VmsDomainException($"Vehicle not found."), this);

        // remember task time, or the included use case (assign) will end up in the log
        // before the creation of the booking 
        var taskTime = timeService.GetTime();

        // create the service booking
        var serviceBooking = await Vehicle.CreateServiceBooking();

        // log activity and task
        _ = await activityLog.AddAsync(serviceBooking.Id, SummaryText, taskTime, cancellationToken);
        taskLogger.Log(serviceBooking.Id, nameof(CreateServiceBooking), Command);

        return serviceBooking;
    }

    class VehicleRole(Vehicle self, CreateServiceBooking ctx)
    {
        public async Task<ServiceBooking> CreateServiceBooking()
        {
            ctx.SummaryText.AppendLine("# Create Service Booking");

            // create the booking
            var booking = new ServiceBooking(
                self.CompanyCode,
                self.Id,
                ctx.Command.PreferredDate1,
                ctx.Command.PreferredDate2,
                ctx.Command.PreferredDate3,
                (ServiceLevel)ctx.Command.ServiceLevel,
                ctx.UserProvider.UserId
            );

            // load default driver
            var driver = await (from dv in ctx.DbContext.DriverVehicles
                                where dv.VehicleId == self.Id
                                select dv.Driver).FirstOrDefaultAsync(ctx.CancellationToken);
            if (driver is not null)
            {
                booking.SetDriver(string.Join(" ", driver.FirstName, driver.LastName), driver.EmailAddress, driver.MobileNumber);
            }

            ctx.DbContext.ServiceBookings.Add(booking);

            // add Mot
            if (ctx.Command.MotId is not null)
            {
                var motEntry = await ctx.DbContext.MotEvents.SingleOrDefaultAsync(m => m.Id == ctx.Command.MotId, ctx.CancellationToken)
                    ?? throw new VmsDomainException("Failed to find Mot.");

                motEntry.ServiceBookingId = booking.Id;
            }

            // auto assign
            if (ctx.Command.AutoAssign)
            {
                booking.ChangeStatus(ServiceBookingStatus.Assign, DateTime.Now);

                ctx.DbContext.ThrowIfNoTransaction();
                var assigned = await ctx.AutomaticallyAssignSupplier
                    .Assign(booking.Id, ctx.CancellationToken);

                if (assigned)
                {
                    ctx.SummaryText.AppendLine("\r\n**Supplier was automatically assigned.**");
                }
            }

            ctx.Logger.LogInformation("Created service booking: {servicebookingid}.", booking.Id);

            // update search
            ctx.SearchManager.Add(booking.CompanyCode, booking.Id.ToString(), EntityKind.ServiceBooking, booking.Ref,
                string.Join(" ", self.Vrm, booking.Ref));

            return booking;
        }
    }
}