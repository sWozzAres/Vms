using Microsoft.EntityFrameworkCore;
using Scrum.Api.Domain.Configuration;

namespace Scrum.Api.Domain.Infrastructure;

public class ScrumDbContext : DbContext
{
    public DbSet<Product> Products => Set<Product>(); 
    public DbSet<ProductIncrement> ProductIncrements => Set<ProductIncrement>();
    public DbSet<ProductBacklogItem> ProductBacklogItems => Set<ProductBacklogItem>();
    public DbSet<SprintBacklogItem> SprintBacklogItems => Set<SprintBacklogItem>();
    public ScrumDbContext(DbContextOptions<ScrumDbContext> options) : base(options) { }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ProductBacklogItemEntityTypeConfiguration).Assembly);
    }
}
