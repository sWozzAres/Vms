using System.Text.Json;

namespace Vms.Application.Services;

public interface ITaskLogger
{
    void Log<T>(Guid id, string taskName, T task);
}

public class TaskLogger(VmsDbContext dbContext) : ITaskLogger
{
    public void Log<T>(Guid id, string taskName, T task)
    {
        var taskLog = new TaskLog()
        {
            DocumentId = id,
            TaskName = taskName,
            Log = JsonSerializer.Serialize(task),
            EntryDate = DateTime.Now,
        };
        dbContext.TaskLogs.Add(taskLog);
    }
}
