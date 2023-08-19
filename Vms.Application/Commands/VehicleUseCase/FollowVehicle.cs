namespace Vms.Application.Commands.VehicleUseCase;

public class FollowVehicle(
    VmsDbContext dbContext,
    IActivityLogger<VmsDbContext> activityLog,
    IUserProvider userProvider) : VehicleTaskBase(dbContext, activityLog)
{
    readonly IUserProvider UserProvider = userProvider;
    VehicleRole? Vehicle;

    public async Task FollowAsync(Guid vehicleId, CancellationToken cancellationToken)
    {
        Vehicle = new(await Load(vehicleId, cancellationToken), this);

        SummaryText.AppendLine("# Follow");

        Vehicle.AddFollower();

        await LogActivity();
    }

    class VehicleRole(Vehicle self, FollowVehicle ctx) : VehicleRoleBase<FollowVehicle>(self, ctx)
    {
        public void AddFollower()
        {
            var f = new Follower(Self.Id, Ctx.UserProvider.UserId);
            Ctx.DbContext.Followers.Add(f);
        }
    }
}
