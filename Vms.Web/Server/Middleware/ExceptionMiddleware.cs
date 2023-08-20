using System.Text.Json;

namespace Vms.Web.Server;

public class ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (VmsDomainException ex)
        {
            logger.LogError(ex, "VmsDomainException handler");
            context.Response.Clear();
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonSerializer.Serialize(
                new { title = "There was a problem processing the request.", status = 400, detail = ex.Message }
            ));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception handler");
            context.Response.Clear();
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonSerializer.Serialize(
                new { title = "There was a problem processing the request.", status = 400, detail = "Unexpected error." }
            ));
        }
    }
}
