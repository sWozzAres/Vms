using System.Net;
using System.Text.Json;
using Scrum.Api.Exceptions;

namespace Scrum.Web.Api.Server;

public class ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (ScrumDomainException ex)
        {
            logger.LogError(ex, "ScrumDomainException handler");
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
