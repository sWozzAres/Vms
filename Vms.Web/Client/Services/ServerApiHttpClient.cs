using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http.Headers;
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

    public async Task<ServiceBookingFullDto> GetServiceBookingFull(Guid id)
    {
        http.DefaultRequestHeaders.Accept.Clear();
        http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.full"));
        return await http.GetFromJsonAsync<ServiceBookingFullDto>($"/api/ServiceBooking/{id}")
            ?? throw new InvalidOperationException("Failed to load service booking.");
    }

    public async Task<List<ServiceBookingFullDto>> GetServiceBookingsFullForVehicleAsync(Guid id)
    {
        http.DefaultRequestHeaders.Accept.Clear();
        http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.full"));
        return await http.GetFromJsonAsync<List<ServiceBookingFullDto>>($"/api/vehicle/{id}/servicebookings")
            ?? throw new InvalidOperationException("Failed to get service bookings.");
    }

    public async Task<ServiceBookingDto> CreateServiceBooking(Guid vehicleId)
    {
        http.DefaultRequestHeaders.Accept.Clear();
        var request = new CreateServiceBookingDto(vehicleId);
        return await DeserializeOrThrow<ServiceBookingDto>(await http.PostAsJsonAsync("/api/servicebooking", request));
    }
    
    public async Task<List<FleetShortDto>?> GetFleetsShortAsync(string filter)
    {
        http.DefaultRequestHeaders.Accept.Clear();
        http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.short"));
        return await http.GetFromJsonAsync<List<FleetShortDto>>($"/api/fleet/{filter}");
    }
    public async Task<FleetShortDto> GetFleetShortAsync(string companyCode, string code)
    {
        http.DefaultRequestHeaders.Accept.Clear();
        http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.short"));
        return await http.GetFromJsonAsync<FleetShortDto>($"/api/company/{companyCode}/fleet/{code}")
            ?? throw new InvalidOperationException("Failed to get customer.");
    }

    public async Task<HttpResponseMessage> AssignFleetToVehicle(Guid vehicleId, string code)
    {
        http.DefaultRequestHeaders.Accept.Clear();
        var response = await http.PostAsJsonAsync($"/api/Vehicle/{vehicleId}/fleet", new AssignFleetToVehicleDto(code));
        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException("Failed to assign fleet.");
        }
        return response;
    }
    public async Task<HttpResponseMessage> RemoveFleetFromVehicle(Guid vehicleId)
    {
        http.DefaultRequestHeaders.Accept.Clear();
        var response = await http.DeleteAsync($"/api/Vehicle/{vehicleId}/fleet");
        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException(response.ReasonPhrase);
        }
        return response;
    }

    public async Task<List<CustomerShortDto>?> GetCustomersShortAsync(string filter)
    {
        http.DefaultRequestHeaders.Accept.Clear();
        http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.short"));
        return await http.GetFromJsonAsync<List<CustomerShortDto>>($"/api/customer/{filter}");
    }
    public async Task<CustomerShortDto> GetCustomerShortAsync(string companyCode, string code)
    {
        http.DefaultRequestHeaders.Accept.Clear();
        http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.short"));
        return await http.GetFromJsonAsync<CustomerShortDto>($"/api/company/{companyCode}/customer/{code}")
            ?? throw new InvalidOperationException("Failed to get customer.");
    }

    public async Task<HttpResponseMessage> AssignCustomerToVehicle(Guid vehicleId, string code)
    {
        http.DefaultRequestHeaders.Accept.Clear();
        var response = await http.PostAsJsonAsync($"/api/Vehicle/{vehicleId}/customer", new AssignCustomerToVehicleDto(code));
        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException("Failed to assign customer.");
        }
        return response;
    }
    public async Task<HttpResponseMessage> RemoveCustomerFromVehicle(Guid vehicleId)
    {
        http.DefaultRequestHeaders.Accept.Clear();
        var response = await http.DeleteAsync($"/api/Vehicle/{vehicleId}/customer");
        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException(response.ReasonPhrase);
        }
        return response;
    }

    public async Task<List<DriverShortDto>?> GetDriversShortAsync(string filter)
    {
        http.DefaultRequestHeaders.Accept.Clear();
        http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.short"));
        return await http.GetFromJsonAsync<List<DriverShortDto>>($"/api/driver/{filter}");
    }
    public async Task<DriverShortDto?> GetDriverShortAsync(Guid id)
    {
        http.DefaultRequestHeaders.Accept.Clear();
        http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.short"));
        return await http.GetFromJsonAsync<DriverShortDto>($"/api/driver/{id}");
    }
    public async Task<HttpResponseMessage> RemoveDriverFromVehicle(Guid vehicleId, Guid driverId)
    {
        http.DefaultRequestHeaders.Accept.Clear();
        var response = await http.DeleteAsync($"/api/Vehicle/{vehicleId}/drivers/{driverId}");
        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException("Failed to remove driver.");
        }
        return response;
    }


    public async Task<HttpResponseMessage> AddDriverToVehicle(Guid vehicleId, Guid driverId)
    {
        http.DefaultRequestHeaders.Accept.Clear();
        return await http.PostAsJsonAsync($"/api/Vehicle/{vehicleId}/drivers", new AddDriverToVehicleDto(driverId));
    }

    public async Task<List<VehicleMakeShortListModel>> GetMakesShortAsync()
    {
        http.DefaultRequestHeaders.Accept.Clear();
        http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.short"));
        return await http.GetFromJsonAsync<List<VehicleMakeShortListModel>>($"/api/vehiclemake")
            ?? throw new InvalidOperationException("Failed to get makes.");
    }
    public async Task<List<VehicleModelShortListModel>> GetModelsShortAsync(string make)
    {
        http.DefaultRequestHeaders.Accept.Clear();
        http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.short"));
        return await http.GetFromJsonAsync<List<VehicleModelShortListModel>>($"/api/vehiclemake/{make}/models")
            ?? throw new InvalidOperationException("Request returned null.");
    }

    public async Task<VehicleFullDto> GetVehicleFull(Guid id)
    {
        http.DefaultRequestHeaders.Accept.Clear();
        http.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/vnd.vehiclefull"));
        return await http.GetFromJsonAsync<VehicleFullDto>($"/api/Vehicle/{id}")
            ?? throw new InvalidOperationException("Failed to load vehicle.");
    }
    public async Task<VehicleDto> CreateVehicle(VehicleDto vehicle)
    {
        http.DefaultRequestHeaders.Accept.Clear();
        return await DeserializeOrThrow<VehicleDto>(await http.PostAsJsonAsync("/api/vehicle", vehicle));
    }

    public async Task<List<CompanyListModel>> GetCompaniesShortAsync()
    {
        http.DefaultRequestHeaders.Accept.Clear();
        http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.short"));
        return await http.GetFromJsonAsync<List<CompanyListModel>>($"/api/company")
            ?? throw new InvalidOperationException("Failed to get list of companies.");
    }
}
