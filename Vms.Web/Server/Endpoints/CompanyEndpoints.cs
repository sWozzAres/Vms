namespace Vms.Web.Server.Endpoints;

public static class CompanyEndpoints
{
    public static void Map(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/ClientApp/api/Company",
            [Authorize(Policy = "ClientPolicy")] async (int list, int start, int take, VmsDbContext context, CancellationToken cancellationToken) =>
        {
            //int totalCount = await context.Companies.CountAsync();

            var result = await context.Companies
                .Skip(start)
                .Take(take)
                .Select(x => new CompanyListDto(x.Code, x.Name))
                .ToListAsync(cancellationToken);
            return result;

            //return new CompanyListResponse(result, totalCount);
        });
    }
}
