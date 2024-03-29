using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;
using Vms.Blazor.Server.Configuration;
using Vms.Blazor.Server.Services;
using Vms.Domain.Infrastructure;
using Vms.Domain.Services;

const string AppName = "Vms.Blazor.Server";

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog();

Log.Logger = new LoggerConfiguration()
        //.MinimumLevel.Verbose()
        .Enrich.WithProperty("ApplicationContext", AppName)
        .Enrich.FromLogContext()
        //.WriteTo.Console()
        //.WriteTo.Seq(string.IsNullOrWhiteSpace(seqServerUrl) ? "http://seq" : seqServerUrl)
        //.WriteTo.Http(string.IsNullOrWhiteSpace(logstashUrl) ? "http://logstash:8080" : logstashUrl)
        .ReadFrom.Configuration(builder.Configuration)
        .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}", theme: AnsiConsoleTheme.Code)
        .CreateLogger();

Log.Information("Configuring web host ({ApplicationContext})...", AppName);

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUserProvider, UserProvider>();

builder.Services.AddDbContext<VmsDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("VmsDbConnection"),
                sqlOptions =>
                {
                    sqlOptions.UseNetTopologySuite();
                    sqlOptions.UseDateOnlyTimeOnly();
                    sqlOptions.MigrationsAssembly(Assembly.GetExecutingAssembly().GetName().Name);
                    sqlOptions.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                }), ServiceLifetime.Scoped
            );

builder.Services.AddApplicationSecurity();

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

//app.UseBlazorFrameworkFiles();
app.MapWhen(ctx => ctx.Request.Host.Port == 5003 ||
    ctx.Request.Host.Equals("firstapp.com"), adminApp =>
    {
        adminApp.Use((ctx, nxt) =>
        {
            ctx.Request.Path = "/AdminApp" + ctx.Request.Path;
            return nxt();
        });

        adminApp.UseBlazorFrameworkFiles("/AdminApp");
        adminApp.UseStaticFiles();
        adminApp.UseStaticFiles("/AdminApp");
        adminApp.UseRouting();
        adminApp.UseAuthorization();
        adminApp.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapFallbackToFile("/AdminApp/{*path:nonfile}",
                "AdminApp/index.html");
        });
    });

app.MapWhen(ctx => ctx.Request.Host.Port == 5002 ||
    ctx.Request.Host.Equals("secondapp.com"), clientApp =>
    {
        clientApp.Use((ctx, nxt) =>
        {
            ctx.Request.Path = "/ClientApp" + ctx.Request.Path;
            return nxt();
        });

        clientApp.UseBlazorFrameworkFiles("/ClientApp");
        clientApp.UseStaticFiles();
        clientApp.UseStaticFiles("/ClientApp");
        clientApp.UseRouting();
        clientApp.UseAuthorization();
        clientApp.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapFallbackToFile("/ClientApp/{*path:nonfile}",
                "ClientApp/index.html");
        });
    });

app.UseStaticFiles();

app.UseRouting();

app.MapRazorPages();
app.MapControllers();
//app.MapFallbackToFile("index.html");

app.Run();