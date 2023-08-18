using System.Text;
using Microsoft.Extensions.Logging;
using Utopia.Api.Domain.Infrastructure;
using Utopia.Api.Services;
using Vms.Application.Services;

namespace Utopia.Api.Application.Services;

public interface IActivityLogger<TContext> where TContext : ISystemContext
{
    Task<ActivityLog> AddNoteAsync(Guid documentId, StringBuilder log, CancellationToken cancellationToken);
    Task<ActivityLog> AddAsync(Guid documentId, StringBuilder log, CancellationToken cancellationToken);
    Task<ActivityLog> AddAsync(Guid documentId, StringBuilder log, DateTime taskTime, CancellationToken cancellationToken);
}
/// <summary>
/// Logs an activity and notifies any followers.
/// </summary>
public class ActivityLogger<TContext>(TContext dbContext,
    IUserProvider userProvider,
    IEmailSender<TContext> emailSender,
    INotifyFollowers notifyFollowers,
    ILogger<ActivityLogger<TContext>> logger,
    ITimeService timeService) : IActivityLogger<TContext> where TContext : ISystemContext
{
    public async Task<ActivityLog> AddAsync(Guid documentId, StringBuilder log, DateTime taskTime, CancellationToken cancellationToken)
    {
        await NotifyFollowers(documentId, log, cancellationToken);
        return LogActivity(documentId, log, taskTime);
    }

    public Task<ActivityLog> AddAsync(Guid documentId, StringBuilder log, CancellationToken cancellationToken)
        => AddAsync(documentId, log, timeService.Now(), cancellationToken);

    public async Task<ActivityLog> AddNoteAsync(Guid documentId, StringBuilder log, CancellationToken cancellationToken)
    {
        await NotifyFollowers(documentId, log, cancellationToken);
        return LogActivity(documentId, log, timeService.Now(), true);
    }

    private ActivityLog LogActivity(Guid documentId, StringBuilder log, DateTime taskTime, bool isNote = false)
    {
        logger.LogDebug("Logging activity for user {user}.", userProvider.UserId);

        var entry = new ActivityLog(documentId, log.ToString(), userProvider.UserId, userProvider.UserName, taskTime, isNote);
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
        emailSender.Send(followers.Select(f => f.EmailAddress), "Activity", log.ToString());

        // send notification (ie via SignalR)
        await notifyFollowers
            .NotifyAsync(followers.Select(f => f.UserId));
    }
}