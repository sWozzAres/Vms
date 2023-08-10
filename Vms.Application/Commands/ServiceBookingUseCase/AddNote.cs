namespace Vms.Application.Commands.ServiceBookingUseCase;

public interface IAddNote
{
    Task<ActivityLogDto> Add(Guid id, AddNoteDto request, CancellationToken cancellationToken);
}

public class AddNote(VmsDbContext context, IActivityLogger activityLog) : IAddNote
{
    readonly VmsDbContext DbContext = context;
    readonly StringBuilder SummaryText = new();

    public async Task<ActivityLogDto> Add(Guid id, AddNoteDto request, CancellationToken cancellationToken)
    {
        // load to make sure the user has access
        _ = await DbContext.ServiceBookings.FindAsync(new object[] { id }, cancellationToken)
            ?? throw new InvalidOperationException("Failed to load service booking.");

        SummaryText.AppendLine(request.Text);

        var entry = await activityLog.AddAsync(id, SummaryText, cancellationToken);

        return entry.ToDto();
    }
}
