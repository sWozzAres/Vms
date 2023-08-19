namespace Vms.Application.Commands;

public interface IUseCase<TDbContext, TId> where TDbContext : DbContext
{
    VmsDbContext DbContext { get; }
    StringBuilder SummaryText { get; }
    CancellationToken CancellationToken { get; }
    TId Id { get; }
}
