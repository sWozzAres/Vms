﻿namespace Vms.Application.Commands.SupplierUseCase;

public class AddNoteSupplier(VmsDbContext context, IActivityLogger<VmsDbContext> activityLog)
{
    readonly VmsDbContext DbContext = context;
    readonly StringBuilder SummaryText = new();

    public async Task<ActivityLogDto> AddAsync(Guid id, AddNoteDto request, CancellationToken cancellationToken)
    {
        var supplier = await DbContext.Suppliers.SingleOrDefaultAsync(s => s.Id == id, cancellationToken)
            ?? throw new InvalidOperationException("Failed to load supplier.");

        SummaryText.AppendLine(request.Text);

        var entry = await activityLog.AddAsync(id, nameof(Supplier), supplier.Code,
            SummaryText, cancellationToken);

        return entry.ToDto();
    }
}
