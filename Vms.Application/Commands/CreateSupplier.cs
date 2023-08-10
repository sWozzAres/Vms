namespace Vms.Application.Commands;

public class CreateSupplier(VmsDbContext dbContext, ISearchManager searchManager, ILogger logger)
{
    readonly VmsDbContext DbContext = dbContext;

    public Supplier Create(CreateSupplierRequest request)
    {
        logger.LogInformation("Creating supplier {suppliercode} {suppliername}", request.Code, request.Name);

        var supplier = new Supplier(request.Code, request.Name, request.Address, request.IsIndependant);
        DbContext.Add(supplier);

        searchManager.Add(null, supplier.Code, EntityKind.Supplier, supplier.Name,
            string.Join(" ", supplier.Code, supplier.Name));

        return supplier;
    }
}


public record CreateSupplierRequest(string Code, string Name, Address Address, bool IsIndependant);
