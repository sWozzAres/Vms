using Vms.Domain.ServiceBookingProcess;

namespace Vms.Application.Commands.ServiceBookingUseCase;

public class UnfollowServiceBooking(
    VmsDbContext dbContext,
    IActivityLogger<VmsDbContext> activityLog,
    IUserProvider userProvider,
    ILogger<UnfollowServiceBooking> logger) : ServiceBookingTaskBase(dbContext, activityLog)
{
    readonly IUserProvider UserProvider = userProvider;
    ServiceBookingRole? ServiceBooking;

    public async Task<bool> UnfollowAsync(Guid serviceBookingId, CancellationToken cancellationToken)
    {
        logger.LogInformation("Unfollowing service booking: {servicebookingid}, user {userid}.", serviceBookingId, UserProvider.UserId);

        ServiceBooking = new(await Load(serviceBookingId, cancellationToken), this);

        SummaryText.AppendLine("# Unfollow");

        bool removed = await ServiceBooking.RemoveFollower();

        if (removed)
            await LogActivity();

        return removed;
    }

    class ServiceBookingRole(ServiceBooking self, UnfollowServiceBooking ctx) : ServiceBookingRoleBase<UnfollowServiceBooking>(self, ctx)
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
