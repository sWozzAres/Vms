using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Vms.Blazor.Admin;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// https://docs.microsoft.com/en-us/aspnet/core/blazor/security/webassembly/additional-scenarios?view=aspnetcore-5.0#configure-the-httpclient-handler-1
builder.Services.AddHttpClient("Vms.ServerAPI", client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
    .AddHttpMessageHandler(sp => sp.GetRequiredService<AuthorizationMessageHandler>()
    .ConfigureHandler(
        authorizedUrls: new[] { "https://localhost:5003" },
        scopes: new[] { "vms.admin" }));

builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("Vms.ServerAPI"));

builder.Services.AddOidcAuthentication(options =>
{
    builder.Configuration.Bind("oidc", options.ProviderOptions);

});

builder.Services.AddAuthorizationCore(configure =>
{
    configure.AddPolicy("IsAdmin", configurePolicy => 
        configurePolicy.Requirements.Add(new ClientAppRequirement("*")));
});

builder.Services.AddSingleton<IAuthorizationHandler, ClientAppHandler>();

await builder.Build().RunAsync();

public class ClientAppRequirement : IAuthorizationRequirement
{
    public string TenantId { get; }

    public ClientAppRequirement(string tenantId) => TenantId = tenantId;
}

public class ClientAppHandler : AuthorizationHandler<ClientAppRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ClientAppRequirement requirement)
    {
        if (!context.User.HasClaim(c => c.Type == "tenantid"))
        {
            return Task.CompletedTask;
        }

        var tenantid = context.User.FindFirst(c => c.Type == "tenantid")?.Value ?? throw new InvalidOperationException("Claim not found.");

        if (tenantid == requirement.TenantId)
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}