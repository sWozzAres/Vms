using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Vms.Domain.Infrastructure;
using Vms.Web.Shared;

namespace Vms.Web.Server.Endpoints;

public static class VehicleEndpoints
{
    public static void Map(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/ClientApp/api/Vehicle",
            [Authorize(Policy = "ClientPolicy")] async (int list, int start, int take, VmsDbContext context, CancellationToken cancellationToken) =>
        {
            int totalCount = await context.Vehicles.CountAsync();
          
            var result = await context.Vehicles
                .Skip(start)
                .Take(take)
                .Select(x => new VehicleListModel(x.Id, x.CompanyCode, x.Vrm, x.Make, x.Model))
                .ToListAsync(cancellationToken);
            
            return new ListResult<VehicleListModel>(totalCount, result);
        });

        endpoints.MapGet("/ClientApp/api/VehicleMake/All",
            [Authorize(Policy = "ClientPolicy")] async (VmsDbContext context, CancellationToken cancellationToken)
                => await context.VehicleMakes
                    .Select(x => new VehicleMakeListModel(x.Make))
                    .ToListAsync(cancellationToken)
            );
    }
}
