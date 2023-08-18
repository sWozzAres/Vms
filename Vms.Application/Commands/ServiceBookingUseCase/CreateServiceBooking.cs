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
    readonly ITimeService TimeService = timeService;
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
        var taskTime = TimeService.Now();

        // create the service booking
        var serviceBooking = await Vehicle.CreateServiceBooking();

        // log activity and task
        _ = await activityLog.AddAsync(serviceBooking.Entity.Id, SummaryText, taskTime, cancellationToken);
        taskLogger.Log(serviceBooking.Entity.Id, nameof(CreateServiceBooking), Command);

        return serviceBooking.Entity;
    }

    class ServiceBookingRole
    {
        readonly ServiceBooking self;
        readonly CreateServiceBooking ctx;

        public ServiceBooking Entity => self;

        public ServiceBookingRole(ServiceBooking _self, CreateServiceBooking _ctx)
        {
            self = _self;
            ctx = _ctx;
            
            ctx.DbContext.ServiceBookings.Add(self);

            if (self.IsValid)
                self.ChangeStatus(ServiceBookingStatus.Assign, ctx.TimeService.Now());

            // update search
            ctx.SearchManager.Add(self.CompanyCode, self.Id.ToString(), EntityKind.ServiceBooking, self.Ref,
               string.Join(" ", self.Vehicle.Vrm, self.Ref));

            ctx.Logger.LogInformation("Created service booking: {servicebookingid}.", self.Id);
        }

        public async Task AutoAssign()
        {
            self.ChangeStatus(ServiceBookingStatus.Assign, ctx.TimeService.Now());

            ctx.DbContext.ThrowIfNoTransaction();
            if (await ctx.AutomaticallyAssignSupplier.AutoAssign(self.Id, ctx.CancellationToken))
            {
                ctx.SummaryText.AppendLine("\r\n**Supplier was automatically assigned.**");
            }
        }
    }

    class VehicleRole(Vehicle self, CreateServiceBooking ctx)
    {
        public async Task<ServiceBookingRole> CreateServiceBooking()
        {
            ctx.SummaryText.AppendLine("# Create Service Booking");

            var motEvent = ctx.Command.MotId is null
                 ? null
                 : await ctx.DbContext.MotEvents.FindAsync(new object[] { ctx.Command.MotId }, ctx.CancellationToken)
                    ?? throw new VmsDomainException("Failed to find Mot Event.");

            // create the booking
            var serviceBooking = new ServiceBookingRole(new ServiceBooking(
                self.CompanyCode,
                self.Id,
                ctx.Command.PreferredDate1,
                ctx.Command.PreferredDate2,
                ctx.Command.PreferredDate3,
                (ServiceLevel)ctx.Command.ServiceLevel,
                ctx.UserProvider.UserId,
                await GetFirstDriver(),
                motEvent
            ), ctx);

            // auto assign
            if (ctx.Command.AutoAssign)
                await serviceBooking.AutoAssign();

            return serviceBooking;
        }

        async Task<Driver?> GetFirstDriver()
            => await (from dv in ctx.DbContext.DriverVehicles
                      where dv.VehicleId == self.Id
                      select dv.Driver).FirstOrDefaultAsync(ctx.CancellationToken);
    }
}