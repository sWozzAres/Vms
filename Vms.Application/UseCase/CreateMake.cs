namespace Vms.Application.UseCase;

public class CreateMake(VmsDbContext dbContext)
{
    readonly VmsDbContext DbContext = dbContext;

    public VehicleMake Create(CreateMakeRequest request)
    {
        var make = new VehicleMake(request.Make);
        DbContext.Add(make);

        //await DbContext.SaveChangesAsync(cancellationToken);
        
        return make;
    }
}


public record CreateMakeRequest(string Make);
