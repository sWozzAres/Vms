namespace Vms.Application.Commands;

public class CreateMake(VmsDbContext dbContext, ILogger<CreateMake> logger)
{
    readonly VmsDbContext DbContext = dbContext;

    public VehicleMake Create(CreateMakeRequest request)
    {
        logger.LogInformation("Creating make {vehiclemake}", request.Make);

        var make = new VehicleMake(request.Make);
        DbContext.Add(make);

        return make;
    }
}


public record CreateMakeRequest(string Make);
