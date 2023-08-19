namespace Vms.Application.Queries;

public class NetworkQueries(
    VmsDbContext context 
    //IUserProvider userProvider
    )
{
    public async Task<(int TotalCount, List<NetworkListDto> Result)> GetNetworks(
        NetworkListOptions list, int start, int take, CancellationToken cancellationToken)
    {
        var networks = context.Networks.AsNoTracking()
            .AsQueryable();

        switch (list)
        {
            case NetworkListOptions.All:
                break;
            default:
                throw new NotSupportedException($"Unknown list option '{list}'.");
        }

        int totalCount = await networks.CountAsync(cancellationToken);

        var result = await networks
            .OrderBy(n => n.CompanyCode).ThenBy(n => n.Code)
            .Skip(start)
            .Take(take)
            .Select(x => new NetworkListDto(x.CompanyCode, x.Code, x.Name))
            .ToListAsync(cancellationToken);

        return (totalCount, result);

    }
}
