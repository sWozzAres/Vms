namespace Vms.Application.UseCase;

public class CreateMake
{
    readonly VmsDbContext DbContext;

    public CreateMake(VmsDbContext dbContext)
       => DbContext = dbContext;

    public VehicleMake Create(CreateMakeRequest request, CancellationToken cancellationToken = default)
    {
        var make = new VehicleMake(request.Make);
        DbContext.Add(make);

        //await DbContext.SaveChangesAsync(cancellationToken);
        
        return make;
    }
}


public record CreateMakeRequest(string Make);
