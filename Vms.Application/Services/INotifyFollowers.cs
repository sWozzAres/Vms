namespace Vms.Application.Services;

public interface INotifyFollowers
{
    Task NotifyAsync(List<string> userIds);
}
