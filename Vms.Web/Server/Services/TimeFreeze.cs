using Utopia.Api.Application.Services;

namespace Vms.Web.Server.Services;

public class TimeFreeze : ITimeService
{
    readonly DateTime _time;
    public TimeFreeze(DateTime time)
    {
        _time = time.Kind == DateTimeKind.Local
            ? time
            : throw new ArgumentException("Time must be Local.", nameof(time));
    }
    public TimeFreeze(ITimeService time)
    {
        _time = time.Now;
    }
    public DateTime Now => _time;
}
