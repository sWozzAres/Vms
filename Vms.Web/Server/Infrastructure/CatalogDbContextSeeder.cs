using Catalog.Api.Domain;
using Catalog.Api.Domain.Infrastructure;
using Microsoft.Extensions.Options;
using Vms.Web.Server;

namespace Vms.Domain.Infrastructure.Seed;

public class CatalogDbContextSeeder(
    CatalogDbContext context,
    IServiceProvider services,
    ILogger<CatalogDbContextSeeder> logger)
{
    T New<T>() where T : notnull => services.GetRequiredService<T>();

    public async Task SeedAsync(IWebHostEnvironment env, IOptions<AppSettings> settings)
    {
        var userProvider = New<IUserProvider>();

        var strategy = context.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            using var transaction = context.Database.BeginTransaction();
            try
            {
                if (!context.Products.Any())
                {
                    var p1 = new Product("P1", "The first product");
                    var p2 = new Product("P2", "The second product");

                    context.Products.AddRange(p1, p2);
                }

                await context.SaveChangesAsync();
                await transaction.CommitAsync();

                return true;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to seed database.");
                await transaction.RollbackAsync();
                return false;
            }
        });
    }
}
