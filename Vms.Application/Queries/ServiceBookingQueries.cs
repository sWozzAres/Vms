using Dapper;

namespace Vms.Application.Queries;

public interface IServiceBookingQueries
{
    Task<ServiceBookingFullDto?> GetServiceBookingFull(Guid id, CancellationToken cancellationToken);
    Task<(int TotalCount, List<ServiceBookingListDto> Result)> GetServiceBookings(ServiceBookingListOptions list, int start, int take, CancellationToken cancellationToken);
    Task<IEnumerable<ServiceBookingFullDto>> GetServiceBookingsFullByVehicle(Guid id, CancellationToken cancellationToken);
}

public class ServiceBookingQueries(VmsDbContext context, IUserProvider userProvider) : IServiceBookingQueries
{
    public async Task<(int TotalCount, List<ServiceBookingListDto> Result)> GetServiceBookings(
        ServiceBookingListOptions list, int start, int take,
        CancellationToken cancellationToken)
    {
        var serviceBookings = context.ServiceBookings.AsNoTracking()
            .Include(s => s.Lock)
            .AsQueryable();

        switch (list)
        {
            case ServiceBookingListOptions.All:
                break;
            case ServiceBookingListOptions.Following:
                serviceBookings = from x in serviceBookings
                                  join f in context.Followers on x.Id equals f.DocumentId
                                  where f.UserId == userProvider.UserId
                                  select x;
                break;
            case ServiceBookingListOptions.Assigned:
                serviceBookings = serviceBookings.Where(s => s.AssignedToUserId == userProvider.UserId);
                break;
            case ServiceBookingListOptions.Due:
                serviceBookings = serviceBookings.Where(s => s.RescheduleTime <= DateTime.Now);
                break;
            case ServiceBookingListOptions.Recent:
                serviceBookings = from x in serviceBookings
                                  join f in context.RecentViews on x.Id equals f.DocumentId
                                  where f.UserId == userProvider.UserId
                                  select x;
                break;
            default:
                throw new NotSupportedException($"Unknown list option '{list}'.");
        }

        int totalCount = await serviceBookings.CountAsync(cancellationToken);

        var result = await serviceBookings
            .Include(s => s.Vehicle)
            .OrderBy(s => s.RescheduleTime)
            .Skip(start)
            .Take(take)
            .Where(s => s.Status > 0)
            .Select(x => new ServiceBookingListDto(x.Id, x.VehicleId, x.Ref, x.Vehicle.Vrm,
                x.RescheduleTime, (ServiceBookingDtoStatus)x.Status,
                x.Lock == null ? null : x.Lock.UserName, x.Lock == null ? null : x.Lock.Granted
                ))
            .ToListAsync(cancellationToken);

        return (totalCount, result);
    }
    public async Task<ServiceBookingFullDto?> GetServiceBookingFull(Guid id,
           CancellationToken cancellationToken)
    {
        var serviceBooking = (await context.Database.GetDbConnection().QueryAsync<ServiceBookingFullDto>(
            new CommandDefinition($"{GetServiceBookingQueryText()} AND sb.Id = @id",
                new { id, userId = userProvider.UserId, tenantId = userProvider.TenantId }, cancellationToken: cancellationToken)))
            .SingleOrDefault();

        return serviceBooking;
    }
    public async Task<IEnumerable<ServiceBookingFullDto>> GetServiceBookingsFullByVehicle(Guid id,
        CancellationToken cancellationToken)
    {
        var serviceBookings = await context.Database.GetDbConnection().QueryAsync<ServiceBookingFullDto>(
            new CommandDefinition($"{GetServiceBookingQueryText()} AND v.Id = @id",
                new { id, userId = userProvider.UserId, tenantId = userProvider.TenantId }, cancellationToken: cancellationToken));

        return serviceBookings;
    }
    static string GetServiceBookingQueryText()
    {
        return """
                SELECT sb.Id, sb.VehicleId, sb.CompanyCode, vv.Vrm, v.Make, v.Model, sb.PreferredDate1, sb.PreferredDate2, sb.PreferredDate3, sb.Status, sb.ServiceLevel,
            	    s.Code 'Supplier_Code', s.Name 'Supplier_Name',
            	    mo.Id 'MotEvent_Id', mo.Due 'MotEvent_Due',
            	    CASE 
            		    WHEN EXISTS (SELECT 1 FROM System.Followers f WHERE sb.Id = f.DocumentId AND f.UserId = @userId) THEN 1
            		    ELSE 0
            	    END AS IsFollowing,
                    sb.AssignedToUserId, sb.Ref, sb.RescheduleTime,
                    sbd.Name 'Driver_Name', sbd.EmailAddress 'Driver_EmailAddress', sbd.MobileNumber 'Driver_MobileNumber',
                    sbc.Name 'Contact_Name', sbc.EmailAddress 'Contact_EmailAddress', sbc.MobileNumber 'Contact_MobileNumber',
                    sbl.UserName 'Worker_Name', sbl.Granted 'WorkStarted'
                FROM ServiceBookings sb
                LEFT JOIN ServiceBookingDrivers sbd ON sb.Id = sbd.ServiceBookingId
                LEFT JOIN ServiceBookingContacts sbc ON sb.Id = sbc.ServiceBookingId
                LEFT JOIN ServiceBookingLocks sbl ON sb.Id = sbl.ServiceBookingId
                JOIN Vehicles v ON sb.VehicleId = v.Id
                JOIN VehicleVrms vv ON v.Id = vv.VehicleId
                LEFT JOIN Suppliers s ON sb.SupplierCode = s.Code
                LEFT JOIN MotEvents mo ON sb.MotEventId = mo.Id
                WHERE (@tenantId = '*' OR @tenantId = sb.CompanyCode)
            """;
    }
}
