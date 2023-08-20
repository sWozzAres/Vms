using Utopia.Api.Application.Services;

namespace Vms.Tests.Services;

public class CurrentTime : ITimeService
{
    public DateTime Now => DateTime.Now;
}
