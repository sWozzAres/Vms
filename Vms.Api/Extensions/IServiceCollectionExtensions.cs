﻿using Utopia.Api.Application.Services;
using Vms.Application.Commands.ServiceBookingUseCase;
using Vms.Application.Commands.SupplierUseCase;
using Vms.Application.Commands.VehicleUseCase;

namespace Vms.Api.Extensions;

public static class IServiceCollectionExtensions
{
    public static void AddVmsApplication(this IServiceCollection services)
    {
        // queries
        services.AddScoped<IDocumentQueries, DocumentQueries>();
        services.AddScoped<ICompanyQueries, CompanyQueries>();
        services.AddScoped<IDriverQueries, DriverQueries>();
        services.AddScoped<ICustomerQueries, CustomerQueries>();
        services.AddScoped<IFleetQueries, FleetQueries>();
        services.AddScoped<INetworkQueries, NetworkQueries>();
        services.AddScoped<ISupplierQueries, SupplierQueries>();
        services.AddScoped<IVehicleQueries, VehicleQueries>();
        services.AddScoped<IServiceBookingQueries, ServiceBookingQueries>();

        // services
        services.AddScoped<IActivityLogger<VmsDbContext>, ActivityLogger<VmsDbContext>>();
        services.AddScoped<ITaskLogger<VmsDbContext>, TaskLogger<VmsDbContext>>();
        services.AddScoped<IEmailSender<VmsDbContext>, EmailSender<VmsDbContext>>();
        services.AddScoped<IRecentViewLogger<VmsDbContext>, RecentViewLogger<VmsDbContext>>();
        services.AddScoped<ISearchManager, SearchManager>();

        // supplier
        services.AddScoped<IFollowSupplier, FollowSupplier>();
        services.AddScoped<IUnfollowSupplier, UnfollowSupplier>();
        services.AddScoped<IAddNoteSupplier, AddNoteSupplier>();
        services.AddScoped<IEditSupplier, EditSupplier>();
        services.AddScoped<ICreateSupplier, CreateSupplier>();

        // vehicle
        services.AddScoped<IFollowVehicle, FollowVehicle>();
        services.AddScoped<IUnfollowVehicle, UnfollowVehicle>();
        services.AddScoped<IAddNoteVehicle, AddNoteVehicle>();
        services.AddScoped<IEditVehicle, EditVehicle>();
        services.AddScoped<ICreateVehicle, CreateVehicle>();

        // service booking
        services.AddScoped<IAddNoteServiceBooking, AddNoteServiceBooking>();
        services.AddScoped<IEditServiceBooking, EditServiceBooking>();
        services.AddScoped<IFollowServiceBooking, FollowServiceBooking>();
        services.AddScoped<IUnfollowServiceBooking, UnfollowServiceBooking>();
        services.AddScoped<IBookSupplier, BookSupplier>();
        services.AddScoped<IConfirmBooked, ConfirmBooked>();
        services.AddScoped<ICheckArrival, CheckArrival>();
        services.AddScoped<ICheckWorkStatus, CheckWorkStatus>();
        services.AddScoped<IChaseDriver, ChaseDriver>();
        services.AddScoped<IRebookDriver, RebookDriver>();
        services.AddScoped<INotifyCustomer, NotifyCustomer>();
        services.AddScoped<INotifyCustomerDelay, NotifyCustomerDelay>();
        services.AddScoped<ICreateServiceBooking, CreateServiceBooking>();
        services.AddScoped<ISupplierLocator, SupplierLocator>();
        services.AddScoped<IAssignSupplier, AssignSupplier>();
        services.AddScoped<IUnbookSupplier, UnbookSupplier>();
        services.AddScoped<IAutomaticallyAssignSupplier, AutomaticallyAssignSupplier>();
    }
}