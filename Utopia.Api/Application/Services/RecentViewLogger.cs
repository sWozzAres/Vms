using Utopia.Api.Domain.Infrastructure;
using Utopia.Api.Services;

namespace Vms.Application.Services;

public interface IRecentViewLogger<TContext> where TContext : ISystemContext
{
    Task LogAsync(Guid documentId);
}

public class RecentViewLogger<TContext>(TContext context, IUserProvider userProvider) : IRecentViewLogger<TContext> where TContext : ISystemContext
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
