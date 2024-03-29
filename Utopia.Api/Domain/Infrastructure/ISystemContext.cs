﻿namespace Utopia.Api.Domain.Infrastructure;

public interface ISystemContext
{
    public DbSet<Login> Logins { get; }
    public DbSet<RecentView> RecentViews { get; }
    public DbSet<ActivityLog> ActivityLog { get; }
    public DbSet<TaskLog> TaskLogs { get; }
    public DbSet<User> Users { get; }
    public DbSet<Follower> Followers { get; }
    public DbSet<Email> Emails { get; }
    public DbSet<ActivityNotification> ActivityNotifications { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
