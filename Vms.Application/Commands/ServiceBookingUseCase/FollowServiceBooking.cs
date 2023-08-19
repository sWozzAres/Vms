using Vms.Domain.ServiceBookingProcess;

namespace Vms.Application.Commands.ServiceBookingUseCase;

public class FollowServiceBooking(
    VmsDbContext dbContext,
    IActivityLogger<VmsDbContext> activityLog,
    IUserProvider userProvider,
    ILogger<FollowServiceBooking> logger) : ServiceBookingTaskBase(dbContext, activityLog)
{
    readonly IUserProvider UserProvider = userProvider;
    ServiceBookingRole? ServiceBooking;

    public async Task FollowAsync(Guid serviceBookingId, CancellationToken cancellationToken)
    {
        logger.LogInformation("Following service booking: {servicebookingid}, user {userid}.", serviceBookingId, UserProvider.UserId);

        ServiceBooking = new(await Load(serviceBookingId, cancellationToken), this);

        SummaryText.AppendLine("# Follow");

        ServiceBooking.AddFollower();

        await LogActivity();
    }

    class ServiceBookingRole(ServiceBooking self, FollowServiceBooking ctx) : ServiceBookingRoleBase<FollowServiceBooking>(self, ctx)
    {
        public void AddFollower()
        {
            var f = new Follower(Self.Id, Ctx.UserProvider.UserId);
            Ctx.DbContext.Followers.Add(f);
        }
    }
}
