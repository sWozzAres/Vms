using Vms.Application.Services;

namespace Vms.Web.Server.Services;

public class CurrentTime : ITimeService
{
    public DateTime GetTime() => DateTime.Now;
}
