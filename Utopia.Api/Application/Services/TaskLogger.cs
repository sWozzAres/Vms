using System.Text.Json;

namespace Utopia.Api.Application.Services;

public interface ITaskLogger<TContext> where TContext : ISystemContext
{
    void Log<T>(Guid id, string taskName, T task);
}

public class TaskLogger<TContext>(
    TContext dbContext,
    IUserProvider userProvider,
    ITimeService timeService) : ITaskLogger<TContext> where TContext : ISystemContext
{
    public void Log<T>(Guid id, string taskName, T task)
    {
        var taskLog = new TaskLog(id, taskName, JsonSerializer.Serialize(task), timeService.Now, userProvider.UserId);
        dbContext.TaskLogs.Add(taskLog);
    }
}