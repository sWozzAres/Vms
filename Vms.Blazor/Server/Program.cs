using Microsoft.AspNetCore.ResponseCompression;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog();

Log.Logger = new LoggerConfiguration()
        //.MinimumLevel.Verbose()
        .Enrich.WithProperty("ApplicationContext", Program.AppName)
        .Enrich.FromLogContext()
        //.WriteTo.Console()
        //.WriteTo.Seq(string.IsNullOrWhiteSpace(seqServerUrl) ? "http://seq" : seqServerUrl)
        //.WriteTo.Http(string.IsNullOrWhiteSpace(logstashUrl) ? "http://logstash:8080" : logstashUrl)
        .ReadFrom.Configuration(builder.Configuration)
        .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}", theme: AnsiConsoleTheme.Code)
        .CreateLogger();

Log.Information("Configuring web host ({ApplicationContext})...", Program.AppName);

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
app.MapWhen(ctx => ctx.Request.Host.Port == 5001 ||
    ctx.Request.Host.Equals("firstapp.com"), first =>
    {
        first.Use((ctx, nxt) =>
        {
            ctx.Request.Path = "/AdminApp" + ctx.Request.Path;
            return nxt();
        });

        first.UseBlazorFrameworkFiles("/AdminApp");
        first.UseStaticFiles();
        first.UseStaticFiles("/AdminApp");
        first.UseRouting();

        first.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapFallbackToFile("/AdminApp/{*path:nonfile}",
                "AdminApp/index.html");
        });
    });

app.MapWhen(ctx => ctx.Request.Host.Port == 5002 ||
    ctx.Request.Host.Equals("secondapp.com"), second =>
    {
        second.Use((ctx, nxt) =>
        {
            ctx.Request.Path = "/ClientApp" + ctx.Request.Path;
            return nxt();
        });

        second.UseBlazorFrameworkFiles("/ClientApp");
        second.UseStaticFiles();
        second.UseStaticFiles("/ClientApp");
        second.UseRouting();

        second.UseEndpoints(endpoints =>
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


public partial class Program
{
    public static string Namespace = typeof(Program).Namespace ?? string.Empty;
    public static string AppName = Namespace[(Namespace.LastIndexOf('.', Namespace.LastIndexOf('.') - 1) + 1)..];
}