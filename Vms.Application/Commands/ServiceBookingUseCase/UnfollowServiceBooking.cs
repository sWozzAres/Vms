using Microsoft.Extensions.Logging;

namespace Vms.Application.Commands.ServiceBookingUseCase;

public interface IUnfollowServiceBooking
{
    Task<bool> UnfollowAsync(Guid id, CancellationToken cancellationToken);
}

public class UnfollowServiceBooking(VmsDbContext dbContext, IUserProvider userProvider, IActivityLogger activityLog,
    ILogger<UnfollowServiceBooking> logger) : IUnfollowServiceBooking
{
    readonly VmsDbContext DbContext = dbContext;
    readonly IUserProvider UserProvider = userProvider;
    readonly StringBuilder SummaryText = new();

    ServiceBookingRole? ServiceBooking;
    Guid Id;
    CancellationToken CancellationToken;

    public async Task<bool> UnfollowAsync(Guid id, CancellationToken cancellationToken)
    {
        logger.LogInformation("UnfollowServiceBooking task for service booking {servicebookingid}, user {userid}.", id, UserProvider.UserId);
        Id = id;
        CancellationToken = cancellationToken;

        //
        // dont need to load service booking
        //
        //ServiceBooking = new(await DbContext.ServiceBookings.FindAsync(new object[] { Id }, CancellationToken)
        //    ?? throw new InvalidOperationException("Failed to load service booking."), this);

        ServiceBooking = new(this);

        SummaryText.AppendLine("# Unfollow");

        bool removed = await ServiceBooking.RemoveFollower();

        if (removed)
            await activityLog.AddAsync(Id, SummaryText, CancellationToken);

        return removed;
    }

    class ServiceBookingRole(UnfollowServiceBooking ctx)
    {
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
