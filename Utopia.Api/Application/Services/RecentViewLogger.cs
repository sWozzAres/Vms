using Utopia.Api.Application.Services;

namespace Vms.Application.Services;

public interface IRecentViewLogger<TContext> where TContext : ISystemContext
{
    Task LogAsync(Guid documentId);
}

public class RecentViewLogger<TContext>(
    TContext context, 
    IUserProvider userProvider,
    ITimeService timeService) : IRecentViewLogger<TContext> where TContext : ISystemContext
{
    public async Task LogAsync(Guid documentId)
    {
        var recentView = await context.RecentViews.SingleOrDefaultAsync(r => r.DocumentId == documentId && r.UserId == userProvider.UserId);
        if (recentView is null)
        {
            recentView = new RecentView(documentId, userProvider.UserId, timeService.Now);
            context.RecentViews.Add(recentView);
        }
        else
        {
            recentView.ViewDate = timeService.Now;
        }
    }
}
