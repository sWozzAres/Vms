namespace Vms.Web.Server.Endpoints;

public static class VehicleEndpoints
{
    public static void Map(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/ClientApp/api/VehicleMake/All",
            [Authorize(Policy = "ClientPolicy")] async (VmsDbContext context, CancellationToken cancellationToken)
                => await context.VehicleMakes
                    .Select(x => new VehicleMakeShortListModel(x.Make))
                    .ToListAsync(cancellationToken)
            );
    }
}
