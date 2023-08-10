namespace Vms.Application.Services;

public interface IActivityLogger
{
    Task AddAsync(Guid documentId, StringBuilder log, CancellationToken cancellationToken);
    Task AddAsync(Guid documentId, StringBuilder log, DateTime taskTime, CancellationToken cancellationToken);
}

public class ActivityLogger(VmsDbContext dbContext,
    IUserProvider userProvider,
    IEmailSender emailSender,
    INotifyFollowers notifyFollowers,
    ILogger<ActivityLogger> logger) : IActivityLogger
{
    public async Task AddAsync(Guid documentId, StringBuilder log, DateTime taskTime, CancellationToken cancellationToken)
    {
        await NotifyFollowers(documentId, log, cancellationToken);
        LogActivity(documentId, log, taskTime);
    }
    public Task AddAsync(Guid documentId, StringBuilder log, CancellationToken cancellationToken)
        => AddAsync(documentId, log, DateTime.Now, cancellationToken);

    private void LogActivity(Guid documentId, StringBuilder log, DateTime taskTime)
    {
        dbContext.ActivityLog.Add(new ActivityLog(documentId, log.ToString(), userProvider.UserId, userProvider.UserName, taskTime));
    }

    private async Task NotifyFollowers(Guid documentId, StringBuilder log, CancellationToken cancellationToken)
    {
        var followers = await (from f in dbContext.Followers
                         join u in dbContext.Users on f.UserId equals u.UserId
                         where f.DocumentId == documentId
                         select new { f.UserId, u.EmailAddress }).ToListAsync(cancellationToken);

        // TODO dont notify current user

        if (followers.Count == 0)
            return;

        logger.LogDebug("Notifying followers {@followers},", followers);

        // send email
        emailSender.Send(followers.Select(f=>f.EmailAddress), "Activity", log.ToString());

        // send notification (ie via SignalR)
        await notifyFollowers
            .NotifyAsync(followers.Select(f => f.UserId));
    }
}