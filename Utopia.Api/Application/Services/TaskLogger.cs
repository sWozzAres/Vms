using System.Text.Json;
using Utopia.Api.Domain.Infrastructure;
using Utopia.Api.Services;

namespace Utopia.Api.Application.Services;

public interface ITaskLogger<TContext> where TContext : ISystemContext
{
    void Log<T>(Guid id, string taskName, T task);
}

public class TaskLogger<TContext>(TContext dbContext, IUserProvider userProvider) : ITaskLogger<TContext> where TContext : ISystemContext
{
    public void Log<T>(Guid id, string taskName, T task)
    {
        var taskLog = new TaskLog(id, taskName, JsonSerializer.Serialize(task), DateTime.Now, userProvider.UserId);
        dbContext.TaskLogs.Add(taskLog);
    }
}