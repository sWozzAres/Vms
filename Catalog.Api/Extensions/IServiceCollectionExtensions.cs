using Utopia.Api.Application.Services;

namespace Catalog.Api.Extensions;

public static class IServiceCollectionExtensions
{
    public static void AddCatalogApplication(this IServiceCollection services)
    {
        // queries
        services.AddScoped<IProductQueries, ProductQueries>();

        // services
        services.AddScoped<IActivityLogger<CatalogDbContext>, ActivityLogger<CatalogDbContext>>();
        services.AddScoped<ITaskLogger<CatalogDbContext>, TaskLogger<CatalogDbContext>>();
        services.AddScoped<IEmailSender<CatalogDbContext>, EmailSender<CatalogDbContext>>();
        services.AddScoped<IRecentViewLogger<CatalogDbContext>, RecentViewLogger<CatalogDbContext>>();

        // product
        //services.AddScoped<IFollowProduct, FollowProduct>();
        //services.AddScoped<IUnfollowProduct, UnfollowProduct>();
        //services.AddScoped<IAddNoteProduct, AddNoteProduct>();
        //services.AddScoped<IEditProduct, EditProduct>();
        //services.AddScoped<ICreateProduct, CreateProduct>();
    }
}
