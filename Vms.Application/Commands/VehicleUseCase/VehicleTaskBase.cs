namespace Vms.Application.Commands.VehicleUseCase;

public abstract class VehicleTaskBase(VmsDbContext dbContext, IActivityLogger<VmsDbContext> activityLog)
    : IUseCase<VmsDbContext, Guid>
{
    public VmsDbContext DbContext { get; } = dbContext;
    public IActivityLogger<VmsDbContext> ActivityLog { get; } = activityLog;
    public StringBuilder SummaryText { get; } = new();

    private CancellationToken? _cancellationToken;
    public CancellationToken CancellationToken
        => _cancellationToken ?? throw new InvalidOperationException(NotLoadedMessage);

    private Guid? _id;
    public Guid Id => _id ?? throw new InvalidOperationException(NotLoadedMessage);

    private Vehicle? _vehicle;
    public Vehicle? Entity => _vehicle;

    private const string NotLoadedMessage = "Vehicle is not loaded.";

    public Task LogActivity()
    {
        return SummaryText.Length == 0
            ? Task.CompletedTask
            : ActivityLog.AddAsync(
                Id, nameof(Vehicle),
                _vehicle?.Vrm ?? throw new InvalidOperationException(NotLoadedMessage),
                SummaryText, CancellationToken
          );
    }

    public async Task<Vehicle> Load(Guid vehicleId, CancellationToken cancellationToken)
    {
        _vehicle = await DbContext.Vehicles.FindAsync(new object[] { vehicleId }, cancellationToken)
            ?? throw new InvalidOperationException("Failed to load vehicle.");

        _id = vehicleId;
        _cancellationToken = cancellationToken;

        return _vehicle;
    }
}
