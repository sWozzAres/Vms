using Vms.Application.Commands.ServiceBookingUseCase;
using Vms.Domain.ServiceBookingProcess;

namespace Vms.Application.Commands.VehicleUseCase;

public class CreateServiceBooking(VmsDbContext dbContext,
    IUserProvider userProvider,
    AutomaticallyAssignSupplier automaticallyAssignSupplier,
    IActivityLogger<VmsDbContext> activityLog,
    ITaskLogger<VmsDbContext> taskLogger,
    ISearchManager searchManager,
    ITimeService timeService,
    ILogger<CreateServiceBooking> logger)
{
    readonly VmsDbContext DbContext = dbContext;
    readonly IUserProvider UserProvider = userProvider;
    readonly AutomaticallyAssignSupplier AutomaticallyAssignSupplier = automaticallyAssignSupplier;
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
        var taskTime = TimeService.Now;

        // create the service booking
        var serviceBooking = await Vehicle.CreateServiceBooking();

        // log activity and task
        _ = await activityLog.AddAsync(serviceBooking.Id, nameof(ServiceBooking), serviceBooking.Ref,
            SummaryText, CancellationToken);
        taskLogger.Log(serviceBooking.Id, nameof(CreateServiceBooking), Command);

        Logger.LogInformation("Created service booking: {servicebookingid}.", serviceBooking.Id);

        return serviceBooking;
    }

    class ServiceBookingRole(CreateServiceBooking ctx)
    {
        public async Task<ServiceBooking> Create(VehicleRole vehicle)
        {
            ctx.SummaryText.AppendLine("# Create Service Booking");

            // load the mot event 
            var motEvent = ctx.Command.MotId is null ? null
                 : await ctx.DbContext.MotEvents.FindAsync(new object[] { ctx.Command.MotId }, ctx.CancellationToken)
                    ?? throw new VmsDomainException("Failed to find Mot Event.");

            var self = new ServiceBooking(
                vehicle.Entity.CompanyCode,
                vehicle.Entity.Id,
                ctx.Command.PreferredDate1,
                ctx.Command.PreferredDate2,
                ctx.Command.PreferredDate3,
                (ServiceLevel)ctx.Command.ServiceLevel,
                ctx.UserProvider.UserId,
                await vehicle.GetFirstDriver(),
                motEvent
            );

            ctx.DbContext.ServiceBookings.Add(self);

            if (self.IsReady)
                self.ChangeStatus(ServiceBookingStatus.Assign, ctx.TimeService.Now);

            SummarizeInActivityLog();

            // auto assign
            if (ctx.Command.AutoAssign)
                await AutoAssign();

            // update search
            ctx.SearchManager.Add(self.CompanyCode, self.Id.ToString(), EntityKind.ServiceBooking, self.Ref,
               string.Join(" ", self.Vehicle.Vrm, self.Ref));

            return self;

            async Task AutoAssign()
            {
                self.ChangeStatus(ServiceBookingStatus.Assign, ctx.TimeService.Now);

                ctx.DbContext.ThrowIfNoTransaction();
                if (await ctx.AutomaticallyAssignSupplier.AutoAssign(self.Id, ctx.CancellationToken))
                {
                    ctx.SummaryText.AppendLine("\r\n**Supplier was automatically assigned.**");
                }
            }

            void SummarizeInActivityLog()
            {
                ctx.SummaryText.AppendLine($"* Service Level: {self.ServiceLevel.ToDisplayString()}");
                if (self.PreferredDate1.HasValue)
                    ctx.SummaryText.AppendLine($"* Preferred Date 1: {self.PreferredDate1}");
                if (self.PreferredDate2.HasValue)
                    ctx.SummaryText.AppendLine($"* Preferred Date 2: {self.PreferredDate2}");
                if (self.PreferredDate3.HasValue)
                    ctx.SummaryText.AppendLine($"* Preferred Date 3: {self.PreferredDate3}");
                if (!string.IsNullOrEmpty(self.Driver.Name))
                    ctx.SummaryText.AppendLine($"* Driver Name: {self.Driver.Name}");
                if (!string.IsNullOrEmpty(self.Driver.EmailAddress))
                    ctx.SummaryText.AppendLine($"* Driver Email: [{self.Driver.EmailAddress}](mailto://{self.Driver.EmailAddress})");
                if (!string.IsNullOrEmpty(self.Driver.MobileNumber))
                    ctx.SummaryText.AppendLine($"* Driver Mobile: {self.Driver.MobileNumber}");

                if (self.MotEvent is not null)
                    ctx.SummaryText.AppendLine($"* MOT Due: {self.MotEvent.Due}");
            }
        }
    }

    class VehicleRole(Vehicle self, CreateServiceBooking ctx)
    {
        public Vehicle Entity => self;
        public async Task<ServiceBooking> CreateServiceBooking()
            => await new ServiceBookingRole(ctx).Create(this);

        public async Task<Driver?> GetFirstDriver()
            => await (from dv in ctx.DbContext.DriverVehicles
                      where dv.VehicleId == self.Id
                      select dv.Driver).FirstOrDefaultAsync(ctx.CancellationToken);
    }
}