using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Logging;
using Vms.Domain.Services;

namespace Vms.Domain.Infrastructure;

public class VmsDbContextFactory : IDesignTimeDbContextFactory<VmsDbContext>
{
    public VmsDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<VmsDbContext>();
        optionsBuilder.UseSqlServer(@"Data Source=SKYLAKE\SQL2019;Initial Catalog=VehicleManagementSystem;Integrated Security=true;TrustServerCertificate=True;Encrypt=False",
            x =>
            {
                x.UseNetTopologySuite();
                x.UseDateOnlyTimeOnly();
            });

        var factory = new LoggerFactory();
        var logger = factory.CreateLogger<VmsDbContext>();
        return new VmsDbContext(optionsBuilder.Options, new DesignTimeUserProvider(), logger);
    }
}

public class DesignTimeUserProvider : IUserProvider
{
    public string UserId => string.Empty;

    public string TenantId => "DES001";
}
