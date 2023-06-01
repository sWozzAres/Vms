using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Vms.Domain.Services;

namespace Vms.Domain.Infrastructure;

public class VmsDbContextFactory : IDesignTimeDbContextFactory<VmsDbContext>
{
    public VmsDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<VmsDbContext>();
        optionsBuilder.UseSqlServer(@"Data Source=SKYLAKE\SQL2016;Initial Catalog=VehicleManagementSystem;Integrated Security=true;TrustServerCertificate=True;Encrypt=False",
            x =>
            {
                x.UseNetTopologySuite();
                x.UseDateOnlyTimeOnly();
            });

        return new VmsDbContext(optionsBuilder.Options, new DesignTimeUserProvider());
    }
}

public class DesignTimeUserProvider : IUserProvider
{
    public Guid UserId => Guid.Empty;

    public string TenantId => "DES001";
}
