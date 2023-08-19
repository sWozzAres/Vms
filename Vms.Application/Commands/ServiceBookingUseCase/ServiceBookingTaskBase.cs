using Vms.Domain.ServiceBookingProcess;

namespace Vms.Application.Commands.ServiceBookingUseCase;

public abstract class ServiceBookingTaskBase(VmsDbContext dbContext,
    IActivityLogger<VmsDbContext> activityLog)
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

    private ServiceBooking? _serviceBooking;
    public ServiceBooking? Entity => _serviceBooking;

    private const string NotLoadedMessage = "ServiceBooking is not loaded.";

    public Task LogActivity()
    {
        return SummaryText.Length == 0
            ? Task.CompletedTask
            : ActivityLog.AddAsync(
                Id, nameof(ServiceBooking),
                _serviceBooking?.Ref ?? throw new InvalidOperationException(NotLoadedMessage),
                SummaryText, CancellationToken
          );
    }

    public async Task<ServiceBooking> Load(Guid serviceBookingId, CancellationToken cancellationToken)
    {
        _serviceBooking = await DbContext.ServiceBookings.FindAsync(new object[] { serviceBookingId }, cancellationToken)
            ?? throw new InvalidOperationException("Failed to load service booking.");

        _id = serviceBookingId;
        _cancellationToken = cancellationToken;

        return _serviceBooking;
    }
}
