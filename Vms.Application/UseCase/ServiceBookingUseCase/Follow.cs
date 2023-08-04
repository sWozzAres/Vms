using Vms.Domain.Entity.ServiceBookingEntity;
using Vms.Domain.Services;

namespace Vms.Application.UseCase.ServiceBookingUseCase;

public interface IFollow
{
    Task FollowAsync(Guid id, CancellationToken cancellationToken);
}

public class Follow(VmsDbContext dbContext, IUserProvider userProvider) : IFollow
{
    readonly VmsDbContext DbContext = dbContext;
    readonly IUserProvider UserProvider = userProvider ?? throw new ArgumentNullException(nameof(userProvider));
    ServiceBookingRole? ServiceBooking;

    public async Task FollowAsync(Guid id, CancellationToken cancellationToken)
    {
        ServiceBooking = new(await DbContext.ServiceBookings.FindAsync(new object[] { id }, cancellationToken)
            ?? throw new InvalidOperationException("Failed to load service booking."), this);

        ServiceBooking.AddFollower(UserProvider);
    }

    class ServiceBookingRole(ServiceBooking self, Follow ctx)
    {
        public void AddFollower(IUserProvider userProvider)
        {
            var f = new Follower(self.Id, userProvider.UserId, userProvider.EmailAddress);
            ctx.DbContext.Followers.Add(f);
        }
    }
}
