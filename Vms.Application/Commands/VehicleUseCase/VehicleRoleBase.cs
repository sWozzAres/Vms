namespace Vms.Application.Commands.VehicleUseCase;

public abstract class VehicleRoleBase<TContext>(Vehicle self, TContext ctx)
    where TContext : IUseCase<VmsDbContext, Guid>
{
    protected Vehicle Self { get; private set; } = self;
    protected TContext Ctx { get; private set; } = ctx;
    public Vehicle Entity => Self;
}