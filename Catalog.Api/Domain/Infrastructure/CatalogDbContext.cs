using Utopia.Api.Domain.Infrastructure;
using Utopia.Api.Domain.System;
using Utopia.Api.Domain.System.Configuration;

namespace Catalog.Api.Domain.Infrastructure;

public class CatalogDbContext(DbContextOptions options) : DbContext(options), ISystemContext
{
    public DbSet<Product> Products => Set<Product>();
    #region System
    public DbSet<Login> Logins => Set<Login>();
    public DbSet<ActivityLog> ActivityLog => Set<ActivityLog>();
    public DbSet<Email> Emails => Set<Email>();
    public DbSet<Follower> Followers => Set<Follower>();
    public DbSet<RecentView> RecentViews => Set<RecentView>();
    public DbSet<TaskLog> TaskLogs => Set<TaskLog>();
    public DbSet<User> Users => Set<User>();

    #endregion
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(Product).Assembly);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(UserEntityTypeConfiguration).Assembly);
    }
}
