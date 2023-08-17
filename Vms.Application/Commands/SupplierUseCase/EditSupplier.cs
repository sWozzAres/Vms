namespace Vms.Application.Commands.SupplierUseCase;

public interface IEditSupplier
{
    Task<bool> EditAsync(string code, SupplierDto command, CancellationToken cancellationToken);
}

public class EditSupplier(VmsDbContext dbContext, IActivityLogger<VmsDbContext> activityLog,
    ITaskLogger<VmsDbContext> taskLogger, ILogger<EditSupplier> logger) : IEditSupplier
{
    readonly VmsDbContext DbContext = dbContext;
    readonly StringBuilder SummaryText = new();

    public async Task<bool> EditAsync(string code, SupplierDto command, CancellationToken cancellationToken)
    {
        logger.LogInformation("Editing supplier: {suppliercode}, command: {@supplierdto}.", code, command);

        var supplier = await DbContext.Suppliers.FindAsync(new object[] { code }, cancellationToken)
            ?? throw new InvalidOperationException("Failed to load supplier.");

        SummaryText.AppendLine("# Edit");

        bool isModified = false;

        if (supplier.Code != command.Code)
        {
            SummaryText.AppendLine($"* Code: {command.Code}");
            supplier.Code = command.Code;
            isModified = true;
        }
        if (supplier.Name != command.Name)
        {
            SummaryText.AppendLine($"* Name: {command.Name}");
            supplier.Name = command.Name;
            isModified = true;
        }
        if (supplier.IsIndependent != command.IsIndependent)
        {
            SummaryText.AppendLine($"* Is Independant: {command.IsIndependent.YesNo()}");
            supplier.IsIndependent = command.IsIndependent;
            isModified = true;
        }

        bool addressModified = false;
        if (supplier.Address.Street != command.Address.Street)
        {
            SummaryText.AppendLine($"* Street: {command.Address.Street}");
            addressModified = true;
        }
        if (supplier.Address.Locality != command.Address.Locality)
        {
            SummaryText.AppendLine($"* Locality: {command.Address.Locality}");
            addressModified = true;
        }
        if (supplier.Address.Town != command.Address.Town)
        {
            SummaryText.AppendLine($"* Town: {command.Address.Town}");
            addressModified = true;
        }
        if (supplier.Address.Postcode != command.Address.Postcode)
        {
            SummaryText.AppendLine($"* Postcode: {command.Address.Postcode}");
            addressModified = true;
        }
        if (supplier.Address.Location.Coordinate.Y != command.Address.Location.Latitude)
        {
            SummaryText.AppendLine($"* Latitude: {command.Address.Location.Latitude}");
            addressModified = true;
        }
        if (supplier.Address.Location.Coordinate.X != command.Address.Location.Longitude)
        {
            SummaryText.AppendLine($"* Longitude: {command.Address.Location.Longitude}");
            addressModified = true;
        }
        if (addressModified)
        {
            supplier.Address = new Address(command.Address.Street, command.Address.Locality, command.Address.Town, command.Address.Postcode,
                new Point(command.Address.Location.Longitude, command.Address.Location.Latitude) { SRID = 4326 });

            isModified = true;
        }

        if (isModified)
        {
            _ = await activityLog.AddAsync(supplier.Id, SummaryText, cancellationToken);
            taskLogger.Log(supplier.Id, nameof(EditSupplier), command);
        }

        return isModified;
    }
}