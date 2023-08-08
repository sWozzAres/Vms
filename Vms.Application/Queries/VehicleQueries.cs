using Vms.Domain.Core;

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
            .Select(x => new VehicleListDto(x.Id, x.CompanyCode, x.VehicleVrm.Vrm, x.Make, x.Model))
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
                    select new
                    {
                        v.CompanyCode,
                        v.Id,
                        v.VehicleVrm.Vrm,
                        v.Make,
                        v.Model,
                        v.ChassisNumber,
                        v.DateFirstRegistered,
                        MotDue = v.Mot.Due,

                        v.Address.Street,
                        v.Address.Locality,
                        v.Address.Town,
                        v.Address.Postcode,
                        v.Address.Location.Coordinate.X,
                        v.Address.Location.Coordinate.Y,

                        Customer_CompanyCode = v.C == null ? null : v.C.CompanyCode,
                        Customer_Code = v.C == null ? null : v.C.Code,
                        Customer_Name = v.C == null ? null : v.C.Name,

                        Fleet_CompanyCode = v.Fleet == null ? null : v.Fleet.CompanyCode,
                        Fleet_Code = v.Fleet == null ? null : v.Fleet.Code,
                        Fleet_Name = v.Fleet == null ? null : v.Fleet.Name,

                        Drivers = v.DriverVehicles.Select(x => x.Driver),
                        IsFollowing = f.UserId != default
                    };

        var vehicle = await query.SingleOrDefaultAsync(v => v.Id == id, cancellationToken);
        if (vehicle is null)
        {
            return null;
        }

        var dto = new VehicleFullDto(vehicle.CompanyCode, vehicle.Id, vehicle.Vrm, vehicle.Make, vehicle.Model,
            vehicle.ChassisNumber, vehicle.DateFirstRegistered, vehicle.MotDue,
            new(vehicle.Street, vehicle.Locality, vehicle.Town, vehicle.Postcode, new(vehicle.X, vehicle.Y)),
            new(vehicle.Customer_CompanyCode, vehicle.Customer_Code, vehicle.Customer_Name),
            new(vehicle.Fleet_CompanyCode, vehicle.Fleet_Code, vehicle.Fleet_Name),
            vehicle.Drivers.Select(d => d.ToShortDto()).ToList(),
            vehicle.IsFollowing);

        return dto;//.ToFullDto(vehicle.IsFollowing);
    }
}
