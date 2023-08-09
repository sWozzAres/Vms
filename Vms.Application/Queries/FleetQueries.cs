namespace Vms.Application.Queries;

public interface IFleetQueries
{
    Task<(int TotalCount, List<FleetListDto> Result)> GetFleets(FleetListOptions list, int start, int take, CancellationToken cancellationToken);
}

public class FleetQueries(VmsDbContext context, IUserProvider userProvider) : IFleetQueries
{
    public async Task<(int TotalCount, List<FleetListDto> Result)> GetFleets(
        FleetListOptions list, int start, int take, CancellationToken cancellationToken)
    {
        var fleets = context.Fleets.AsNoTracking()
            .AsQueryable();

        switch (list)
        {
            case FleetListOptions.All:
                break;
            default:
                throw new NotSupportedException($"Unknown list option '{list}'.");
        }

        int totalCount = await fleets.CountAsync(cancellationToken);

        var result = await fleets
            .OrderBy(f => f.CompanyCode).ThenBy(f => f.Code)
            .Skip(start)
            .Take(take)
            .Select(x => new FleetListDto(x.CompanyCode, x.Code, x.Name))
            .ToListAsync(cancellationToken);

        return (totalCount, result);

    }
}
