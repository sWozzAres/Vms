namespace Vms.Application.Commands.SupplierUseCase;

public interface IUnfollowSupplier
{
    Task<bool> UnfollowAsync(Guid id, CancellationToken cancellationToken);
}

public class UnfollowSupplier(VmsDbContext dbContext, IUserProvider userProvider, IActivityLogger<VmsDbContext> activityLog) : IUnfollowSupplier
{
    readonly VmsDbContext DbContext = dbContext;
    readonly IUserProvider UserProvider = userProvider;
    readonly StringBuilder SummaryText = new();

    SupplierRole? Supplier;
    Guid Id;
    CancellationToken CancellationToken;

    public async Task<bool> UnfollowAsync(Guid id, CancellationToken cancellationToken)
    {
        CancellationToken = cancellationToken;
        Id = id;

        var supplier = await DbContext.Suppliers.SingleOrDefaultAsync(s => s.Id == Id, CancellationToken)
                    ?? throw new InvalidOperationException("Failed to load supplier.");

        Supplier = new(this);

        SummaryText.AppendLine("# Unfollow");

        bool removed = await Supplier.RemoveFollower();

        if (removed)
            _ = await activityLog.AddAsync(Id, SummaryText, CancellationToken);

        return removed;
    }

    class SupplierRole(UnfollowSupplier ctx)
    {
        public async Task<bool> RemoveFollower()
        {
            var follow = await ctx.DbContext.Followers
                .SingleOrDefaultAsync(f => f.UserId == ctx.UserProvider.UserId && f.DocumentId == ctx.Id, ctx.CancellationToken);

            if (follow is not null)
            {
                ctx.DbContext.Followers.Remove(follow);
                return true;
            }

            return false;
        }
    }
}
