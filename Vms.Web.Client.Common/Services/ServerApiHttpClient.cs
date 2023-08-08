using System.Net.Http.Json;
using Vms.Web.Client.Common.Extensions;
using Vms.Web.Shared;

namespace Vms.Web.Client.Common.Services;

public class ServerApiHttpClient(HttpClient http)
{
    readonly HttpClient http = http;


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
    public async Task<List<FleetShortDto>?> GetFleetsShortAsync(string companyCode, string filter)
    {
        http.DefaultRequestHeaders.Accept.ClearAndAdd("application/vnd.short");
        return await http.GetFromJsonAsync<List<FleetShortDto>>($"/api/company/{companyCode.Trim()}/fleets?filter={filter}");
    }
    public async Task<CustomerShortDto> GetCustomerShortByCompanyAsync(string companyCode, string code)
    {
        http.DefaultRequestHeaders.Accept.ClearAndAdd("application/vnd.short");
        return await http.GetFromJsonAsync<CustomerShortDto>($"/api/company/{companyCode}/customers/{code}")
            ?? throw new InvalidOperationException("Failed to get customer.");
    }
    public async Task<List<CustomerShortDto>?> GetCustomersShortAsync(string companyCode, string filter)
    {
        http.DefaultRequestHeaders.Accept.ClearAndAdd("application/vnd.short");
        return await http.GetFromJsonAsync<List<CustomerShortDto>>($"/api/company/{companyCode.Trim()}/customers?filter={filter}");
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