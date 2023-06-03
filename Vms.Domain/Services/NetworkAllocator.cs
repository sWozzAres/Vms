using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Operation.Distance;
using Vms.Domain.Entity;
using Vms.Domain.Exceptions;
using Vms.Domain.Infrastructure;

namespace Vms.Domain.Services;

public interface INetworkAllocator
{
    Task<List<(string Code, double Distance)>> GetSuppliers(ServiceBooking serviceBooking, CancellationToken cancellationToken);
}

public class NetworkAllocator : INetworkAllocator
{
    readonly VmsDbContext DbContext;

    public NetworkAllocator(VmsDbContext dbContext)
        => DbContext = dbContext;

    public async Task<List<(string Code, double Distance)>> GetSuppliers(ServiceBooking serviceBooking, CancellationToken cancellationToken)
    {
        var vehicle = await DbContext.Vehicles.FindAsync(serviceBooking.VehicleId, cancellationToken) ?? throw new VmsDomainException("Vehicle not found.");

        var query =
            from s in DbContext.Suppliers
            join ns in DbContext.NetworkSuppliers on s.Code equals ns.SupplierCode
            join n in DbContext.Networks on new { ns.CompanyCode, Code = ns.NetworkCode } equals new { n.CompanyCode, n.Code }
            join cs in DbContext.CustomerNetworks on new { n.CompanyCode, n.Code } equals new { cs.CompanyCode, Code = cs.NetworkCode }
            where cs.CompanyCode == vehicle.CompanyCode && cs.CustomerCode == vehicle.CustomerCode &&
                s.Address.Location.Distance(serviceBooking.VehicleLocation) < 50
            select new {
                s.Code,
                Distance = s.Address.Location.Distance(serviceBooking.VehicleLocation)
            };

        var result = await query.ToListAsync(cancellationToken);
        return result.Select(x => (x.Code, x.Distance)).ToList();
    }
}
