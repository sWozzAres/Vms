using Vms.Domain.ServiceBookingProcess;

namespace Vms.Application.Services;

public class SupplierLocator(VmsDbContext dbContext)
{
    public async Task<IEnumerable<SupplierDistance>> GetSuppliers(ServiceBooking serviceBooking, string? filter, CancellationToken cancellationToken = default)
    {
        var vehicle = await dbContext.Vehicles.FindAsync(new object[] { serviceBooking.VehicleId }, cancellationToken)
            ?? throw new VmsDomainException("Vehicle not found.");

        var result = await GetSupplierDistances(vehicle, serviceBooking.Id, filter, cancellationToken);

        return result;
    }

    async Task<List<SupplierDistance>> GetSupplierDistances(Vehicle vehicle, Guid serviceBookingId, string? filter,
        CancellationToken cancellationToken)
    {
        var customerList = vehicle.CustomerCode is null
            ? Enumerable.Empty<SupplierDistance>().ToList()
            : await SuppliersOnCustomerNetwork(vehicle.Address.Location,
            serviceBookingId,
            filter,
            (vehicle.CompanyCode, vehicle.CustomerCode)).ToListAsync(cancellationToken);

        var fleetList = vehicle.FleetCode is null
            ? Enumerable.Empty<SupplierDistance>().ToList()
            : await SuppliersOnFleetNetwork(vehicle.Address.Location,
            serviceBookingId,
            filter,
            (vehicle.CompanyCode, vehicle.FleetCode)).ToListAsync(cancellationToken);

        //const double MetresInMile = 1609.344d;

        var result = customerList.Union(fleetList)
            //.Select(s => new SupplierDistance(s.Code, s.Name, s.Address.Location.Distance(vehicle.Address.Location) / MetresInMile))
            .Distinct()
            .OrderBy(s => s.Distance)
            .ToList();
        //.ToListAsync(cancellationToken);

        return result;
    }

    private IQueryable<SupplierDistance> SuppliersOnCustomerNetwork(Geometry location,
        Guid serviceBookingId,
        string? filter,
        (string CompanyCode, string CustomerCode) customer)
        => from s in dbContext.Suppliers
           join ns in dbContext.NetworkSuppliers on s.Code equals ns.SupplierCode
           join n in dbContext.Networks on new { ns.CompanyCode, Code = ns.NetworkCode } equals new { n.CompanyCode, n.Code }
           join cs in dbContext.CustomerNetworks on new { n.CompanyCode, n.Code } equals new { cs.CompanyCode, Code = cs.NetworkCode }

           join _sr in dbContext.SupplierRefusals.OrderByDescending(x => x.Id).Where(r => r.ServiceBookingId == serviceBookingId)
           on s.Code equals _sr.SupplierCode into __sr
           from sr in __sr.Take(1).DefaultIfEmpty()

           where (cs.CompanyCode == customer.CompanyCode && cs.CustomerCode == customer.CustomerCode) && (filter == null || s.Name.StartsWith(filter))
           //&& s.Address.Location.Distance(serviceBooking.VehicleLocation) < 50
           orderby s.Address.Location.Distance(location)
           select new SupplierDistance(s.Code, s.Name, s.Address.Location.Distance(location), sr.Code, sr.Name);

    private IQueryable<SupplierDistance> SuppliersOnFleetNetwork(Geometry location,
        Guid serviceBookingId,
        string? filter,
        (string CompanyCode, string FleetCode) fleet)
        => from s in dbContext.Suppliers
           join ns in dbContext.NetworkSuppliers on s.Code equals ns.SupplierCode
           join n in dbContext.Networks on new { ns.CompanyCode, Code = ns.NetworkCode } equals new { n.CompanyCode, n.Code }
           join fs in dbContext.FleetNetworks on new { n.CompanyCode, n.Code } equals new { fs.CompanyCode, Code = fs.NetworkCode }

           join _sr in dbContext.SupplierRefusals.OrderByDescending(x => x.Id).Where(r => r.ServiceBookingId == serviceBookingId)
           on s.Code equals _sr.SupplierCode into __sr
           from sr in __sr.Take(1).DefaultIfEmpty()

           where (fs.CompanyCode == fleet.CompanyCode && fs.FleetCode == fleet.FleetCode) && (filter == null || s.Name.StartsWith(filter))
           //&& s.Address.Location.Distance(serviceBooking.VehicleLocation) < 50
           orderby s.Address.Location.Distance(location)
           select new SupplierDistance(s.Code, s.Name, s.Address.Location.Distance(location), sr.Code, sr.Name);
}

public record SupplierDistance(string Code, string Name, double Distance, string? RefusalCode, string? RefusalName);
