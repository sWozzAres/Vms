namespace Vms.Application.Commands.SupplierUseCase;

public interface IAssignSupplierToNetwork
{
    Task AssignAsync(string code, string companyCode, string networkCode, CancellationToken cancellationToken);
}

public class AssignSupplierToNetwork(VmsDbContext dbContext) : IAssignSupplierToNetwork
{
    readonly VmsDbContext DbContext = dbContext;
    CancellationToken CancellationToken;
    SupplierRole? Supplier;
    public async Task AssignAsync(string code, string companyCode, string networkCode, CancellationToken cancellationToken = default)
    {
        CancellationToken = cancellationToken;

        Supplier = new(await DbContext.Suppliers.FindAsync(new object[] { code }, cancellationToken)
            ?? throw new InvalidOperationException("Failed to load supplier."), this);

        var assigned = await Supplier.AssignAsync(companyCode, networkCode);

        // TODO log activity etc
    }

    class SupplierRole(Supplier self, AssignSupplierToNetwork ctx)
    {
        public async Task<bool> AssignAsync(string companyCode, string networkCode)
        {
            //var ns = await ctx.DbContext.NetworkSuppliers.FindAsync(new object[] { companyCode, networkCode, self.Code }, ctx.CancellationToken);
            //if (ns is null) 
            {
                ctx.DbContext.NetworkSuppliers.Add(new NetworkSupplier(self.Code, companyCode, networkCode));
                return true;
            }

            return false;
        }
    }
}
