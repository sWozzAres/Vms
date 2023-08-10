namespace Vms.Application.Services;

public interface IActivityLogger
{
    Task AddAsync(Guid id, StringBuilder log, CancellationToken cancellationToken);
    Task AddAsync(Guid id, StringBuilder log, DateTime taskTime, CancellationToken cancellationToken);
}

public class ActivityLogger(VmsDbContext dbContext, 
    IUserProvider userProvider, 
    IEmailSender emailSender, 
    INotifyFollowers notifyFollowers,
    ILogger<ActivityLogger> logger) : IActivityLogger
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
        var followers = await dbContext.Followers.AsNoTracking()
                .Where(f => f.DocumentId == id)
                .Select(f=>new {f.EmailAddress, f.UserId})
                .ToListAsync(cancellationToken);

        if (followers.Count != 0)
        {
            // send email
            foreach (var follower in followers)
            {
                emailSender.Send(follower.EmailAddress, "Activity", log.ToString());
            }

            // send notification (ie via SignalR)
            await notifyFollowers
                .NotifyAsync(followers.Select(f => f.UserId).Distinct().ToList());
        }
    }
}