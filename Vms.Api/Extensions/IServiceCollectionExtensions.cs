using Utopia.Api.Application.Commands;
using Utopia.Api.Application.Services;
using Vms.Application.Commands;
using Vms.Application.Commands.CompanyUseCase;
using Vms.Application.Commands.ServiceBookingUseCase;
using Vms.Application.Commands.SupplierUseCase;
using Vms.Application.Commands.VehicleUseCase;

namespace Vms.Api.Extensions;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddVmsApplication(this IServiceCollection services)
    {
        // queries
        services.AddScoped<UtopiaQueries<VmsDbContext>>();

        services.AddScoped<DocumentQueries>();
        services.AddScoped<CompanyQueries>();
        services.AddScoped<DriverQueries>();
        services.AddScoped<CustomerQueries>();
        services.AddScoped<FleetQueries>();
        services.AddScoped<NetworkQueries>();
        services.AddScoped<SupplierQueries>();
        services.AddScoped<VehicleQueries>();
        services.AddScoped<ServiceBookingQueries>();

        // services
        services.AddScoped<IActivityLogger<VmsDbContext>, ActivityLogger<VmsDbContext>>();
        services.AddScoped<ITaskLogger<VmsDbContext>, TaskLogger<VmsDbContext>>();
        services.AddScoped<IEmailSender<VmsDbContext>, EmailSender<VmsDbContext>>();
        services.AddScoped<IRecentViewLogger<VmsDbContext>, RecentViewLogger<VmsDbContext>>();
        services.AddScoped<ISearchManager, SearchManager>();

        // utopia
        services.AddTransient<IMarkActivityNotificationAsRead<VmsDbContext>, MarkActivityNotificationAsRead<VmsDbContext>>();

        // supplier
        services.AddTransient<IFollowSupplier, FollowSupplier>();
        services.AddTransient<IUnfollowSupplier, UnfollowSupplier>();
        services.AddTransient<IAddNoteSupplier, AddNoteSupplier>();
        services.AddTransient<IEditSupplier, EditSupplier>();
        services.AddTransient<ICreateSupplier, CreateSupplier>();

        // vehicle
        services.AddTransient<FollowVehicle>();
        services.AddTransient<UnfollowVehicle>();
        services.AddTransient<AddNoteVehicle>();
        services.AddTransient<EditVehicle>();
        services.AddTransient<CreateServiceBooking>();
        services.AddTransient<ChangeVrm>();

        // company
        services.AddTransient<CreateVehicle>();
        services.AddTransient<CreateCompany>();
        services.AddTransient<CreateCustomer>();
        services.AddTransient<CreateFleet>();
        services.AddTransient<CreateNetwork>();
        services.AddTransient<CreateDriver>();
        services.AddTransient<CreateSupplier>();

        // service booking
        services.AddTransient<AddNoteServiceBooking>();
        services.AddTransient<EditServiceBooking>();
        services.AddTransient<FollowServiceBooking>();
        services.AddTransient<UnfollowServiceBooking>();
        services.AddTransient<BookSupplier>();
        services.AddTransient<ConfirmBooked>();
        services.AddTransient<CheckArrival>();
        services.AddTransient<CheckWorkStatus>();
        services.AddTransient<ChaseDriver>();
        services.AddTransient<RebookDriver>();
        services.AddTransient<NotifyCustomer>();
        services.AddTransient<NotifyCustomerDelay>();
        services.AddTransient<SupplierLocator>();
        services.AddTransient<AssignSupplier>();
        services.AddTransient<UnbookSupplier>();
        services.AddTransient<AutomaticallyAssignSupplier>();

        // other
        services.AddTransient<CreateMake>();
        services.AddTransient<CreateModel>();
        return services;
    }
}
