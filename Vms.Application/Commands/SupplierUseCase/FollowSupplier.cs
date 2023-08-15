﻿namespace Vms.Application.Commands.SupplierUseCase;

public interface IFollowSupplier
{
    Task FollowAsync(Guid id, CancellationToken cancellationToken);
}

public class FollowSupplier(VmsDbContext dbContext, IUserProvider userProvider, IActivityLogger activityLog) : IFollowSupplier
{
    readonly VmsDbContext DbContext = dbContext;
    readonly IUserProvider UserProvider = userProvider;
    readonly StringBuilder SummaryText = new();

    SupplierRole? Supplier;

    public async Task FollowAsync(Guid id, CancellationToken cancellationToken)
    {
        Supplier = new(await DbContext.Suppliers.SingleOrDefaultAsync(s => s.Id == id, cancellationToken)
                    ?? throw new InvalidOperationException("Failed to load supplier."), this);

        SummaryText.AppendLine("# Follow");

        Supplier.AddFollower();

        _ = await activityLog.AddAsync(id, SummaryText, cancellationToken);
    }

    class SupplierRole(Supplier self, FollowSupplier ctx)
    {
        public void AddFollower()
        {
            var f = new Follower(self.Id, ctx.UserProvider.UserId);
            ctx.DbContext.Followers.Add(f);
        }
    }
}
