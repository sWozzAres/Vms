using NetTopologySuite.Geometries;
using Vms.Domain.Entity.ServiceBookingEntity;

namespace Vms.Application.Services;

public interface ISupplierLocator
{
    Task<IEnumerable<(string Code, string Name, double Distance)>> GetSuppliers(ServiceBooking serviceBooking, CancellationToken cancellationToken);
}

public class SupplierLocator(VmsDbContext dbContext) : ISupplierLocator
{
    public async Task<IEnumerable<(string Code, string Name, double Distance)>> GetSuppliers(ServiceBooking serviceBooking, CancellationToken cancellationToken = default)
    {
        var vehicle = await dbContext.Vehicles.FindAsync(new object[] { serviceBooking.VehicleId }, cancellationToken)
            ?? throw new VmsDomainException("Vehicle not found.");

        var result = await GetSupplierDistances(vehicle, cancellationToken);

        return result.Select(x => (x.Code, x.Name, x.Distance));
    }

    async Task<List<SupplierDistance>> GetSupplierDistances(Vehicle vehicle, CancellationToken cancellationToken)
    {
        var customerList = vehicle.CustomerCode is null
            ? Enumerable.Empty<SupplierDistance>().ToList()
            : await SuppliersOnCustomerNetwork(vehicle.Address.Location, (vehicle.CompanyCode, vehicle.CustomerCode)).ToListAsync(cancellationToken);

        var fleetList = vehicle.FleetCode is null
            ? Enumerable.Empty<SupplierDistance>().ToList()
            : await SuppliersOnFleetNetwork(vehicle.Address.Location, (vehicle.CompanyCode, vehicle.FleetCode)).ToListAsync(cancellationToken);

        //const double MetresInMile = 1609.344d;

        var result = customerList.Union(fleetList)
            //.Select(s => new SupplierDistance(s.Code, s.Name, s.Address.Location.Distance(vehicle.Address.Location) / MetresInMile))
            .Distinct()
            .OrderBy(s => s.Distance)
            .ToList();
            //.ToListAsync(cancellationToken);

        return result;
    }

    private IQueryable<SupplierDistance> SuppliersOnCustomerNetwork(Geometry location, (string CompanyCode, string CustomerCode) customer)
        => from s in dbContext.Suppliers
           join ns in dbContext.NetworkSuppliers on s.Code equals ns.SupplierCode
           join n in dbContext.Networks on new { ns.CompanyCode, Code = ns.NetworkCode } equals new { n.CompanyCode, n.Code }
           join cs in dbContext.CustomerNetworks on new { n.CompanyCode, n.Code } equals new { cs.CompanyCode, Code = cs.NetworkCode }
           where cs.CompanyCode == customer.CompanyCode && cs.CustomerCode == customer.CustomerCode
           //&& s.Address.Location.Distance(serviceBooking.VehicleLocation) < 50
           orderby s.Address.Location.Distance(location)
           select new SupplierDistance(s.Code, s.Name, s.Address.Location.Distance(location));

    private IQueryable<SupplierDistance> SuppliersOnFleetNetwork(Geometry location, (string CompanyCode, string FleetCode) fleet)
        => from s in dbContext.Suppliers
           join ns in dbContext.NetworkSuppliers on s.Code equals ns.SupplierCode
           join n in dbContext.Networks on new { ns.CompanyCode, Code = ns.NetworkCode } equals new { n.CompanyCode, n.Code }
           join fs in dbContext.FleetNetworks on new { n.CompanyCode, n.Code } equals new { fs.CompanyCode, Code = fs.NetworkCode }
           where fs.CompanyCode == fleet.CompanyCode && fs.FleetCode == fleet.FleetCode
           //&& s.Address.Location.Distance(serviceBooking.VehicleLocation) < 50
           orderby s.Address.Location.Distance(location)
           select new SupplierDistance(s.Code, s.Name, s.Address.Location.Distance(location));

    record SupplierDistance(string Code, string Name, double Distance);
}
