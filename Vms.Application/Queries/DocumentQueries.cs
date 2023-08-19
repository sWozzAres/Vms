namespace Vms.Application.Queries;

public class DocumentQueries(VmsDbContext context
    //IUserProvider userProvider
    )
{
    public async Task<List<ActivityLogDto>> GetActivities(Guid id, CancellationToken cancellationToken)
        => await context.ActivityLog.AsNoTracking()
            .Where(l => l.DocumentId == id)
            .OrderBy(l => l.EntryDate)
            .Select(l => l.ToDto())
            .ToListAsync(cancellationToken);

}
