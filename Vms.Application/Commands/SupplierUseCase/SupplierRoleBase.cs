namespace Vms.Application.Commands.VehicleUseCase;

public abstract class SupplierRoleBase<TContext>(Supplier self, TContext ctx)
    where TContext : IUseCase<VmsDbContext, string>
{
    protected Supplier Self { get; private set; } = self;
    protected TContext Ctx { get; private set; } = ctx;
    public Supplier Entity => Self;
}