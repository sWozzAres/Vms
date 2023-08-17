using Vms.Application.Commands.ServiceBookingUseCase;

namespace Vms.Application.Commands.SupplierUseCase;

public interface ICreateSupplier
{
    Task<Supplier> CreateAsync(CreateSupplierRequest command, CancellationToken cancellationToken = default);
}

public class CreateSupplier(VmsDbContext dbContext, ISearchManager searchManager,
    ILogger<CreateSupplier> logger,
    IActivityLogger<VmsDbContext> activityLog,
    ITaskLogger<VmsDbContext> taskLogger,
    ITimeService timeService) : ICreateSupplier
{
    readonly VmsDbContext DbContext = dbContext;
    readonly StringBuilder SummaryText = new();

    public async Task<Supplier> CreateAsync(CreateSupplierRequest command, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Creating supplier {suppliercode} {suppliername}", command.Code, command.Name);

        SummaryText.AppendLine("# Create Supplier");
        SummaryText.AppendLine($"* Code: {command.Code}");
        SummaryText.AppendLine($"* Name: {command.Name}");
        SummaryText.AppendLine($"* Is Independent: {command.IsIndependant.YesNo()}");

        if (!string.IsNullOrEmpty(command.Address.Street))
            SummaryText.AppendLine($"* Street: {command.Address.Street}");
        if (!string.IsNullOrEmpty(command.Address.Locality))
            SummaryText.AppendLine($"* Locality: {command.Address.Locality}");
        if (!string.IsNullOrEmpty(command.Address.Town))
            SummaryText.AppendLine($"* Town: {command.Address.Town}");
        if (!string.IsNullOrEmpty(command.Address.Postcode))
            SummaryText.AppendLine($"* Postcode: {command.Address.Postcode}");
        if (command.Address.Location.Latitude != 0)
            SummaryText.AppendLine($"* Latitude: {command.Address.Location.Latitude}");
        if (command.Address.Location.Longitude != 0)
            SummaryText.AppendLine($"* Longitude: {command.Address.Location.Longitude}");


        var supplier = new Supplier(command.Code, command.Name,
            new(command.Address.Street, command.Address.Locality, command.Address.Town, command.Address.Postcode,
            new Point(command.Address.Location.Longitude, command.Address.Location.Latitude) { SRID = 4326 }),
            command.IsIndependant);

        DbContext.Add(supplier);

        searchManager.Add(null, supplier.Code, EntityKind.Supplier, supplier.Name,
            string.Join(" ", supplier.Code, supplier.Name));

        // log activity and task
        _ = await activityLog.AddAsync(supplier.Id, SummaryText, timeService.GetTime(), cancellationToken);
        taskLogger.Log(supplier.Id, nameof(CreateServiceBooking), command);

        return supplier;
    }
}


public record CreateSupplierRequest(string Code, string Name, AddressDto Address, bool IsIndependant);
