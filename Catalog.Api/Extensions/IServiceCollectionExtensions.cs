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
        services.AddTransient<IMarkActivityNotificationAsRead<CatalogDbContext>, MarkActivityNotificationAsRead<CatalogDbContext>>();

        // product
        //services.AddTransient<IFollowProduct, FollowProduct>();
        //services.AddTransient<IUnfollowProduct, UnfollowProduct>();
        //services.AddTransient<IAddNoteProduct, AddNoteProduct>();
        //services.AddTransient<IEditProduct, EditProduct>();
        //services.AddTransient<ICreateProduct, CreateProduct>();
    }
}
