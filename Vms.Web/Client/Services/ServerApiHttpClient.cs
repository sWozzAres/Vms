using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using Vms.Web.Shared;
using static System.Net.WebRequestMethods;

namespace Vms.Web.Client.Services;

public class ServerApiHttpClient(HttpClient http)
{
    readonly HttpClient http = http;

    static async Task<T> DeserializeOrThrow<T>(HttpResponseMessage response)
    {
        if (response.StatusCode == HttpStatusCode.Created)
        {
            var content = await response.Content.ReadFromJsonAsync<T>()
                ?? throw new InvalidOperationException("Failed to deserialize response.");

            return content;
        }
        else
        {
            throw new InvalidOperationException("Unexpected response.");
        }
    }
    void ClearAndAddAcceptHeaders(params string[] headers)
    {
        http.DefaultRequestHeaders.Accept.Clear();
        foreach (var header in headers)
        {
            http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(header));
        }
    }

    #region /api/company
    public async Task<List<ConfirmBookedRefusalReasonDto>> GetConfirmBookedRefusalReasons(string companyCode)
    {
        http.DefaultRequestHeaders.Accept.Clear();
        return await http.GetFromJsonAsync<List<ConfirmBookedRefusalReasonDto>>($"/api/company/{companyCode}/confirmbookedrefusalreasons")
            ?? throw new InvalidOperationException("Failed to get confirm booked refusal reasons.");
    }
    public async Task<List<RefusalReasonDto>> GetRefusalReasons(string companyCode)
    {
        http.DefaultRequestHeaders.Accept.Clear();
        return await http.GetFromJsonAsync<List<RefusalReasonDto>>($"/api/company/{companyCode}/refusalreasons")
            ?? throw new InvalidOperationException("Failed to get refusal reasons.");
    }
    public async Task<List<NonArrivalReasonDto>> GetNonArrivalReasons(string companyCode)
    {
        http.DefaultRequestHeaders.Accept.Clear();
        return await http.GetFromJsonAsync<List<NonArrivalReasonDto>>($"/api/company/{companyCode}/nonarrivalreasons")
            ?? throw new InvalidOperationException("Failed to get non-arrival reasons.");
    }
    public async Task<List<NotCompleteReasonDto>> GetNotCompleteReasons(string companyCode)
    {
        http.DefaultRequestHeaders.Accept.Clear();
        return await http.GetFromJsonAsync<List<NotCompleteReasonDto>>($"/api/company/{companyCode}/notcompletereasons")
            ?? throw new InvalidOperationException("Failed to get not-complete reasons.");
    }
    public async Task<List<RescheduleReasonDto>> GetRescheduleReasons(string companyCode)
    {
        http.DefaultRequestHeaders.Accept.Clear();
        return await http.GetFromJsonAsync<List<RescheduleReasonDto>>($"/api/company/{companyCode}/reschedulereasons")
            ?? throw new InvalidOperationException("Failed to get reschedule reasons.");
    }
    public async Task<List<CompanyListModel>> GetCompaniesShortAsync()
    {
        ClearAndAddAcceptHeaders("application/vnd.short");
        return await http.GetFromJsonAsync<List<CompanyListModel>>($"/api/company")
            ?? throw new InvalidOperationException("Failed to get list of companies.");
    }
    public async Task<FleetShortDto> GetFleetShortByCompanyAsync(string companyCode, string code)
    {
        ClearAndAddAcceptHeaders("application/vnd.short");
        return await http.GetFromJsonAsync<FleetShortDto>($"/api/company/{companyCode}/fleet/{code}")
            ?? throw new InvalidOperationException("Failed to get customer.");
    }
    public async Task<CustomerShortDto> GetCustomerShortByCompanyAsync(string companyCode, string code)
    {
        ClearAndAddAcceptHeaders("application/vnd.short");
        return await http.GetFromJsonAsync<CustomerShortDto>($"/api/company/{companyCode}/customer/{code}")
            ?? throw new InvalidOperationException("Failed to get customer.");
    }
    #endregion
    #region /api/customer
    public async Task<List<CustomerShortDto>?> GetCustomersShortAsync(string filter)
    {
        ClearAndAddAcceptHeaders("application/vnd.short");
        return await http.GetFromJsonAsync<List<CustomerShortDto>>($"/api/customer/{filter}");
    }
    #endregion
    #region /api/driver
    public async Task<List<DriverShortDto>?> GetDriversShortAsync(string filter)
    {
        ClearAndAddAcceptHeaders("application/vnd.short");
        return await http.GetFromJsonAsync<List<DriverShortDto>>($"/api/driver/{filter}");
    }
    public async Task<DriverShortDto?> GetDriverShortAsync(Guid id)
    {
        ClearAndAddAcceptHeaders("application/vnd.short");
        return await http.GetFromJsonAsync<DriverShortDto>($"/api/driver/{id}");
    }
    #endregion
    #region /api/fleet
    public async Task<List<FleetShortDto>?> GetFleetsShortAsync(string filter)
    {
        ClearAndAddAcceptHeaders("application/vnd.short");
        return await http.GetFromJsonAsync<List<FleetShortDto>>($"/api/fleet/{filter}");
    }
    #endregion
    #region /api/servicebooking
    public async Task<PostResponse> AddNote(Guid id, AddNoteDto request)
    {
        http.DefaultRequestHeaders.Accept.Clear();
        return PostResponse.Create(await http.PostAsJsonAsync($"/api/servicebooking/{id}/activity", request));
    }
    public async Task<List<ActivityLogDto>> GetActivity(Guid id)
    {
        http.DefaultRequestHeaders.Accept.Clear();
        return await http.GetFromJsonAsync<List<ActivityLogDto>>($"/api/servicebooking/{id}/activity")
            ?? throw new InvalidOperationException("Failed to load activity.");
    }
    public async Task<PostResponse> SaveServiceBooking(Guid id, ServiceBookingDto request)
    {
        http.DefaultRequestHeaders.Accept.Clear();
        return PostResponse.Create(await http.PostAsJsonAsync($"/api/servicebooking/{id}/edit", request));
    }
    public async Task<PostResponse> AssignSupplier(Guid id, TaskAssignSupplierCommand request)
    {
        http.DefaultRequestHeaders.Accept.Clear();
        return PostResponse.Create(await http.PostAsJsonAsync($"/api/servicebooking/{id}/assignsupplier", request));
    }
    public async Task<PostResponse> BookSupplier(Guid id, TaskBookSupplierCommand request)
    {
        http.DefaultRequestHeaders.Accept.Clear();
        return PostResponse.Create(await http.PostAsJsonAsync($"/api/servicebooking/{id}/booksupplier", request));
    }
    public async Task<PostResponse> ChaseDriver(Guid id, TaskChaseDriverCommand request)
    {
        http.DefaultRequestHeaders.Accept.Clear();
        return PostResponse.Create(await http.PostAsJsonAsync($"/api/servicebooking/{id}/chasedriver", request));
    }
    public async Task<PostResponse> RebookDriver(Guid id, TaskRebookDriverCommand request)
    {
        http.DefaultRequestHeaders.Accept.Clear();
        return PostResponse.Create(await http.PostAsJsonAsync($"/api/servicebooking/{id}/rebookdriver", request));
    }
    public async Task<PostResponse> CheckWorkStatus(Guid id, TaskCheckWorkStatusCommand request)
    {
        http.DefaultRequestHeaders.Accept.Clear();
        return PostResponse.Create(await http.PostAsJsonAsync($"/api/servicebooking/{id}/checkworkstatus", request));
    }
    public async Task<PostResponse> ConfirmBooked(Guid id, TaskConfirmBookedCommand request)
    {
        http.DefaultRequestHeaders.Accept.Clear();
        return PostResponse.Create(await http.PostAsJsonAsync($"/api/servicebooking/{id}/confirmbooked", request));
    }
    public async Task<PostResponse> NotifyCustomer(Guid id, TaskNotifyCustomerCommand request)
    {
        http.DefaultRequestHeaders.Accept.Clear();
        return PostResponse.Create(await http.PostAsJsonAsync($"/api/servicebooking/{id}/notifycustomer", request));
    }
    public async Task<PostResponse> NotifyCustomerDelay(Guid id, TaskNotifyCustomerDelayCommand request)
    {
        http.DefaultRequestHeaders.Accept.Clear();
        return PostResponse.Create(await http.PostAsJsonAsync($"/api/servicebooking/{id}/notifycustomerdelay", request));
    }
    public async Task<PostResponse> CheckArrival(Guid id, TaskCheckArrivalCommand request)
    {
        http.DefaultRequestHeaders.Accept.Clear();
        return PostResponse.Create(await http.PostAsJsonAsync($"/api/servicebooking/{id}/checkarrival", request));
    }
    public async Task<PostResponse> ChangeMotStatus(Guid id, TaskChangeMotStatusCommand request)
    {
        http.DefaultRequestHeaders.Accept.Clear();
        return PostResponse.Create(await http.PostAsJsonAsync($"/api/servicebooking/{id}/changemotstatus", request));
    }
    public async Task<PostResponse> UnbookSupplier(Guid id, TaskUnbookSupplierCommand request)
    {
        http.DefaultRequestHeaders.Accept.Clear();
        return PostResponse.Create(await http.PostAsJsonAsync($"/api/servicebooking/{id}/unbooksupplier", request));
    }
    public async Task<List<SupplierLocatorDto>?> GetSuppliersForServiceBookingShortAsync(Guid id)
    {
        ClearAndAddAcceptHeaders("application/vnd.short");
        return await http.GetFromJsonAsync<List<SupplierLocatorDto>>($"/api/servicebooking/{id}/suppliers");
    }
    public async Task<ServiceBookingFullDto> GetServiceBookingFullAsync(Guid id)
    {
        ClearAndAddAcceptHeaders("application/vnd.full");
        return await http.GetFromJsonAsync<ServiceBookingFullDto>($"/api/servicebooking/{id}")
            ?? throw new InvalidOperationException("Failed to load service booking.");
    }
    public async Task<ServiceBookingDto> CreateServiceBookingAsync(Guid vehicleId, CreateServiceBookingCommand request)
    {
        http.DefaultRequestHeaders.Accept.Clear();
        return await DeserializeOrThrow<ServiceBookingDto>(await http.PostAsJsonAsync("/api/servicebooking", request));
    }
    #endregion
    #region /api/vehicle
    public async Task<OpenEvents> GetOpenEvents(Guid id)
    {
        http.DefaultRequestHeaders.Accept.Clear();
        return await http.GetFromJsonAsync<OpenEvents>($"/api/vehicle/{id}/openevents")
            ?? throw new InvalidOperationException("Failed to load open events.");
    }
    public async Task<VehicleFullDto> GetVehicleFullAsync(Guid id)
    {
        ClearAndAddAcceptHeaders("application/vnd.vehiclefull");
        return await http.GetFromJsonAsync<VehicleFullDto>($"/api/vehicle/{id}")
            ?? throw new InvalidOperationException("Failed to load vehicle.");
    }
    public async Task<VehicleDto> CreateVehicleAsync(VehicleDto vehicle)
    {
        http.DefaultRequestHeaders.Accept.Clear();
        return await DeserializeOrThrow<VehicleDto>(await http.PostAsJsonAsync("/api/vehicle", vehicle));
    }
    public async Task<HttpResponseMessage> RemoveDriverFromVehicleAsync(Guid vehicleId, Guid driverId)
    {
        http.DefaultRequestHeaders.Accept.Clear();
        var response = await http.DeleteAsync($"/api/vehicle/{vehicleId}/drivers/{driverId}");
        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException("Failed to remove driver.");
        }
        return response;
    }
    public async Task<HttpResponseMessage> AddDriverToVehicleAsync(Guid vehicleId, Guid driverId)
    {
        http.DefaultRequestHeaders.Accept.Clear();
        return await http.PostAsJsonAsync($"/api/vehicle/{vehicleId}/drivers", new AddDriverToVehicleCommand(driverId));
    }
    public async Task<List<ServiceBookingFullDto>> GetServiceBookingsForVehicleFullAsync(Guid id)
    {
        ClearAndAddAcceptHeaders("application/vnd.full");
        return await http.GetFromJsonAsync<List<ServiceBookingFullDto>>($"/api/vehicle/{id}/servicebookings")
            ?? throw new InvalidOperationException("Failed to get service bookings.");
    }
    public async Task<HttpResponseMessage> AssignFleetToVehicleAsync(Guid vehicleId, string code)
    {
        http.DefaultRequestHeaders.Accept.Clear();
        var response = await http.PostAsJsonAsync($"/api/vehicle/{vehicleId}/fleet", new AssignFleetToVehicleCommand(code));
        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException("Failed to assign fleet.");
        }
        return response;
    }
    public async Task<HttpResponseMessage> RemoveFleetFromVehicleAsync(Guid vehicleId)
    {
        http.DefaultRequestHeaders.Accept.Clear();
        var response = await http.DeleteAsync($"/api/vehicle/{vehicleId}/fleet");
        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException(response.ReasonPhrase);
        }
        return response;
    }
    public async Task<HttpResponseMessage> AssignCustomerToVehicleAsync(Guid vehicleId, string code)
    {
        http.DefaultRequestHeaders.Accept.Clear();
        var response = await http.PostAsJsonAsync($"/api/vehicle/{vehicleId}/customer", new AssignCustomerToVehicleCommand(code));
        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException("Failed to assign customer.");
        }
        return response;
    }
    public async Task<HttpResponseMessage> RemoveCustomerFromVehicleAsync(Guid vehicleId)
    {
        http.DefaultRequestHeaders.Accept.Clear();
        var response = await http.DeleteAsync($"/api/vehicle/{vehicleId}/customer");
        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException(response.ReasonPhrase);
        }
        return response;
    }
    #endregion
    #region /api/vehiclemake
    public async Task<List<VehicleMakeShortListModel>> GetMakesShortAsync()
    {
        ClearAndAddAcceptHeaders("application/vnd.short");
        return await http.GetFromJsonAsync<List<VehicleMakeShortListModel>>($"/api/vehiclemake")
            ?? throw new InvalidOperationException("Failed to get makes.");
    }
    public async Task<List<VehicleModelShortListModel>> GetModelsShortAsync(string make)
    {
        ClearAndAddAcceptHeaders("application/vnd.short");
        return await http.GetFromJsonAsync<List<VehicleModelShortListModel>>($"/api/vehiclemake/{make}/models")
            ?? throw new InvalidOperationException("Request returned null.");
    }
    #endregion
}