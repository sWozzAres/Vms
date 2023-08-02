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
    readonly IUserProvider UserProvider = userProvider;
    ServiceBookingRole? ServiceBooking;

    public async Task FollowAsync(Guid id, CancellationToken cancellationToken)
    {
        ServiceBooking = new(await DbContext.ServiceBookings.FindAsync(new object[] { id }, cancellationToken)
            ?? throw new InvalidOperationException("Failed to load service booking."), this);

        ServiceBooking.Follow();
    }

    class ServiceBookingRole(ServiceBooking self, Follow ctx)
    {
        public void Follow()
        {
            var f = new Follower()
            {
                UserId = ctx.UserProvider.UserId,
                DocumentId = self.Id,
                EmailAddress = ctx.UserProvider.EmailAddress
            };

            ctx.DbContext.Followers.Add(f);
        }
    }
}
