namespace Vms.Application.Commands.VehicleUseCase;

public interface IAddNoteVehicle
{
    Task<ActivityLogDto> Add(Guid id, AddNoteDto request, CancellationToken cancellationToken);
}

public class AddNoteVehicle(VmsDbContext context, IActivityLogger activityLog) : IAddNoteVehicle
{
    readonly VmsDbContext DbContext = context;
    readonly StringBuilder SummaryText = new();

    public async Task<ActivityLogDto> Add(Guid id, AddNoteDto request, CancellationToken cancellationToken)
    {
        // load to make sure the user has access
        _ = await DbContext.Vehicles.FindAsync(new object[] { id }, cancellationToken)
            ?? throw new InvalidOperationException("Failed to load vehicle.");

        SummaryText.AppendLine(request.Text);

        var entry = await activityLog.AddAsync(id, SummaryText, cancellationToken);

        return entry.ToDto();
    }
}
