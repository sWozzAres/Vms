using Catalog.Api.Domain;
using Catalog.Api.Domain.Infrastructure;
using Microsoft.Extensions.Options;
using Utopia.Api.Application.Services;
using Vms.Web.Server;

namespace Vms.Domain.Infrastructure.Seed;

public interface ICatalogDbContextSeeder
{
    Task SeedAsync(IWebHostEnvironment env, IOptions<AppSettings> settings);
}

public class CatalogDbContextSeeder(
    CatalogDbContext context,
    //ISearchManager searchManager,
    ILogger<CatalogDbContextSeeder> logger
    //ILoggerFactory loggerFactory,
    //IActivityLogger<CatalogDbContext> activityLog,
    //ITaskLogger<CatalogDbContext> taskLogger,
    //ITimeService timeService
    ) : IVmsDbContextSeeder
{
    public async Task SeedAsync(IWebHostEnvironment env, IOptions<AppSettings> settings)
    {
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
                logger.LogError("Failed to seed database {ex}.", ex);
                await transaction.RollbackAsync();
                return false;
            }
        });
    }
}
