using Utopia.Api.Services;

namespace Vms.Tests.Services;

public class NotifyFollowers : INotifyFollowers
{
    public Task NotifyAsync(IEnumerable<string> userIds) => Task.CompletedTask;
}
