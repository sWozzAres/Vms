using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Vms.Application.UseCase;

public class RemoveDriverFromVehicle
{
    readonly VmsDbContext DbContext;

    public RemoveDriverFromVehicle(VmsDbContext dbContext)
        => DbContext = dbContext;

    public async Task<bool> RemoveAsync(Guid id, string emailAddress, CancellationToken cancellationToken = default)
    {
        var entry = await DbContext.DriverVehicles.FindAsync(new object[] { emailAddress, id }, cancellationToken);
        if (entry is null)
        {
            return false;
        }

        DbContext.DriverVehicles.Remove(entry);
        await DbContext.SaveChangesAsync(cancellationToken);

        return true;
    }
}

//public record RemoveDriverFromVehicle(bool Result)