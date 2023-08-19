namespace Vms.Application.Commands.VehicleUseCase;

public class AddNoteVehicle(VmsDbContext context, IActivityLogger<VmsDbContext> activityLog)
{
    readonly VmsDbContext DbContext = context;
    readonly StringBuilder SummaryText = new();

    public async Task<ActivityLogDto> Add(Guid id, AddNoteDto request, CancellationToken cancellationToken)
    {
        // load to make sure the user has access
        var vehicle = await DbContext.Vehicles.FindAsync(new object[] { id }, cancellationToken)
            ?? throw new InvalidOperationException("Failed to load vehicle.");

        SummaryText.AppendLine(request.Text);

        var entry = await activityLog.AddNoteAsync(id, nameof(Vehicle), vehicle.Vrm,
            SummaryText, cancellationToken);

        return entry.ToDto();
    }
}
