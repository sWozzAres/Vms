using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure.Core;
using Microsoft.Identity.Client;
using Vms.Application.Services;
using Vms.Domain.Entity;
using Vms.Domain.Exceptions;
using Vms.Domain.Infrastructure;

namespace Vms.Application;

public class AssignSupplier
{
    readonly VmsDbContext DbContext;
    readonly ISupplierLocator Locator;
    ServiceBookingRole? ServiceBooking;

    public AssignSupplier(VmsDbContext dbContext, ISupplierLocator locator)
        => (DbContext, Locator) = (dbContext, locator);

    public async Task<bool> Assign(AssignSupplierRequest request, CancellationToken cancellationToken = default)
    {
        ServiceBooking = new(await DbContext.ServiceBookings.FindAsync(request.ServiceBookingId, cancellationToken)
            ?? throw new VmsDomainException("Service Booking not found."), this);

        return await ServiceBooking.Assign(cancellationToken);
    }
 
    class ServiceBookingRole(ServiceBooking self, AssignSupplier context)
    {
        public async Task<bool> Assign(CancellationToken cancellationToken)
        {
            var list = await context.Locator.GetSuppliers(self, cancellationToken);
            if (!list.Any())
            {
                return false;
            }

            self.AssignSupplier(list.First().Code);
            return true;
        }
    }
}

public record AssignSupplierRequest(Guid ServiceBookingId);