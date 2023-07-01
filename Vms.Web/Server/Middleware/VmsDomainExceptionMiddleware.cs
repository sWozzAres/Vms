using System.Net;
using System.Text.Json;
using Vms.Domain.Exceptions;

namespace Vms.Web.Server;

public class VmsDomainExceptionMiddleware
{
    readonly RequestDelegate _next;

    public VmsDomainExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (VmsDomainException ex)
        {
            context.Response.StatusCode = (int)HttpStatusCode.UnprocessableEntity;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonSerializer.Serialize(
                new { title = "There was a problem processing the request.", status = 422, detail = ex.Message }
            ));
        }
        catch (Exception ex)
        {
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonSerializer.Serialize(
                new { title = "There was a problem processing the request.", status = 400, detail = ex.Message }
            ));
        }
    }
}
