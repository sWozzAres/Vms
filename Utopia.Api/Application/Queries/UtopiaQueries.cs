namespace Vms.Application.Queries;

public interface IUtopiaQueries<TContext> where TContext : ISystemContext
{
    Task<IEnumerable<ActivityNotificationDto>> GetActivityNotifications(CancellationToken cancellationToken);
}

public class UtopiaQueries<TContext>(TContext context, IUserProvider userProvider) : IUtopiaQueries<TContext> where TContext : ISystemContext
{
    public async Task<IEnumerable<ActivityNotificationDto>> GetActivityNotifications(CancellationToken cancellationToken)
    {
        var query = from an in context.ActivityNotifications
                    join al in context.ActivityLog on an.ActivityLogId equals al.Id
                    orderby an.EntryDate descending
                    where an.UserId == userProvider.UserId
                    select new
                    {
                        an.Id,
                        al.Text,
                        an.Read,
                        al.EntryDate,
                        an.DocumentId,
                        an.DocumentKind,
                        an.DocumentKey
                    };

        var result = await query.ToListAsync(cancellationToken);

        return result.Select(r => new ActivityNotificationDto(r.Id, r.DocumentId, r.DocumentKind, r.DocumentKey,
            r.Text, r.Read, r.EntryDate));
    }
}