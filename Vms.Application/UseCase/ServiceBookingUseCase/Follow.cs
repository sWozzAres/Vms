using System.Text;
using Vms.Application.Services;
using Vms.Domain.Entity.ServiceBookingEntity;
using Vms.Domain.Services;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Vms.Application.UseCase.ServiceBookingUseCase;

public interface IFollow
{
    Task FollowAsync(Guid id, CancellationToken cancellationToken);
}

public class Follow(VmsDbContext dbContext, IUserProvider userProvider, IActivityLogger activityLog) : IFollow
{
    readonly VmsDbContext DbContext = dbContext;
    readonly IUserProvider UserProvider = userProvider;
    readonly StringBuilder SummaryText = new();

    ServiceBookingRole? ServiceBooking;

    public async Task FollowAsync(Guid id, CancellationToken cancellationToken)
    {
        ServiceBooking = new(await DbContext.ServiceBookings.FindAsync(new object[] { id }, cancellationToken)
            ?? throw new InvalidOperationException("Failed to load service booking."), this);

        SummaryText.AppendLine("# Follow");

        ServiceBooking.AddFollower();

        await activityLog.AddAsync(id, SummaryText, cancellationToken);
    }

    class ServiceBookingRole(ServiceBooking self, Follow ctx)
    {
        public void AddFollower()
        {
            var f = new Follower(self.Id, ctx.UserProvider.UserId, ctx.UserProvider.EmailAddress);
            ctx.DbContext.Followers.Add(f);
        }
    }
}
