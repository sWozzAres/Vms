namespace Vms.Application.Commands.VehicleUseCase;

public class ChangeVrm(
    VmsDbContext dbContext,
    IActivityLogger<VmsDbContext> activityLog,
    ILogger<ChangeVrm> logger) : VehicleTaskBase(dbContext, activityLog)
{
    ChangeVrmRequest Command = null!;
    VehicleRole? Vehicle;

    public async Task ChangeTo(ChangeVrmRequest command, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Changing vehicle Vrm. vehicle: {vehicleid}, command: {@command}.", command.VehicleId, command);

        Command = command;
        Vehicle = new(await Load(command.VehicleId, cancellationToken), this);

        Vehicle.ChangeVrm();
    }

    class VehicleRole(Vehicle self, ChangeVrm ctx) : VehicleRoleBase<ChangeVrm>(self, ctx)
    {
        public void ChangeVrm() 
            => Self.Vrm = Ctx.Command.NewVrm;
    }
}

public record ChangeVrmRequest(Guid VehicleId, string NewVrm);
