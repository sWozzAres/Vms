namespace Vms.Application.Commands.VehicleUseCase;

public interface IFollowVehicle
{
    Task FollowAsync(Guid id, CancellationToken cancellationToken);
}

public class FollowVehicle(VmsDbContext dbContext, IUserProvider userProvider, IActivityLogger<VmsDbContext> activityLog) : IFollowVehicle
{
    readonly VmsDbContext DbContext = dbContext;
    readonly IUserProvider UserProvider = userProvider;
    readonly StringBuilder SummaryText = new();

    VehicleRole? Vehicle;

    public async Task FollowAsync(Guid id, CancellationToken cancellationToken)
    {
        Vehicle = new(await DbContext.Vehicles.FindAsync(new object[] { id }, cancellationToken)
            ?? throw new InvalidOperationException("Failed to load vehicle."), this);

        SummaryText.AppendLine("# Follow");

        Vehicle.AddFollower();

        _ = await activityLog.AddAsync(id, nameof(Domain.Core.Vehicle), Vehicle.Entity.Vrm,
            SummaryText, cancellationToken);
    }

    class VehicleRole(Vehicle self, FollowVehicle ctx)
    {
        public Vehicle Entity => self;
        public void AddFollower()
        {
            var f = new Follower(self.Id, ctx.UserProvider.UserId);
            ctx.DbContext.Followers.Add(f);
        }
    }
}
