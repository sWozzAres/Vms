namespace Vms.Application.Commands.SupplierUseCase;

public abstract class SupplierTaskBase(VmsDbContext dbContext, IActivityLogger<VmsDbContext> activityLog)
    : IUseCase<VmsDbContext, string>
{
    public VmsDbContext DbContext { get; } = dbContext;
    public IActivityLogger<VmsDbContext> ActivityLog { get; } = activityLog;
    public StringBuilder SummaryText { get; } = new();

    private CancellationToken? _cancellationToken;
    public CancellationToken CancellationToken
        => _cancellationToken ?? throw new InvalidOperationException(NotLoadedMessage);

    private string? _id;
    public string Id => _id ?? throw new InvalidOperationException(NotLoadedMessage);

    private Supplier? _supplier;
    public Supplier? Entity => _supplier;

    private const string NotLoadedMessage = "Supplier is not loaded.";

    public Task LogActivity()
    {
        return SummaryText.Length == 0
            ? Task.CompletedTask
            : ActivityLog.AddAsync(
                _supplier?.Id ?? throw new InvalidOperationException(NotLoadedMessage),
                nameof(Supplier),
                _supplier?.Code ?? throw new InvalidOperationException(NotLoadedMessage),
                SummaryText, CancellationToken
          );
    }

    public async Task<Supplier> Load(string supplierCode, CancellationToken cancellationToken)
    {
        _supplier = await DbContext.Suppliers.FindAsync(new object[] { supplierCode }, cancellationToken)
            ?? throw new InvalidOperationException("Failed to load supplier.");

        _id = supplierCode;
        _cancellationToken = cancellationToken;

        return _supplier;
    }
}
