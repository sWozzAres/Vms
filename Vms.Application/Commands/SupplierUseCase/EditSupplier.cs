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

            if (self.Code != ctx.Command.Code)
            {
                Ctx.SummaryText.AppendLine($"* Code: {ctx.Command.Code}");
                self.Code = ctx.Command.Code;
                isModified = true;
            }
            if (self.Name != ctx.Command.Name)
            {
                Ctx.SummaryText.AppendLine($"* Name: {ctx.Command.Name}");
                self.Name = ctx.Command.Name;
                isModified = true;
            }
            if (self.IsIndependent != ctx.Command.IsIndependent)
            {
                Ctx.SummaryText.AppendLine($"* Is Independant: {ctx.Command.IsIndependent.YesNo()}");
                self.IsIndependent = ctx.Command.IsIndependent;
                isModified = true;
            }

            if (Self.Address.AddModificationSummary(ctx.Command.Address, Ctx.SummaryText))
            {
                self.SetAddress(ctx.Command.Address.Street, ctx.Command.Address.Locality, ctx.Command.Address.Town, ctx.Command.Address.Postcode,
                    ctx.Command.Address.Location.Latitude, ctx.Command.Address.Location.Longitude);
                isModified = true;
            }

            return isModified;
        }
    }
}