using Vms.Application.Commands.VehicleUseCase;

namespace Vms.Application.Commands.SupplierUseCase;

public class EditSupplier(
    VmsDbContext dbContext,
    IActivityLogger<VmsDbContext> activityLog,
    ITaskLogger<VmsDbContext> taskLogger,
    ILogger<EditSupplier> logger) : SupplierTaskBase(dbContext, activityLog)
{
    SupplierDto Command = null!;
    SupplierRole? Supplier;

    public async Task<bool> EditAsync(string code, SupplierDto command, CancellationToken cancellationToken)
    {
        Command = command;

        logger.LogInformation("Editing supplier: {suppliercode}, command: {@supplierdto}.", code, command);

        Supplier = new(await Load(code, cancellationToken), this);

        SummaryText.AppendLine("# Edit");

        bool isModified = Supplier.Modify();
        if (isModified)
        {
            await LogActivity();
            taskLogger.Log(Supplier.Entity.Id, nameof(EditSupplier), command);
        }

        return isModified;
    }
    class SupplierRole(Supplier self, EditSupplier ctx) : SupplierRoleBase<EditSupplier>(self, ctx)
    {
        public bool Modify()
        {
            bool isModified = false;

            if (Self.Code != Ctx.Command.Code)
            {
                Ctx.SummaryText.AppendLine($"* Code: {Ctx.Command.Code}");
                Self.Code = Ctx.Command.Code;
                isModified = true;
            }
            if (Self.Name != Ctx.Command.Name)
            {
                Ctx.SummaryText.AppendLine($"* Name: {Ctx.Command.Name}");
                Self.Name = Ctx.Command.Name;
                isModified = true;
            }
            if (Self.IsIndependent != Ctx.Command.IsIndependent)
            {
                Ctx.SummaryText.AppendLine($"* Is Independant: {Ctx.Command.IsIndependent.YesNo()}");
                Self.IsIndependent = Ctx.Command.IsIndependent;
                isModified = true;
            }

            if (Self.Address.AddModificationSummary(Ctx.Command.Address, Ctx.SummaryText))
            {
                Self.SetAddress(Ctx.Command.Address.Street, Ctx.Command.Address.Locality, Ctx.Command.Address.Town, Ctx.Command.Address.Postcode,
                    Ctx.Command.Address.Location.Latitude, Ctx.Command.Address.Location.Longitude);
                isModified = true;
            }

            return isModified;
        }
    }
}