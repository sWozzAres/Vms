using System.Data.Common;
using System.Net.Http.Json;
using Vms.Web.Shared;
using static System.Net.WebRequestMethods;

namespace Vms.Web.Client.Services;

public class ServerApiHttpClient
{
    private readonly HttpClient http;

    public ServerApiHttpClient(HttpClient http)
    {
        this.http = http;
    }

    public async Task<List<DriverShortDto>?> GetDriversShortAsync(string filter)
    {
        http.DefaultRequestHeaders.Accept.Clear();
        http.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/vnd.drivershort"));
        return await http.GetFromJsonAsync<List<DriverShortDto>>($"/api/driver/{filter}");
    }
    public async Task<DriverFullDto?> GetDriverFullAsync(string emailAddress)
    {
        http.DefaultRequestHeaders.Accept.Clear();
        http.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/vnd.driverfull"));
        return await http.GetFromJsonAsync<DriverFullDto>($"/api/driver/{emailAddress}");
    }
    public async Task<HttpResponseMessage> RemoveDriverFromVehicle(string vehicleId, string emailAddress)
    {
        http.DefaultRequestHeaders.Accept.Clear();
        return await http.DeleteAsync($"/api/Vehicle/{vehicleId}/drivers/{emailAddress}");
    }
    public async Task<HttpResponseMessage> AddDriverToVehicle(string vehicleId, string emailAddress)
    {
        http.DefaultRequestHeaders.Accept.Clear();
        return await http.PostAsJsonAsync($"/api/Vehicle/{vehicleId}/drivers", emailAddress);
    }
}
