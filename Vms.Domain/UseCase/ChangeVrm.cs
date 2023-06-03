using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vms.Domain.Entity;
using Vms.Domain.Exceptions;
using Vms.Domain.Infrastructure;

namespace Vms.Domain.UseCase;

public class ChangeVrm
{
    readonly VmsDbContext DbContext;
    VehicleRole? Vehicle;

    public ChangeVrm(VmsDbContext context) => DbContext = context;

    public async Task ChangeTo(ChangeVrmRequest request, CancellationToken cancellationToken = default)
    {
        Vehicle = new(await DbContext.Vehicles.FindAsync(request.vehicleId, cancellationToken)
            ?? throw new VmsDomainException("Vehicle not found."), this);

        Vehicle.ChangeVrm(request.newVrm);
    }

    class VehicleRole(Vehicle self, ChangeVrm context)
    {
        public void ChangeVrm(string newVrm) => self.Vrm = newVrm;
    }
}

public record ChangeVrmRequest(Guid vehicleId, string newVrm);
