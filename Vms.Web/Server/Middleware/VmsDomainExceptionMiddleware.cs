using System.Net;
using System.Text.Json;
using Vms.Domain.Exceptions;
using Vms.Web.Server.Middleware;

namespace Vms.Web.Server;

public class VmsDomainExceptionMiddleware(RequestDelegate next, ILogger<VmsDomainExceptionMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (VmsDomainException ex)
        {
            logger.LogError("VmsDomainException handler {exception}", ex);
            context.Response.StatusCode = (int)HttpStatusCode.UnprocessableEntity;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonSerializer.Serialize(
                new { title = "There was a problem processing the request.", status = 422, detail = ex.Message }
            ));
        }
        catch (Exception ex)
        {
            logger.LogError("VmsDomainException handler {exception}", ex);
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonSerializer.Serialize(
                new { title = "There was a problem processing the request.", status = 400, detail = ex.Message }
            ));
        }
    }
}
