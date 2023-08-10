namespace Vms.Application.Services;

public interface INotifyFollowers
{
    Task NotifyAsync(IEnumerable<string> userIds);
}
