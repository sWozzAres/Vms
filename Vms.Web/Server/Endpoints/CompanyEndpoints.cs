using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Vms.Domain.Infrastructure;

namespace Vms.Web.Server.Endpoints;

public static class CompanyEndpoints
{
    public static void Map(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/ClientApp/test", () =>
        {
            return Results.Ok("Hello World!");
        });

        endpoints.MapGet("/ClientApp/api/Company", [Authorize(Policy = "ClientPolicy")] async (int? list, VmsDbContext context, CancellationToken cancellationToken) =>
        {
            //return Results.Ok("Hello World2!");
            return await context.Companies
                .Select(x => new Vms.Web.Shared.CompanyListModel(x.Code, x.Name))
                .ToListAsync(cancellationToken);
        });
    }
}
