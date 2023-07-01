using System.Runtime.CompilerServices;

namespace Vms.Application.UseCase;

public class CreateModel
{
    readonly VmsDbContext DbContext;
    MakeRole? Make;
    public CreateModel(VmsDbContext dbContext)
       => DbContext = dbContext;

    public async Task<VehicleModel> CreateAsync(CreateModelRequest request, CancellationToken cancellationToken = default)
    {
        Make = new(await DbContext.VehicleMakes.FindAsync(request.Make, cancellationToken)
            ?? throw new VmsDomainException("Make not found."), this);

        var model = Make.CreateModel(request.Model);

        //await DbContext.SaveChangesAsync(cancellationToken);

        return model;
    }

    class MakeRole(VehicleMake self, CreateModel context)
    {
        public VehicleModel CreateModel(string model)
        {
            var vm = new VehicleModel(self.Make, model);
            context.DbContext.VehicleModels.Add(vm);
            return vm;
        }
    }
}

public record CreateModelRequest(string Make, string Model);