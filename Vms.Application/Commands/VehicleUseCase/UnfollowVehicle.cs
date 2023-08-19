using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;

namespace Vms.Application.Commands.VehicleUseCase;

public interface IUnfollowVehicle
{
    Task<bool> UnfollowAsync(Guid id, CancellationToken cancellationToken);
}

public class UnfollowVehicle(VmsDbContext dbContext, IUserProvider userProvider, IActivityLogger<VmsDbContext> activityLog) : IUnfollowVehicle
{
    readonly VmsDbContext DbContext = dbContext;
    readonly IUserProvider UserProvider = userProvider;
    readonly StringBuilder SummaryText = new();

    VehicleRole? Vehicle;
    Guid Id;
    CancellationToken CancellationToken;

    public async Task<bool> UnfollowAsync(Guid id, CancellationToken cancellationToken)
    {
        Id = id;
        CancellationToken = cancellationToken;

        Vehicle = new(await DbContext.Vehicles.FindAsync(new object[] { Id }, CancellationToken)
            ?? throw new InvalidOperationException("Failed to load service booking."), this);

        //Vehicle = new(this);

        SummaryText.AppendLine("# Unfollow");

        bool removed = await Vehicle.RemoveFollower();

        if (removed)
            _ = await activityLog.AddAsync(Id, nameof(Domain.Core.Vehicle), Vehicle.Entity.Vrm,
                SummaryText, CancellationToken);

        return removed;
    }

    class VehicleRole(Vehicle self, UnfollowVehicle ctx)
    {
        public Vehicle Entity => self;
        public async Task<bool> RemoveFollower()
        {
            var follow = await ctx.DbContext.Followers
                .SingleOrDefaultAsync(f => f.UserId == ctx.UserProvider.UserId && f.DocumentId == ctx.Id, ctx.CancellationToken);

            if (follow is not null)
            {
                ctx.DbContext.Followers.Remove(follow);
                return true;
            }

            return false;
        }
    }
}
