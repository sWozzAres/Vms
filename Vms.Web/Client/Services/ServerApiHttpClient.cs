using System.Net;
using System.Net.Http.Json;
using Vms.Web.Client.Extensions;
using Vms.Web.Shared;

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
    #region App
    public async Task<PostResponse> RegisterLogin()
    {
        http.DefaultRequestHeaders.Accept.Clear();
        return PostResponse.Create(await http.PostAsJsonAsync($"api/app/register", new { }));
    }
    public async Task<List<UserDto>> GetUsersForCompany(string companyCode)
    {
        http.DefaultRequestHeaders.Accept.Clear();
        return await http.GetFromJsonAsync<List<UserDto>>($"/api/app/users/{companyCode}")
            ?? throw new InvalidOperationException("Failed to get users.");
    }
    public async Task<List<EntityTagDto>> Search(string searchString)
    {
        http.DefaultRequestHeaders.Accept.Clear();
        return await http.GetFromJsonAsync<List<EntityTagDto>>($"/api/search/{searchString}")
            ?? throw new InvalidOperationException("Failed to get search.");
    }
    #endregion
    #region Task Locking
    public async Task<Guid> Lock(string url)
    {
        http.DefaultRequestHeaders.Accept.Clear();
        var result = PostResponse.Create(await http.PostAsJsonAsync($"{url}/lock", new { }));
        if (result is PostResponse.Success)
        {
            var dto = result.Response.Content.ReadFromJsonAsync<LockDto>().GetAwaiter().GetResult()
                ?? throw new InvalidOperationException("Failed to deserialize.");

            return dto.Id;
        }
        throw new InvalidOperationException("Failed to lock.");
    }
    public async Task<HttpResponseMessage> Unlock(string url, Guid lockId)
    {
        http.DefaultRequestHeaders.Accept.Clear();
        return await http.DeleteAsync($"{url}/lock/{lockId}");
    }
    public async Task<PostResponse> RefreshLock(string url, Guid lockId)
    {
        http.DefaultRequestHeaders.Accept.Clear();
        return PostResponse.Create(await http.PostAsJsonAsync($"{url}/lock/{lockId}/refresh", new { }));
    }
    #endregion
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
        http.DefaultRequestHeaders.Accept.ClearAndAdd("application/vnd.short");
        return await http.GetFromJsonAsync<List<CompanyListModel>>($"/api/company")
            ?? throw new InvalidOperationException("Failed to get list of companies.");
    }
    public async Task<FleetShortDto> GetFleetShortByCompanyAsync(string companyCode, string code)
    {
        http.DefaultRequestHeaders.Accept.ClearAndAdd("application/vnd.short");
        return await http.GetFromJsonAsync<FleetShortDto>($"/api/company/{companyCode}/fleet/{code}")
            ?? throw new InvalidOperationException("Failed to get customer.");
    }
    public async Task<CustomerShortDto> GetCustomerShortByCompanyAsync(string companyCode, string code)
    {
        http.DefaultRequestHeaders.Accept.ClearAndAdd("application/vnd.short");
        return await http.GetFromJsonAsync<CustomerShortDto>($"/api/company/{companyCode}/customer/{code}")
            ?? throw new InvalidOperationException("Failed to get customer.");
    }
    #endregion
    #region /api/customer
    public async Task<List<CustomerShortDto>?> GetCustomersShortAsync(string filter)
    {
        http.DefaultRequestHeaders.Accept.ClearAndAdd("application/vnd.short");
        return await http.GetFromJsonAsync<List<CustomerShortDto>>($"/api/customer/{filter}");
    }
    #endregion
    #region /api/driver
    public async Task<List<DriverShortDto>?> GetDriversShortAsync(string filter)
    {
        http.DefaultRequestHeaders.Accept.ClearAndAdd("application/vnd.short");
        return await http.GetFromJsonAsync<List<DriverShortDto>>($"/api/driver/{filter}");
    }
    public async Task<DriverShortDto?> GetDriverShortAsync(Guid id)
    {
        http.DefaultRequestHeaders.Accept.ClearAndAdd("application/vnd.short");
        return await http.GetFromJsonAsync<DriverShortDto>($"/api/driver/{id}");
    }
    #endregion
    #region /api/fleet
    public async Task<List<FleetShortDto>?> GetFleetsShortAsync(string filter)
    {
        http.DefaultRequestHeaders.Accept.ClearAndAdd("application/vnd.short");
        return await http.GetFromJsonAsync<List<FleetShortDto>>($"/api/fleet/{filter}");
    }
    #endregion
    #region /api/vehicle
    public async Task<PostResponse> SaveVehicle(Guid id, VehicleDto request)
    {
        http.DefaultRequestHeaders.Accept.Clear();
        return PostResponse.Create(await http.PostAsJsonAsync($"/api/vehicle/{id}/edit", request));
    }
    public async Task<OpenEvents> GetOpenEvents(Guid id)
    {
        http.DefaultRequestHeaders.Accept.Clear();
        return await http.GetFromJsonAsync<OpenEvents>($"/api/vehicle/{id}/openevents")
            ?? throw new InvalidOperationException("Failed to load open events.");
    }
    public async Task<VehicleFullDto> GetVehicleFullAsync(Guid id)
    {
        http.DefaultRequestHeaders.Accept.ClearAndAdd("application/vnd.vehiclefull");
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
        http.DefaultRequestHeaders.Accept.ClearAndAdd("application/vnd.short");
        return await http.GetFromJsonAsync<List<VehicleMakeShortListModel>>($"/api/vehiclemake")
            ?? throw new InvalidOperationException("Failed to get makes.");
    }
    public async Task<List<VehicleModelShortListModel>> GetModelsShortAsync(string make)
    {
        http.DefaultRequestHeaders.Accept.ClearAndAdd("application/vnd.short");
        return await http.GetFromJsonAsync<List<VehicleModelShortListModel>>($"/api/vehiclemake/{make}/models")
            ?? throw new InvalidOperationException("Request returned null.");
    }
    #endregion
}