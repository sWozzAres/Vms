using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Scrum.Api.Application.Commands;
using Scrum.Api.Application.Queries;

namespace Scrum.Api.Extensions;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddScrumApplication(this IServiceCollection services, string connectionString)
    {
        // queries
        services.AddTransient<ProductQueries>();

        // commands
        services.AddTransient<CreateProduct>();
        services.AddTransient<UpdateProduct>();
        services.AddTransient<DeleteProduct>();

        // dbcontext
        services.AddDbContext<ScrumDbContext>(options =>
        {
            options.EnableSensitiveDataLogging();

            options.UseSqlServer(connectionString, sqlOptions =>
            {
                //sqlOptions.UseNetTopologySuite();
                sqlOptions.UseDateOnlyTimeOnly();
                sqlOptions.MigrationsAssembly(Assembly.GetExecutingAssembly().GetName().Name);
                sqlOptions.EnableRetryOnFailure();
            });
        });

        return services;
    }
}
