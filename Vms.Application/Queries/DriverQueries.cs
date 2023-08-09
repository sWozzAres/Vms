namespace Vms.Application.Queries;

public interface IDriverQueries
{
    Task<(int TotalCount, List<DriverListDto> Result)> GetDrivers(DriverListOptions list, int start, int take, CancellationToken cancellationToken);
}

public class DriverQueries(VmsDbContext context, IUserProvider userProvider) : IDriverQueries
{
    public async Task<(int TotalCount, List<DriverListDto> Result)> GetDrivers(
        DriverListOptions list, int start, int take, CancellationToken cancellationToken)
    {
        var drivers = context.Drivers.AsNoTracking()
            .AsQueryable();

        switch (list)
        {
            case DriverListOptions.All:
                break;
            default:
                throw new NotSupportedException($"Unknown list option '{list}'.");
        }

        int totalCount = await drivers.CountAsync(cancellationToken);

        var query = await drivers
            .Skip(start)
            .Take(take)
            .Select(x => new { x.Id, x.Salutation, x.FirstName, x.MiddleNames, x.LastName })
            .ToListAsync(cancellationToken);

        var result = query.Select(q => new DriverListDto(q.Id, 
            string.Join(" ", new string?[] { q.Salutation, q.FirstName, q.MiddleNames, q.LastName }
                .Where(x => !string.IsNullOrEmpty(x))))).ToList();

        return (totalCount, result);

    }
}
