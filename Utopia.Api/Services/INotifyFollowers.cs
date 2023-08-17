namespace Utopia.Api.Services;

public interface INotifyFollowers
{
    Task NotifyAsync(IEnumerable<string> userIds);
}
