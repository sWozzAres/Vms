using Vms.Domain.ServiceBookingProcess;

namespace Vms.Application.Commands.ServiceBookingUseCase;

public interface IFollowServiceBooking
{
    Task FollowAsync(Guid id, CancellationToken cancellationToken);
}

public class FollowServiceBooking(VmsDbContext dbContext, IUserProvider userProvider, IActivityLogger activityLog,
    ILogger<FollowServiceBooking> logger) : IFollowServiceBooking
{
    readonly VmsDbContext DbContext = dbContext;
    readonly IUserProvider UserProvider = userProvider;
    readonly StringBuilder SummaryText = new();

    ServiceBookingRole? ServiceBooking;

    public async Task FollowAsync(Guid id, CancellationToken cancellationToken)
    {
        logger.LogInformation("Following service booking: {servicebookingid}, user {userid}.", id, UserProvider.UserId);

        ServiceBooking = new(await DbContext.ServiceBookings.FindAsync(new object[] { id }, cancellationToken)
            ?? throw new InvalidOperationException("Failed to load service booking."), this);

        SummaryText.AppendLine("# Follow");

        ServiceBooking.AddFollower();

        await activityLog.AddAsync(id, SummaryText, cancellationToken);
    }

    class ServiceBookingRole(ServiceBooking self, FollowServiceBooking ctx)
    {
        public void AddFollower()
        {
            var f = new Follower(self.Id, ctx.UserProvider.UserId);
            ctx.DbContext.Followers.Add(f);
        }
    }
}
