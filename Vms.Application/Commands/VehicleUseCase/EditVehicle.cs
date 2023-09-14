namespace Vms.Application.Commands.VehicleUseCase;

public class EditVehicle(
    VmsDbContext dbContext,
    IActivityLogger<VmsDbContext> activityLog,
    ITaskLogger<VmsDbContext> taskLogger,
    ILogger<EditVehicle> logger,
    ISearchManager searchManager) : VehicleTaskBase(dbContext, activityLog)
{
    readonly ISearchManager SearchManager = searchManager;
    VehicleRole? Vehicle;
    VehicleDto Command = null!;

    public async Task<bool> EditAsync(Guid vehicleId, VehicleDto command, CancellationToken cancellationToken)
    {
        logger.LogInformation("Editing vehicle: {vehicleid}, command: {@vehicledto}.", vehicleId, command);

        Command = command;
        Vehicle = new(await Load(vehicleId, cancellationToken), this);

        var isModified = await Vehicle.Modify();
        if (isModified)
        {
            await LogActivity();
            taskLogger.Log(vehicleId, nameof(EditVehicle), command);
        }

        return isModified;
    }
    public class VehicleRole(Vehicle self, EditVehicle ctx) : VehicleRoleBase<EditVehicle>(self, ctx)
    {
        public async Task<bool> Modify()
        {
            Ctx.SummaryText.AppendLine("# Edit");

            bool isModified = false;

            if (Self.Vrm != Ctx.Command.Vrm)
            {
                Ctx.SummaryText.AppendLine($"* Vrm: {Ctx.Command.Vrm}");
                Self.Vrm = Ctx.Command.Vrm;

                await Ctx.SearchManager.UpdateOrAdd(Self.CompanyCode, Self.Id.ToString(), EntityKind.Vehicle, Self.Vrm, Self.Vrm, Ctx.CancellationToken);

                isModified = true;
            }

            if (Self.Make != Ctx.Command.Make || Self.Model != Ctx.Command.Model)
            {
                if (Self.Make != Ctx.Command.Make)
                    Ctx.SummaryText.AppendLine($"* Make: {Ctx.Command.Make}");
                if (Self.Model != Ctx.Command.Model)
                    Ctx.SummaryText.AppendLine($"* Model: {Ctx.Command.Model}");
                Self.SetMakeModel(Ctx.Command.Make!, Ctx.Command.Model!);
                isModified = true;
            }

            DateOnly motDue = Ctx.Command.MotDue ?? throw new VmsDomainException("Mot Due cannot be null.");

            var me = await Ctx.DbContext.MotEvents
                .FirstOrDefaultAsync(m => m.VehicleId == Self.Id && m.IsCurrent, Ctx.CancellationToken);
            if (me is null)
            {
                Ctx.SummaryText.AppendLine($"* MOT Due: {Ctx.Command.MotDue}");
                me = new(Self.CompanyCode, Self.Id, motDue, true);
                Ctx.DbContext.MotEvents.Add(me);
                isModified = true;
            }
            else if (me.Due != Ctx.Command.MotDue)
            {
                Ctx.SummaryText.AppendLine($"* MOT Due: {Ctx.Command.MotDue}");
                me.Due = motDue;
                isModified = true;
            }

            if (Self.DateFirstRegistered != Ctx.Command.DateFirstRegistered)
            {
                Ctx.SummaryText.AppendLine($"* Date First Registered: {Ctx.Command.DateFirstRegistered}");
                Self.DateFirstRegistered = Ctx.Command.DateFirstRegistered;
                isModified = true;
            }
            if (Self.ChassisNumber != Ctx.Command.ChassisNumber)
            {
                Ctx.SummaryText.AppendLine($"* Chassis Number: {Ctx.Command.ChassisNumber}");
                Self.ChassisNumber = Ctx.Command.ChassisNumber;
                isModified = true;
            }

            if (Self.Address.AddModificationSummary(Ctx.Command.Address, Ctx.SummaryText))
            {
                Self.SetAddress(Ctx.Command.Address.Street, Ctx.Command.Address.Locality,
                    Ctx.Command.Address.Town, Ctx.Command.Address.Postcode,
                    Ctx.Command.Address.Location.Latitude, Ctx.Command.Address.Location.Longitude);
                isModified = true;
            }

            return isModified;
        }
    }
}