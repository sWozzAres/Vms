namespace Vms.Application.Commands.VehicleUseCase;

public interface IUnfollowVehicle
{
    Task<bool> UnfollowAsync(Guid id, CancellationToken cancellationToken);
}

public class UnfollowVehicle(
    VmsDbContext dbContext,
    IUserProvider userProvider,
    IActivityLogger<VmsDbContext> activityLog) : VehicleTaskBase(dbContext, activityLog)
{
    readonly IUserProvider UserProvider = userProvider;

    VehicleRole? Vehicle;

    public async Task<bool> UnfollowAsync(Guid vehicleId, CancellationToken cancellationToken)
    {
        Vehicle = new(await Load(vehicleId, cancellationToken), this);

        SummaryText.AppendLine("# Unfollow");

        bool removed = await Vehicle.RemoveFollower();

        if (removed)
            await LogActivity();

        return removed;
    }

    class VehicleRole(Vehicle self, UnfollowVehicle ctx) : VehicleRoleBase<UnfollowVehicle>(self, ctx)
    {
        public async Task<bool> RemoveFollower()
        {
            var follow = await Ctx.DbContext.Followers
                .SingleOrDefaultAsync(f => f.UserId == Ctx.UserProvider.UserId && f.DocumentId == Ctx.Id, Ctx.CancellationToken);

            if (follow is not null)
            {
                Ctx.DbContext.Followers.Remove(follow);
                return true;
            }

            return false;
        }
    }
}
