using Utopia.Api.Services;

namespace Simulator.Services;

public class NotifyFollowers : INotifyFollowers
{
    public Task NotifyAsync(IEnumerable<string> userIds) => Task.CompletedTask;
}
