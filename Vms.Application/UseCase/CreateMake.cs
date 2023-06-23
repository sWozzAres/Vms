namespace Vms.Application.UseCase;

public class CreateMake
{
    readonly VmsDbContext DbContext;

    public CreateMake(VmsDbContext dbContext)
       => DbContext = dbContext;

    public VehicleMake Create(CreateMakeRequest request)
    {
        var make = new VehicleMake(request.Make);
        DbContext.Add(make);

        return make;
    }
}


public record CreateMakeRequest(string Make);
