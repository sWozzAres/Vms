using Vms.Domain.ServiceBookingProcess;

namespace Vms.Application.Commands.ServiceBookingUseCase;

public interface IAddNoteServiceBooking
{
    Task<ActivityLogDto> Add(Guid serviceBookingId, AddNoteDto request, CancellationToken cancellationToken);
}

public class AddNoteServiceBooking(VmsDbContext context, IActivityLogger<VmsDbContext> activityLog) : IAddNoteServiceBooking
{
    readonly VmsDbContext DbContext = context;
    readonly StringBuilder SummaryText = new();

    public async Task<ActivityLogDto> Add(Guid serviceBookingId, AddNoteDto request, CancellationToken cancellationToken)
    {
        // load to make sure the user has access
        var serviceBooking = await DbContext.ServiceBookings.FindAsync(new object[] { serviceBookingId }, cancellationToken)
            ?? throw new InvalidOperationException("Failed to load service booking.");

        SummaryText.AppendLine(request.Text);

        var entry = await activityLog.AddNoteAsync(serviceBookingId, nameof(ServiceBooking), serviceBooking.Ref,
            SummaryText, cancellationToken);

        return entry.ToDto();
    }
}
