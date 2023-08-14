using System.Net;
using System.Net.Http.Json;
using Vms.Web.Client.Common.Extensions;
using Vms.Web.Shared;

namespace Vms.Web.Client.Common.Services;

public class VehicleApiClient(HttpClient http)
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
    #region /api/vehicle
    public async Task<PostResponse> AddNote(string id, string note)
    {
        http.DefaultRequestHeaders.Accept.Clear();
        return PostResponse.Create(await http.PostAsJsonAsync($"/api/vehicle/{id}/activity", new AddNoteDto(note)));
    }
    public Task<List<ActivityLogDto>?> GetActivity(Guid id)
    {
        http.DefaultRequestHeaders.Accept.Clear();
        return http.GetFromJsonAsync<List<ActivityLogDto>>($"/api/vehicle/{id}/activity");
        //?? throw new InvalidOperationException("Failed to load activity.");
    }
    public async Task<PostResponse> Follow(Guid id)
    {
        http.DefaultRequestHeaders.Accept.Clear();
        return PostResponse.Create(await http.PostAsJsonAsync($"/api/vehicle/{id}/follow", new { }));
    }
    public async Task<bool> Unfollow(Guid id)
    {
        http.DefaultRequestHeaders.Accept.Clear();
        var response = await http.DeleteAsync($"/api/vehicle/{id}/follow");
        return response.IsSuccessStatusCode;
    }
    public async Task<PostResponse> SaveVehicle(Guid id, VehicleDto request)
    {
        http.DefaultRequestHeaders.Accept.Clear();
        return PostResponse.Create(await http.PostAsJsonAsync($"/api/vehicle/{id}/edit", request));
    }
    public async Task<VehicleEvents> GetEvents(Guid id, Guid? serviceBookingId = null)
    {
        http.DefaultRequestHeaders.Accept.Clear();
        string url = $"/api/vehicle/{id}/events";
        if (serviceBookingId is not null)
            url += $"/{serviceBookingId}";
        return await http.GetFromJsonAsync<VehicleEvents>(url)
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
}