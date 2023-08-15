namespace Vms.Application.Commands.SupplierUseCase;

public interface IAddNoteSupplier
{
    Task<ActivityLogDto> AddAsync(Guid id, AddNoteDto request, CancellationToken cancellationToken);
}

public class AddNoteSupplier(VmsDbContext context, IActivityLogger activityLog) : IAddNoteSupplier
{
    readonly VmsDbContext DbContext = context;
    readonly StringBuilder SummaryText = new();

    public async Task<ActivityLogDto> AddAsync(Guid id, AddNoteDto request, CancellationToken cancellationToken)
    {
        var supplier = await DbContext.Suppliers.SingleOrDefaultAsync(s => s.Id == id, cancellationToken)
            ?? throw new InvalidOperationException("Failed to load supplier.");

        SummaryText.AppendLine(request.Text);

        var entry = await activityLog.AddAsync(id, SummaryText, cancellationToken);

        return entry.ToDto();
    }
}
