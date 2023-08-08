namespace Vms.Application.Queries;

public interface IVehicleQueries
{
    Task<VehicleFullDto?> GetVehicleFull(Guid id, CancellationToken cancellationToken);
    Task<(int TotalCount, List<VehicleListDto> Result)> GetVehicles(VehicleListOptions list, int start, int take, CancellationToken cancellationToken);
}

public class VehicleQueries(VmsDbContext context, IUserProvider userProvider) : IVehicleQueries
{
    public async Task<(int TotalCount, List<VehicleListDto> Result)> GetVehicles(
        VehicleListOptions list, int start, int take, CancellationToken cancellationToken)
    {
        var vehicles = context.Vehicles.AsNoTracking()
            .AsQueryable();

        if (list == VehicleListOptions.Following)
        {
            vehicles = from x in vehicles
                       join f in context.Followers on x.Id equals f.DocumentId
                       where f.UserId == userProvider.UserId
                       select x;
        }

        int totalCount = await vehicles.CountAsync(cancellationToken);

        var result = await vehicles
            .Skip(start)
            .Take(take)
            .Select(x => new VehicleListDto(x.Id, x.CompanyCode, x.Vrm, x.Make, x.Model))
            .ToListAsync(cancellationToken);

        return (totalCount, result);
    }

    public async Task<VehicleFullDto?> GetVehicleFull(Guid id,
        CancellationToken cancellationToken)
    {
        var query = from v in context.Vehicles.AsNoTracking()
                        .Include(v => v.C)
                        .Include(v => v.Fleet)
                        .Include(v => v.DriverVehicles).ThenInclude(dv => dv.Driver)
                        .Include(v => v.MotEvents.Where(e => e.IsCurrent))
                    join _f in context.Followers on new { v.Id, userProvider.UserId } equals new { Id = _f.DocumentId, _f.UserId } into __f
                    from f in __f.DefaultIfEmpty()
                    select new { v, IsFollowing = f.UserId != default };

        var vehicle = await query.SingleOrDefaultAsync(v => v.v.Id == id, cancellationToken);

        return vehicle is null ? null : vehicle.v.ToFullDto(vehicle.IsFollowing);
    }
}
