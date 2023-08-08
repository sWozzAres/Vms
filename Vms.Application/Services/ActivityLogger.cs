namespace Vms.Application.Services;

public interface IActivityLogger
{
    Task AddAsync(Guid id, StringBuilder log, CancellationToken cancellationToken);
    Task AddAsync(Guid id, StringBuilder log, DateTime taskTime, CancellationToken cancellationToken);
}

public class ActivityLogger(VmsDbContext dbContext, IUserProvider userProvider, IEmailSender emailSender) : IActivityLogger
{
    public async Task AddAsync(Guid id, StringBuilder log, DateTime taskTime, CancellationToken cancellationToken)
    {
        await NotifyFollowers(id, log, cancellationToken);
        LogActivity(id, log, taskTime);
    }
    public async Task AddAsync(Guid id, StringBuilder log, CancellationToken cancellationToken)
    {
        await NotifyFollowers(id, log, cancellationToken);
        LogActivity(id, log, DateTime.Now);
    }

    private void LogActivity(Guid id, StringBuilder log, DateTime taskTime)
    {
        dbContext.ActivityLog.Add(new ActivityLog(id, log.ToString(), userProvider.UserId, userProvider.UserName, taskTime));
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