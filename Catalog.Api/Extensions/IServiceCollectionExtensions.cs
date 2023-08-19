using Utopia.Api.Application.Commands;
using Utopia.Api.Application.Services;
using Vms.Application.Queries;

namespace Catalog.Api.Extensions;

public static class IServiceCollectionExtensions
{
    public static void AddCatalogApplication(this IServiceCollection services)
    {
        // queries
        services.AddScoped<UtopiaQueries<CatalogDbContext>>();

        services.AddScoped<IProductQueries, ProductQueries>();

        // services
        services.AddScoped<IActivityLogger<CatalogDbContext>, ActivityLogger<CatalogDbContext>>();
        services.AddScoped<ITaskLogger<CatalogDbContext>, TaskLogger<CatalogDbContext>>();
        services.AddScoped<IEmailSender<CatalogDbContext>, EmailSender<CatalogDbContext>>();
        services.AddScoped<IRecentViewLogger<CatalogDbContext>, RecentViewLogger<CatalogDbContext>>();

        // utopia
        services.AddScoped<IMarkActivityNotificationAsRead<CatalogDbContext>, MarkActivityNotificationAsRead<CatalogDbContext>>();

        // product
        //services.AddScoped<IFollowProduct, FollowProduct>();
        //services.AddScoped<IUnfollowProduct, UnfollowProduct>();
        //services.AddScoped<IAddNoteProduct, AddNoteProduct>();
        //services.AddScoped<IEditProduct, EditProduct>();
        //services.AddScoped<ICreateProduct, CreateProduct>();
    }
}
