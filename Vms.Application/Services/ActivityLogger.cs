namespace Vms.Application.Services;

public interface IActivityLogger
{
    Task<ActivityLog> AddAsync(Guid documentId, StringBuilder log, CancellationToken cancellationToken);
    Task<ActivityLog> AddAsync(Guid documentId, StringBuilder log, DateTime taskTime, CancellationToken cancellationToken);
}

public class ActivityLogger(VmsDbContext dbContext,
    IUserProvider userProvider,
    IEmailSender emailSender,
    INotifyFollowers notifyFollowers,
    ILogger<ActivityLogger> logger,
    ITimeService timeService) : IActivityLogger
{
    public async Task<ActivityLog> AddAsync(Guid documentId, StringBuilder log, DateTime taskTime, CancellationToken cancellationToken)
    {
        await NotifyFollowers(documentId, log, cancellationToken);
        return LogActivity(documentId, log, taskTime);
    }
    public Task<ActivityLog> AddAsync(Guid documentId, StringBuilder log, CancellationToken cancellationToken)
        => AddAsync(documentId, log, timeService.GetTime(), cancellationToken);

    private ActivityLog LogActivity(Guid documentId, StringBuilder log, DateTime taskTime)
    {
        var entry = new ActivityLog(documentId, log.ToString(), userProvider.UserId, userProvider.UserName, taskTime);
        dbContext.ActivityLog.Add(entry);
        return entry;
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