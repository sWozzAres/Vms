namespace Utopia.Api.Application.Commands;

public interface IMarkActivityNotificationAsRead<TContext> where TContext : ISystemContext
{
    Task Mark(long id, CancellationToken cancellationToken);
}

public class MarkActivityNotificationAsRead<TContext>(TContext context) : IMarkActivityNotificationAsRead<TContext> where TContext : ISystemContext
{
    public async Task Mark(long id, CancellationToken cancellationToken)
    {
        var activityNotification = await context.ActivityNotifications.FindAsync(new object[] { id }, cancellationToken)
            ?? throw new InvalidOperationException("Activity notification not found.");

        activityNotification.MarkAsRead();
    }
}
