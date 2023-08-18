using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Logging;
using Vms.Domain.Infrastructure.Services;

namespace Vms.Domain.Infrastructure;

public class VmsDbContextFactory : IDesignTimeDbContextFactory<VmsDbContext>
{
    public VmsDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<VmsDbContext>();
        optionsBuilder.UseSqlServer(@"Data Source=localhost\SQL2019;Initial Catalog=Vms.Data;Integrated Security=true;TrustServerCertificate=True;Encrypt=False",
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
