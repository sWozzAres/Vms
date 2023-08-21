using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Scrum.Api.Domain.Infrastructure;

namespace Vms.Domain.Infrastructure;

public class ScrumDbContextFactory : IDesignTimeDbContextFactory<ScrumDbContext>
{
    public ScrumDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ScrumDbContext>();
        optionsBuilder.UseSqlServer(@"Data Source=localhost\SQL2019;Initial Catalog=Scrum.Data;Integrated Security=true;TrustServerCertificate=True;Encrypt=False",
            x =>
            {
                x.UseDateOnlyTimeOnly();
            });

        return new ScrumDbContext(optionsBuilder.Options);
    }
}
