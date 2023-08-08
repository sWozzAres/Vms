using System.Globalization;
using System.Net.Http.Json;
using Vms.Web.Client.Common.Extensions;
using Vms.Web.Shared;

namespace Vms.Web.Client.Common.Services;

public class ServiceBookingApiClient(HttpClient http)
{
    readonly HttpClient http = http;

    #region /api/servicebooking
    public async Task<List<ServiceBookingFullDto>> GetServiceBookingsForVehicleFullAsync(Guid id)
    {
        http.DefaultRequestHeaders.Accept.ClearAndAdd("application/vnd.full");
        return await http.GetFromJsonAsync<List<ServiceBookingFullDto>>($"/api/servicebooking/vehicle/{id}")
            ?? throw new InvalidOperationException("Failed to get service bookings.");
    }
    public async Task<PostResponse> Follow(Guid id)
    {
        http.DefaultRequestHeaders.Accept.Clear();
        return PostResponse.Create(await http.PostAsJsonAsync($"/api/servicebooking/{id}/follow", new { }));
    }
    public async Task<bool> Unfollow(Guid id)
    {
        http.DefaultRequestHeaders.Accept.Clear();
        var response = await http.DeleteAsync($"/api/servicebooking/{id}/follow");
        return response.IsSuccessStatusCode;
    }
    public async Task<PostResponse> AddNote(string id, string note)
    {
        http.DefaultRequestHeaders.Accept.Clear();
        return PostResponse.Create(await http.PostAsJsonAsync($"/api/servicebooking/{id}/activity", new AddNoteDto(note)));
    }
    public Task<List<ActivityLogDto>?> GetActivity(Guid id)
    {
        http.DefaultRequestHeaders.Accept.Clear();
        return http.GetFromJsonAsync<List<ActivityLogDto>>($"/api/servicebooking/{id}/activity");
        //?? throw new InvalidOperationException("Failed to load activity.");
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
    //public async Task<PostResponse> BookSupplier(Guid id, TaskBookSupplierCommand request)
    //{
    //    http.DefaultRequestHeaders.Accept.Clear();
    //    return PostResponse.Create(await http.PostAsJsonAsync($"/api/servicebooking/{id}/booksupplier", request));
    //}
    //public async Task<PostResponse> ChaseDriver(Guid id, TaskChaseDriverCommand request)
    //{
    //    http.DefaultRequestHeaders.Accept.Clear();
    //    return PostResponse.Create(await http.PostAsJsonAsync($"/api/servicebooking/{id}/chasedriver", request));
    //}
    //public async Task<PostResponse> RebookDriver(Guid id, TaskRebookDriverCommand request)
    //{
    //    http.DefaultRequestHeaders.Accept.Clear();
    //    return PostResponse.Create(await http.PostAsJsonAsync($"/api/servicebooking/{id}/rebookdriver", request));
    //}
    //public async Task<PostResponse> CheckWorkStatus(Guid id, TaskCheckWorkStatusCommand request)
    //{
    //    http.DefaultRequestHeaders.Accept.Clear();
    //    return PostResponse.Create(await http.PostAsJsonAsync($"/api/servicebooking/{id}/checkworkstatus", request));
    //}
    //public async Task<PostResponse> ConfirmBooked(Guid id, TaskConfirmBookedCommand request)
    //{
    //    http.DefaultRequestHeaders.Accept.Clear();
    //    return PostResponse.Create(await http.PostAsJsonAsync($"/api/servicebooking/{id}/confirmbooked", request));
    //}
    //public async Task<PostResponse> NotifyCustomer(Guid id, TaskNotifyCustomerCommand request)
    //{
    //    http.DefaultRequestHeaders.Accept.Clear();
    //    return PostResponse.Create(await http.PostAsJsonAsync($"/api/servicebooking/{id}/notifycustomer", request));
    //}
    //public async Task<PostResponse> NotifyCustomerDelay(Guid id, TaskNotifyCustomerDelayCommand request)
    //{
    //    http.DefaultRequestHeaders.Accept.Clear();
    //    return PostResponse.Create(await http.PostAsJsonAsync($"/api/servicebooking/{id}/notifycustomerdelay", request));
    //}
    //public async Task<PostResponse> CheckArrival(Guid id, TaskCheckArrivalCommand request)
    //{
    //    http.DefaultRequestHeaders.Accept.Clear();
    //    return PostResponse.Create(await http.PostAsJsonAsync($"/api/servicebooking/{id}/checkarrival", request));
    //}
    //public async Task<PostResponse> ChangeMotStatus(Guid id, TaskChangeMotStatusCommand request)
    //{
    //    http.DefaultRequestHeaders.Accept.Clear();
    //    return PostResponse.Create(await http.PostAsJsonAsync($"/api/servicebooking/{id}/changemotstatus", request));
    //}
    //public async Task<PostResponse> UnbookSupplier(Guid id, TaskUnbookSupplierCommand request)
    //{
    //    http.DefaultRequestHeaders.Accept.Clear();
    //    return PostResponse.Create(await http.PostAsJsonAsync($"/api/servicebooking/{id}/unbooksupplier", request));
    //}
    public async Task<List<SupplierLocatorDto>?> GetSuppliersForServiceBookingShortAsync(Guid id, string filter)
    {
        http.DefaultRequestHeaders.Accept.ClearAndAdd("application/vnd.short");
        return await http.GetFromJsonAsync<List<SupplierLocatorDto>>($"/api/servicebooking/{id}/suppliers?filter={filter}");
    }
    public async Task<ServiceBookingFullDto> GetServiceBookingFullAsync(Guid id)
    {
        http.DefaultRequestHeaders.Accept.ClearAndAdd("application/vnd.full");
        return await http.GetFromJsonAsync<ServiceBookingFullDto>($"/api/servicebooking/{id}")
            ?? throw new InvalidOperationException("Failed to load service booking.");
    }
    public async Task<PostResponse> CreateServiceBookingAsync(CreateServiceBookingCommand request)
    {
        http.DefaultRequestHeaders.Accept.Clear();
        //return await DeserializeOrThrow<ServiceBookingDto>(await http.PostAsJsonAsync("/api/servicebooking", request));
        return PostResponse.Create(await http.PostAsJsonAsync("/api/servicebooking", request));
    }
    #endregion
}