using System.Text.Json;

namespace Vms.Application.Services;

public interface ITaskLogger
{
    void Log<T>(Guid id, string taskName, T task);
}

public class TaskLogger(VmsDbContext dbContext, IUserProvider userProvider) : ITaskLogger
{
    public void Log<T>(Guid id, string taskName, T task)
    {
        var taskLog = new TaskLog(id, taskName, JsonSerializer.Serialize(task), DateTime.Now, userProvider.UserId);
        dbContext.TaskLogs.Add(taskLog);
    }
}
