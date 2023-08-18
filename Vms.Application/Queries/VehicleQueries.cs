namespace Vms.Application.Queries;

public interface IVehicleQueries
{
    Task<ActivityLogDto?> GetActivity(Guid id, Guid activityId, CancellationToken cancellationToken);
    Task<VehicleFullDto?> GetVehicleFull(Guid id, CancellationToken cancellationToken);
    Task<(int TotalCount, List<VehicleListDto> Result)> GetVehicles(VehicleListOptions list, int start, int take, CancellationToken cancellationToken);
}

public class VehicleQueries(VmsDbContext context,
    IUserProvider userProvider,
    ITimeService timeService) : IVehicleQueries
{
    public async Task<ActivityLogDto?> GetActivity(Guid id, Guid activityId,
        CancellationToken cancellationToken)
    {
        var activityLog = await (from sb in context.Vehicles
                                 join ac in context.ActivityLog on sb.Id equals ac.DocumentId
                                 where sb.Id == id && ac.DocumentId == activityId
                                 select ac).SingleOrDefaultAsync(cancellationToken);

        return activityLog?.ToDto();
    }
    public async Task<(int TotalCount, List<VehicleListDto> Result)> GetVehicles(
        VehicleListOptions list, int start, int take, CancellationToken cancellationToken)
    {
        var vehicles = context.Vehicles.AsNoTracking()
            .AsQueryable();

        if (list == VehicleListOptions.All || list == VehicleListOptions.Following || list == VehicleListOptions.Recent)
        {
            switch (list)
            {
                case VehicleListOptions.All:
                    vehicles = from v in vehicles
                               orderby v.Id
                               select v;
                    break;
                case VehicleListOptions.Following:
                    vehicles = from x in vehicles
                               join f in context.Followers on x.Id equals f.DocumentId
                               where f.UserId == userProvider.UserId
                               orderby f.Id
                               select x;
                    break;
                case VehicleListOptions.Recent:
                    vehicles = from x in vehicles
                               join f in context.RecentViews on x.Id equals f.DocumentId
                               where f.UserId == userProvider.UserId
                               orderby f.ViewDate descending
                               select x;
                    break;
                default:
                    throw new NotSupportedException($"Unknown list option '{list}'.");
            }

            int totalCount = await vehicles.CountAsync(cancellationToken);

            var result = await vehicles
                .Skip(start)
                .Take(take)
                .Select(x => new VehicleListDto(
                    x.Id,
                    x.CompanyCode,
                    x.VehicleVrm.Vrm,
                    x.Make,
                    x.Model,
                    x.Customer == null ? null : x.Customer.Code,
                    x.Customer == null ? null : x.Customer.Name,
                    x.Fleet == null ? null : x.Fleet.Code,
                    x.Fleet == null ? null : x.Fleet.Name,
                    null
                ))
                .ToListAsync(cancellationToken);

            return (totalCount, result);
        }
        else if (list == VehicleListOptions.DueMot)
        {
            var todayPlus30 = DateOnly.FromDateTime(timeService.GetTime()).AddDays(30);
            var vehiclesWithMot = from v in vehicles
                                  join me in context.MotEvents on v.Id equals me.VehicleId
                                  //where me.Due <= todayPlus30 && !context.MotEvents.Any(m => v.Id == m.VehicleId && m.IsCurrent && m.ServiceBookingId != null)
                                  where me.Due <= todayPlus30 && me.IsCurrent && me.ServiceBookingId == null
                                  orderby me.Due
                                  select new { Vehicle = v, MotEvent = me };
            int totalCount = await vehicles.CountAsync(cancellationToken);

            var result = await vehiclesWithMot
                .Skip(start)
                .Take(take)
                .Select(x => new VehicleListDto(
                    x.Vehicle.Id,
                    x.Vehicle.CompanyCode,
                    x.Vehicle.VehicleVrm.Vrm,
                    x.Vehicle.Make,
                    x.Vehicle.Model,
                    x.Vehicle.Customer == null ? null : x.Vehicle.Customer.Code,
                    x.Vehicle.Customer == null ? null : x.Vehicle.Customer.Name,
                    x.Vehicle.Fleet == null ? null : x.Vehicle.Fleet.Code,
                    x.Vehicle.Fleet == null ? null : x.Vehicle.Fleet.Name,
                    x.MotEvent.Due
                ))
                .ToListAsync(cancellationToken);

            return (totalCount, result);
        }
        else
            throw new NotSupportedException($"Unknown list option '{list}'.");
    }

    public async Task<VehicleFullDto?> GetVehicleFull(Guid id,
        CancellationToken cancellationToken)
    {
#pragma warning disable IDE0031 // Use null propagation
        var query = from v in context.Vehicles.AsNoTracking()
                        .Include(v => v.Customer)
                        .Include(v => v.Fleet)
                        .Include(v => v.DriverVehicles).ThenInclude(dv => dv.Driver)
                        //.Include(v => v.MotEvents.Where(e => e.IsCurrent))

                    join __me in context.MotEvents on new { v.Id, IsCurrent = true } equals new { Id = __me.VehicleId, __me.IsCurrent } into _me
                    from me in _me.DefaultIfEmpty()

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
                        MotDue = me.Due == default ? (DateOnly?)null : me.Due,

                        v.Address.Street,
                        v.Address.Locality,
                        v.Address.Town,
                        v.Address.Postcode,
                        v.Address.Location.Coordinate.Y,
                        v.Address.Location.Coordinate.X,

                        Customer_CompanyCode = v.Customer == null ? null : v.Customer.CompanyCode,
                        Customer_Code = v.Customer == null ? null : v.Customer.Code,
                        Customer_Name = v.Customer == null ? null : v.Customer.Name,

                        Fleet_CompanyCode = v.Fleet == null ? null : v.Fleet.CompanyCode,
                        Fleet_Code = v.Fleet == null ? null : v.Fleet.Code,
                        Fleet_Name = v.Fleet == null ? null : v.Fleet.Name,

                        Drivers = v.DriverVehicles.Select(x => x.Driver),
                        IsFollowing = f.UserId != default
                    };
#pragma warning restore IDE0031 // Use null propagation

        var vehicle = await query.SingleOrDefaultAsync(v => v.Id == id, cancellationToken);
        if (vehicle is null)
        {
            return null;
        }

        var dto = new VehicleFullDto(vehicle.CompanyCode, vehicle.Id, vehicle.Vrm, vehicle.Make, vehicle.Model,
            vehicle.ChassisNumber, vehicle.DateFirstRegistered, vehicle.MotDue,
            new(vehicle.Street, vehicle.Locality, vehicle.Town, vehicle.Postcode, new(vehicle.Y, vehicle.X)),
            vehicle.Customer_Code == null ? null : new(vehicle.Customer_CompanyCode, vehicle.Customer_Code, vehicle.Customer_Name),
            vehicle.Fleet_Code == null ? null : new(vehicle.Fleet_CompanyCode, vehicle.Fleet_Code, vehicle.Fleet_Name),
            vehicle.Drivers.Select(d => d.ToShortDto()).ToList(),
            vehicle.IsFollowing);

        return dto;//.ToFullDto(vehicle.IsFollowing);
    }
}
