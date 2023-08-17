namespace Vms.Application.Commands.VehicleUseCase;

public interface IEditVehicle
{
    Task<bool> EditAsync(Guid id, VehicleDto command, CancellationToken cancellationToken);
}

public class EditVehicle(VmsDbContext dbContext, IActivityLogger<VmsDbContext> activityLog,
    ITaskLogger<VmsDbContext> taskLogger, ILogger<EditVehicle> logger) : IEditVehicle
{
    readonly VmsDbContext DbContext = dbContext;
    readonly StringBuilder SummaryText = new();

    public async Task<bool> EditAsync(Guid id, VehicleDto command, CancellationToken cancellationToken)
    {
        logger.LogInformation("Editing vehicle: {vehicleid}, command: {@vehicledto}.", id, command);

        var vehicle = await DbContext.Vehicles.FindAsync(new object[] { id }, cancellationToken)
            ?? throw new InvalidOperationException("Failed to load vehicle.");

        SummaryText.AppendLine("# Edit");

        bool isModified = false;

        if (vehicle.Vrm != command.Vrm)
        {
            SummaryText.AppendLine($"* Vrm: {command.Vrm}");
            vehicle.Vrm = command.Vrm;
            isModified = true;
        }

        if (vehicle.Make != command.Make || vehicle.Model != command.Model)
        {
            if (vehicle.Make != command.Make)
                SummaryText.AppendLine($"Make: {command.Make}");
            if (vehicle.Model != command.Model)
                SummaryText.AppendLine($"* Model: {command.Model}");
            vehicle.UpdateModel(command.Make!, command.Model!);
            isModified = true;
        }

        if (vehicle.Mot.Due != command.MotDue)
        {
            SummaryText.AppendLine($"* MOT Due: {command.MotDue}");
            vehicle.Mot.Due = command.MotDue;

            var mot = dbContext.MotEvents.FirstOrDefault(x => x.VehicleId == vehicle.Id && x.IsCurrent);
            if (mot is not null)
            {
                mot.Due = command.MotDue;
            }
            else
            {
                DbContext.MotEvents.Add(new(vehicle.CompanyCode, vehicle.Id, command.MotDue, true));
            }
            isModified = true;
        }

        if (vehicle.DateFirstRegistered != command.DateFirstRegistered)
        {
            SummaryText.AppendLine($"* Date First Registered: {command.DateFirstRegistered}");
            vehicle.DateFirstRegistered = command.DateFirstRegistered;
            isModified = true;
        }
        if (vehicle.ChassisNumber != command.ChassisNumber)
        {
            SummaryText.AppendLine($"* Chassis Number: {command.ChassisNumber}");
            vehicle.ChassisNumber = command.ChassisNumber;
            isModified = true;
        }

        bool addressModified = false;
        if (vehicle.Address.Street != command.Address.Street)
        {
            SummaryText.AppendLine($"* Street: {command.Address.Street}");
            addressModified = true;
        }
        if (vehicle.Address.Locality != command.Address.Locality)
        {
            SummaryText.AppendLine($"* Locality: {command.Address.Locality}");
            addressModified = true;
        }
        if (vehicle.Address.Town != command.Address.Town)
        {
            SummaryText.AppendLine($"* Town: {command.Address.Town}");
            addressModified = true;
        }
        if (vehicle.Address.Postcode != command.Address.Postcode)
        {
            SummaryText.AppendLine($"* Postcode: {command.Address.Postcode}");
            addressModified = true;
        }
        if (vehicle.Address.Location.Coordinate.Y != command.Address.Location.Latitude)
        {
            SummaryText.AppendLine($"* Latitude: {command.Address.Location.Latitude}");
            addressModified = true;
        }
        if (vehicle.Address.Location.Coordinate.X != command.Address.Location.Longitude)
        {
            SummaryText.AppendLine($"* Longitude: {command.Address.Location.Longitude}");
            addressModified = true;
        }
        if (addressModified)
        {
            vehicle.Address = new Address(command.Address.Street, command.Address.Locality, command.Address.Town, command.Address.Postcode,
                new Point(command.Address.Location.Longitude, command.Address.Location.Latitude) { SRID = 4326 });

            isModified = true;
        }

        if (isModified)
        {
            _ = await activityLog.AddAsync(id, SummaryText, cancellationToken);
            taskLogger.Log(id, nameof(EditVehicle), command);
        }

        return isModified;
    }
}