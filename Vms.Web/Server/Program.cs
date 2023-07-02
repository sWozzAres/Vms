using Microsoft.AspNetCore.ResponseCompression;
using Serilog.Sinks.SystemConsole.Themes;
using Serilog;
using Vms.Domain.Services;
using Vms.Web.Server.Services;
using Microsoft.EntityFrameworkCore;
using Vms.Domain.Infrastructure;
using System.Reflection;
using Vms.Web.Server.Configuration;
using Microsoft.AspNetCore.Authorization;
using Vms.Web.Server.Endpoints;
using Vms.Web.Server.Extensions;
using Vms.Web.Server;
using Microsoft.Extensions.Options;
using Vms.Domain.Infrastructure.Seed;
using Vms.Web.Server.Middleware;

const string AppName = "Vms.Web.Server";

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

builder.Services.Configure<AppSettings>(builder.Configuration);
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUserProvider, UserProvider>();

builder.Services.AddDbContext<VmsDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("VmsDbConnection"),
                sqlOptions =>
                {
                    sqlOptions.UseNetTopologySuite();
                    sqlOptions.UseDateOnlyTimeOnly();
                    sqlOptions.MigrationsAssembly(Assembly.GetExecutingAssembly().GetName().Name);
                    //sqlOptions.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                    sqlOptions.EnableRetryOnFailure();
                }), ServiceLifetime.Scoped
            );

builder.Services.AddApplicationSecurity();

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

Log.Information("Applying migrations ({ApplicationContext})...", AppName);
UserProvider.InMigration = true;
app.MigrateDbContext<VmsDbContext>((context, services) =>
{
    var logger = services.GetService<ILogger<VmsDbContextSeeder>>() ?? throw new InvalidOperationException(); ;
    var env = services.GetService<IWebHostEnvironment>() ?? throw new InvalidOperationException();
    var settings = services.GetService<IOptions<AppSettings>>() ?? throw new InvalidOperationException();

    new VmsDbContextSeeder(context, logger)
        .SeedAsync(env, settings)
        .Wait();
});
UserProvider.InMigration = false;

//app.UseMiddleware<VmsDomainExceptionMiddleware>();

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

//app.MapWhen(ctx => ctx.Request.Host.Port == 5003 ||
//    ctx.Request.Host.Equals("firstapp.com"), adminApp =>
//    {
//        adminApp.Use((ctx, nxt) =>
//        {
//            ctx.Request.Path = "/AdminApp" + ctx.Request.Path;
//            return nxt();
//        });

//        adminApp.UseBlazorFrameworkFiles("/AdminApp");
//        adminApp.UseStaticFiles();
//        adminApp.UseStaticFiles("/AdminApp");
//        adminApp.UseRouting();
//        adminApp.UseAuthorization();
//        adminApp.UseEndpoints(endpoints =>
//        {
//            endpoints.MapControllers();
//            endpoints.MapFallbackToFile("/AdminApp/{*path:nonfile}",
//                "AdminApp/index.html");
//        });
//    });

app.MapWhen(ctx => ctx.Request.Host.Port == 5002 ||
    ctx.Request.Host.Equals("secondapp.com"), clientApp =>
    {
        clientApp.Use((ctx, nxt) =>
        {
            ctx.Request.Path = "/ClientApp" + ctx.Request.Path;
            return nxt();
        });

        clientApp.UseMiddleware<VmsDomainExceptionMiddleware>();
        clientApp.UseMiddleware<TransactionMiddleware>();

        clientApp.UseBlazorFrameworkFiles("/ClientApp");
        clientApp.UseStaticFiles();
        clientApp.UseStaticFiles("/ClientApp");
        clientApp.UseRouting();
        clientApp.UseAuthorization();
        clientApp.UseEndpoints(endpoints =>
        {
            CompanyEndpoints.Map(endpoints);
            VehicleEndpoints.Map(endpoints);

            endpoints.MapControllers();
            endpoints.MapFallbackToFile("/ClientApp/{*path:nonfile}",
                "ClientApp/index.html");
        });
    });

//app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();


app.MapRazorPages();
app.MapControllers();
//app.MapFallbackToFile("index.html");

app.Run();
