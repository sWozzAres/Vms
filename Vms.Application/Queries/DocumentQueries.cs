namespace Vms.Application.Queries;

public interface IDocumentQueries
{
    Task<List<ActivityLogDto>> GetActivities(Guid id, CancellationToken cancellationToken);
}

public class DocumentQueries(VmsDbContext context, IUserProvider userProvider) : IDocumentQueries
{
    public async Task<List<ActivityLogDto>> GetActivities(Guid id, CancellationToken cancellationToken)
        => await context.ActivityLog.AsNoTracking()
            .Where(l => l.DocumentId == id)
            .OrderBy(l => l.EntryDate)
            .Select(l => l.ToDto())
            .ToListAsync(cancellationToken);



}
