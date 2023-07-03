﻿namespace Vms.Application.UseCase;

public class CreateSupplier
{
    readonly VmsDbContext DbContext;

    public CreateSupplier(VmsDbContext dbContext)
       => DbContext = dbContext;

    public Supplier Create(CreateSupplierRequest request)
    {
        var supplier = new Supplier(request.Code, request.Name, request.Address, request.IsIndependant);
        DbContext.Add(supplier);

        //await DbContext.SaveChangesAsync(cancellationToken);

        return supplier;
    }
}


public record CreateSupplierRequest(string Code, string Name, Address Address, bool IsIndependant);