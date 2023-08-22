using Microsoft.EntityFrameworkCore;
using Scrum.Api.Domain;
using Scrum.Api.Domain.Infrastructure;

namespace Scrum.Web.Api.Infrastructure.Seed;

public class ScrumDbContextSeeder(ScrumDbContext context, IServiceProvider services)
{
    T New<T>() where T : notnull => services.GetRequiredService<T>();

    public async Task SeedAsync(IWebHostEnvironment env)
    {
        var logger = New<ILogger<ScrumDbContextSeeder>>();
        
        var strategy = context.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            using var transaction = context.Database.BeginTransaction();
            try
            {
                if (!context.Products.Any())
                {
                    var p1 = new Product("The first product");
                    var p2 = new Product("The second product");

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
