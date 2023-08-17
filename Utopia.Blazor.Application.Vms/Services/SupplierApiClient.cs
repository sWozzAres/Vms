namespace Utopia.Blazor.Application.Vms.Services;

public class SupplierApiClient(HttpClient http)
{
    readonly HttpClient http = http;
    #region /api/supplier
    public async Task<PostResponse> CreateSupplierAsync(CreateSupplierDto supplier)
    {
        http.DefaultRequestHeaders.Accept.Clear();
        //return await DeserializeOrThrow<SupplierDto>(await http.PostAsJsonAsync("/api/supplier", supplier));
        return PostResponse.Create(await http.PostAsJsonAsync("/api/supplier", supplier));
    }
    public async Task<PostResponse> AddNote(string id, string note)
    {
        http.DefaultRequestHeaders.Accept.Clear();
        return PostResponse.Create(await http.PostAsJsonAsync($"/api/supplier/{id}/activity", new AddNoteDto(note)));
    }
    public Task<List<ActivityLogDto>?> GetActivity(Guid id)
    {
        http.DefaultRequestHeaders.Accept.Clear();
        return http.GetFromJsonAsync<List<ActivityLogDto>>($"/api/supplier/{id}/activity");
        //?? throw new InvalidOperationException("Failed to load activity.");
    }
    public async Task<PostResponse> Follow(Guid id)
    {
        http.DefaultRequestHeaders.Accept.Clear();
        return PostResponse.Create(await http.PostAsJsonAsync($"/api/supplier/{id}/follow", new { }));
    }
    public async Task<bool> Unfollow(Guid id)
    {
        http.DefaultRequestHeaders.Accept.Clear();
        var response = await http.DeleteAsync($"/api/supplier/{id}/follow");
        return response.IsSuccessStatusCode;
    }
    public async Task<SupplierFullDto> GetSupplierFullAsync(string code)
    {
        http.DefaultRequestHeaders.Accept.ClearAndAdd("application/vnd.full");
        return await http.GetFromJsonAsync<SupplierFullDto>($"/api/supplier/{code}")
            ?? throw new InvalidOperationException("Failed to load supplier.");
    }
    public async Task<PostResponse> SaveSupplier(string code, SupplierDto request)
    {
        http.DefaultRequestHeaders.Accept.Clear();
        return PostResponse.Create(await http.PostAsJsonAsync($"/api/supplier/{code}/edit", request));
    }
    #endregion
}