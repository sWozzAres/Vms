using System.Text;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using Vms.Application.Services;
using Vms.Domain.Entity.ServiceBookingEntity;
using Vms.Domain.Services;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Vms.Application.UseCase.ServiceBookingUseCase;

public interface IUnfollow
{
    Task<bool> UnfollowAsync(Guid id, CancellationToken cancellationToken);
}

public class Unfollow(VmsDbContext dbContext, IUserProvider userProvider, IActivityLogger activityLog) : IUnfollow
{
    readonly VmsDbContext DbContext = dbContext;
    readonly IUserProvider UserProvider = userProvider;
    readonly StringBuilder SummaryText = new();

    ServiceBookingRole? ServiceBooking;
    Guid Id;
    CancellationToken CancellationToken;

    public async Task<bool> UnfollowAsync(Guid id, CancellationToken cancellationToken)
    {
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

    class ServiceBookingRole(Unfollow ctx)
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
