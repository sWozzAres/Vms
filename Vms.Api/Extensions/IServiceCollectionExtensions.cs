using Utopia.Api.Application.Commands;
using Utopia.Api.Application.Services;
using Vms.Application.Commands.CompanyUseCase;
using Vms.Application.Commands.ServiceBookingUseCase;
using Vms.Application.Commands.SupplierUseCase;
using Vms.Application.Commands.VehicleUseCase;

namespace Vms.Api.Extensions;

public static class IServiceCollectionExtensions
{
    public static void AddVmsApplication(this IServiceCollection services)
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
        services.AddScoped<IMarkActivityNotificationAsRead<VmsDbContext>, MarkActivityNotificationAsRead<VmsDbContext>>();

        // supplier
        services.AddScoped<IFollowSupplier, FollowSupplier>();
        services.AddScoped<IUnfollowSupplier, UnfollowSupplier>();
        services.AddScoped<IAddNoteSupplier, AddNoteSupplier>();
        services.AddScoped<IEditSupplier, EditSupplier>();
        services.AddScoped<ICreateSupplier, CreateSupplier>();

        // vehicle
        services.AddScoped<FollowVehicle>();
        services.AddScoped<IUnfollowVehicle, UnfollowVehicle>();
        services.AddScoped<AddNoteVehicle>();
        services.AddScoped<EditVehicle>();
        services.AddScoped<CreateServiceBooking>();

        // company
        services.AddScoped<CreateVehicle>();

        // service booking
        services.AddScoped<AddNoteServiceBooking>();
        services.AddScoped<EditServiceBooking>();
        services.AddScoped<FollowServiceBooking>();
        services.AddScoped<UnfollowServiceBooking>();
        services.AddScoped<BookSupplier>();
        services.AddScoped<ConfirmBooked>();
        services.AddScoped<CheckArrival>();
        services.AddScoped<CheckWorkStatus>();
        services.AddScoped<ChaseDriver>();
        services.AddScoped<RebookDriver>();
        services.AddScoped<NotifyCustomer>();
        services.AddScoped<NotifyCustomerDelay>();
        services.AddScoped<SupplierLocator>();
        services.AddScoped<AssignSupplier>();
        services.AddScoped<UnbookSupplier>();
        services.AddScoped<AutomaticallyAssignSupplier>();
    }
}
