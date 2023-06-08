using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Vms.Domain.Entity;
using Vms.Domain.Entity.Configuration;
using Vms.Domain.Services;

namespace Vms.Domain.Infrastructure;

public class VmsDbContextClient : VmsDbContext
{
    public VmsDbContextClient(DbContextOptions<VmsDbContext> options, IUserProvider userProvider, ILogger<VmsDbContext> logger) : base(options, userProvider, logger)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CompanyEntityTypeConfiguration).Assembly);

        if (string.IsNullOrEmpty(_userProvider.TenantId)) throw new InvalidOperationException("Users tenantid is not set.");

        modelBuilder.Entity<Company>().HasQueryFilter(x => x.Code == _userProvider.TenantId);
        modelBuilder.Entity<Customer>().HasQueryFilter(x => x.CompanyCode == _userProvider.TenantId);
        modelBuilder.Entity<Network>().HasQueryFilter(x => x.CompanyCode == _userProvider.TenantId);
        modelBuilder.Entity<Fleet>().HasQueryFilter(x => x.CompanyCode == _userProvider.TenantId);
        modelBuilder.Entity<Vehicle>().HasQueryFilter(x => x.CompanyCode == _userProvider.TenantId);
    }
}
