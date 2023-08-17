using Blazored.LocalStorage;
using Catalog.Blazor.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.SignalR.Client;
using Utopia.Blazor.Application.Vms.Services;
using Vms.Web.Client;
using Vms.Web.Client.Security;
using Vms.Web.Client.Shared;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Logging.AddConfiguration(builder.Configuration.GetSection("Logging"));

//builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

var authorizedUrls = new[] { "https://localhost:5002" };
var scopes = new[] { "vms.client" };

// https://docs.microsoft.com/en-us/aspnet/core/blazor/security/webassembly/additional-scenarios?view=aspnetcore-5.0#configure-the-httpclient-handler-1
#region Vms
builder.Services.AddHttpClient("Vms.ServerAPI", client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
    .AddHttpMessageHandler(sp => sp.GetRequiredService<AuthorizationMessageHandler>()
    .ConfigureHandler(authorizedUrls, scopes));

builder.Services.AddHttpClient<ServerApiHttpClient>(client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
    .AddHttpMessageHandler(sp => sp.GetRequiredService<AuthorizationMessageHandler>()
    .ConfigureHandler(authorizedUrls, scopes));

builder.Services.AddHttpClient<ServiceBookingApiClient>(client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
    .AddHttpMessageHandler(sp => sp.GetRequiredService<AuthorizationMessageHandler>()
    .ConfigureHandler(authorizedUrls, scopes));

builder.Services.AddHttpClient<VehicleApiClient>(client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
    .AddHttpMessageHandler(sp => sp.GetRequiredService<AuthorizationMessageHandler>()
    .ConfigureHandler(authorizedUrls, scopes));

builder.Services.AddHttpClient<SupplierApiClient>(client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
    .AddHttpMessageHandler(sp => sp.GetRequiredService<AuthorizationMessageHandler>()
    .ConfigureHandler(authorizedUrls, scopes));
#endregion
#region Catalog
builder.Services.AddHttpClient<ProductApiHttpClient>(client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
    .AddHttpMessageHandler(sp => sp.GetRequiredService<AuthorizationMessageHandler>()
    .ConfigureHandler(authorizedUrls, scopes));
#endregion

// Supply HttpClient instances that include access tokens when making requests to the server project
builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("Vms.ServerAPI"));

//builder.Services.AddApiAuthorization();

builder.Services.AddOidcAuthentication(options =>
{
    builder.Configuration.Bind("oidc", options.ProviderOptions);
});

builder.Services.AddAuthorizationCore(configure =>
{
    configure.AddPolicy("IsAdmin", configurePolicy =>
        configurePolicy.Requirements.Add(new TenantRequirement("*")));
});

builder.Services.AddApiAuthorization();

builder.Services.AddBlazoredLocalStorage();
builder.Services.AddScoped<ISearchHistoryProvider, SearchHistoryProvider>();
//builder.Services.AddScoped<ISearchHistoryProvider>(sp =>
//{

//    var i = sp.GetRequiredService<ILocalStorageService>();

//    var service = new SearchHistoryProvider(i);
//    service.InitializeAsync().Wa.Wait();
//    return service;

//});

builder.Services.AddScoped(sp =>
{
    var navMan = sp.GetRequiredService<NavigationManager>();
    var accessTokenProvider = sp.GetRequiredService<IAccessTokenProvider>();
    return new HubConnectionBuilder()
        .WithUrl(navMan.ToAbsoluteUri("/chathub"), options =>
        {
            options.AccessTokenProvider = async () =>
            {
                var accessTokenResult = await accessTokenProvider.RequestAccessToken();
                accessTokenResult.TryGetToken(out var accessToken);
                if (accessTokenResult.TryGetToken(out var token))
                {
                    return token.Value;
                }
                else
                {
                    //throw new InvalidOperationException("Failed to get access token.");
                    return "";
                }
            };
        })
        .WithAutomaticReconnect()
        .Build();
});

await builder.Build().RunAsync();


