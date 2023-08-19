using Utopia.Api.Application.Services;

namespace Vms.Web.Server.Services;

public class CurrentTime : ITimeService
{
    public DateTime Now => DateTime.Now;
}
