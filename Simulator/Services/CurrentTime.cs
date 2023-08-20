using Utopia.Api.Application.Services;

namespace Simulator.Services;

public class SimulateTime : ITimeService
{
    DateTime _currentTime;
    public SimulateTime()
    {
        _currentTime = DateTime.Now;
    }
    public DateTime Now => _currentTime;
    public void SetTime(DateTime time) => _currentTime = time;

}
