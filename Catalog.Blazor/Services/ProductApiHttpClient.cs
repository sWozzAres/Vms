using System.Net.Http.Json;
using Catalog.Shared;
using Utopia.Blazor.Application.Common.Extensions;

namespace Catalog.Blazor.Services;

public class ProductApiHttpClient(HttpClient http)
{
    readonly HttpClient http = http;
    public async Task<ProductFullDto> GetProductFullAsync(Guid id)
    {
        http.DefaultRequestHeaders.Accept.ClearAndAdd("application/vnd.full");
        return await http.GetFromJsonAsync<ProductFullDto>($"/api/product/{id}")
            ?? throw new InvalidOperationException("Failed to load product.");
    }
}
