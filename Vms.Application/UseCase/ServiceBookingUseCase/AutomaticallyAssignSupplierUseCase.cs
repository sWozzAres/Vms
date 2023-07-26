﻿using Vms.Application.Services;
using Vms.Domain.Entity.ServiceBookingEntity;
using Vms.Domain.Services;
using Vms.Web.Shared;

namespace Vms.Application.UseCase.ServiceBookingUseCase;

public interface IAutomaticallyAssignSupplierUseCase
{
    Task<bool> Assign(Guid id, CancellationToken cancellationToken = default);
}

public class AutomaticallyAssignSupplierUseCase(VmsDbContext dbContext, ISupplierLocator locator, IUserProvider userProvider) : IAutomaticallyAssignSupplierUseCase
{
    readonly VmsDbContext DbContext = dbContext;
    readonly IUserProvider UserProvider = userProvider;
    readonly ISupplierLocator Locator = locator;
    ServiceBookingRole? ServiceBooking;

    public async Task<bool> Assign(Guid id, CancellationToken cancellationToken = default)
    {
        ServiceBooking = new(await DbContext.ServiceBookings.FindAsync(id, cancellationToken)
            ?? throw new VmsDomainException("Service Booking not found."), this);

        return await ServiceBooking.AutoAssign(cancellationToken);
    }

    class ServiceBookingRole(ServiceBooking self, AutomaticallyAssignSupplierUseCase ctx)
    {
        public async Task<bool> AutoAssign(CancellationToken cancellationToken)
        {
            var list = await ctx.Locator.GetSuppliers(self, cancellationToken);
            if (!list.Any())
            {
                return false;
            }

            //self.SupplierCode = list.First().Code;
            await new AssignSupplierUseCase(ctx.DbContext, ctx.UserProvider)
                .AssignAsync(self.Id, new TaskAssignSupplierCommand() { SupplierCode = list.First().Code }, cancellationToken);
            return true;
        }
    }
}
