using Vms.Domain.ServiceBookingProcess;

namespace Vms.Application.Commands.ServiceBookingUseCase;

public interface IUnfollowServiceBooking
{
    Task<bool> UnfollowAsync(Guid serviceBookingId, CancellationToken cancellationToken);
}

public class UnfollowServiceBooking(VmsDbContext dbContext, IUserProvider userProvider, IActivityLogger<VmsDbContext> activityLog,
    ILogger<UnfollowServiceBooking> logger) : IUnfollowServiceBooking
{
    readonly VmsDbContext DbContext = dbContext;
    readonly IUserProvider UserProvider = userProvider;
    readonly StringBuilder SummaryText = new();

    ServiceBookingRole? ServiceBooking;
    Guid Id;
    CancellationToken CancellationToken;

    public async Task<bool> UnfollowAsync(Guid serviceBookingId, CancellationToken cancellationToken)
    {
        logger.LogInformation("Unfollowing service booking: {servicebookingid}, user {userid}.", serviceBookingId, UserProvider.UserId);
        Id = serviceBookingId;
        CancellationToken = cancellationToken;

        ServiceBooking = new(await DbContext.ServiceBookings.FindAsync(new object[] { Id }, CancellationToken)
            ?? throw new InvalidOperationException("Failed to load service booking."), this);

        //ServiceBooking = new(this);

        SummaryText.AppendLine("# Unfollow");

        bool removed = await ServiceBooking.RemoveFollower();

        if (removed)
        {
            _ = await activityLog.AddAsync(serviceBookingId, nameof(Domain.ServiceBookingProcess.ServiceBooking), ServiceBooking.Entity.Ref,
                SummaryText, CancellationToken);
        }

        return removed;
    }

    class ServiceBookingRole(ServiceBooking self, UnfollowServiceBooking ctx)
    {
        public ServiceBooking Entity => self;
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
