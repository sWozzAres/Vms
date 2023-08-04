using System.Text;
using Vms.Domain.Entity.ServiceBookingEntity;
using Vms.Domain.Services;
using Vms.DomainApplication.Services;

namespace Vms.Application.Services;

public interface IActivityLogger
{
    Task AddAsync(Guid id, StringBuilder log, CancellationToken cancellationToken);
}

public class ActivityLogger(VmsDbContext dbContext, IUserProvider userProvider, IEmailSender emailSender) : IActivityLogger
{
    public async Task AddAsync(Guid id, StringBuilder log, CancellationToken cancellationToken)
    {
        await NotifyFollowers(id, log, cancellationToken);
        LogActivity(id, log);
    }

    private void LogActivity(Guid id, StringBuilder log)
    {
        dbContext.ActivityLog.Add(new ActivityLog(id, log.ToString(), userProvider.UserId, userProvider.UserName));
    }

    private async Task NotifyFollowers(Guid id, StringBuilder log, CancellationToken cancellationToken)
    {
        foreach (var follower in await GetFollowersForDocument())
        {
            emailSender.Send(follower.EmailAddress, "Activity", log.ToString());
        }

        async Task<List<Follower>> GetFollowersForDocument()
            => await dbContext.Followers.AsNoTracking()
                .Where(f => f.DocumentId == id)
                .ToListAsync(cancellationToken);
    }
}