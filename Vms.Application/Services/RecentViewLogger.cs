namespace Vms.Application.Services;

public interface IRecentViewLogger
{
    Task LogAsync(Guid documentId);
}

public class RecentViewLogger(VmsDbContext context, IUserProvider userProvider) : IRecentViewLogger
{
    public async Task LogAsync(Guid documentId)
    {
        var recentView = await context.RecentViews.SingleOrDefaultAsync(r => r.DocumentId == documentId && r.UserId == userProvider.UserId);
        if (recentView is null)
        {
            recentView = new RecentView(documentId, userProvider.UserId, DateTime.Now);
            context.RecentViews.Add(recentView);
        }
        else
        {
            recentView.ViewDate = DateTime.Now;
        }
    }
}
