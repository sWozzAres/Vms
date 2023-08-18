namespace Vms.Domain.Infrastructure;

public class CatalogDbContextFactory : IDesignTimeDbContextFactory<CatalogDbContext>
{
    public CatalogDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<CatalogDbContext>();
        optionsBuilder.UseSqlServer(@"Data Source=localhost\SQL2019;Initial Catalog=Catalog.Data;Integrated Security=true;TrustServerCertificate=True;Encrypt=False",
            x =>
            {
                //x.UseNetTopologySuite();
                x.UseDateOnlyTimeOnly();
            });

        var factory = new LoggerFactory();
        return new CatalogDbContext(optionsBuilder.Options);
    }
}
