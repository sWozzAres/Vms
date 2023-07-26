using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Vms.Application.Services;

public interface ITaskLogger
{
    void Log<T>(Guid id, T task);
}

public class TaskLogger(VmsDbContext dbContext) : ITaskLogger
{
    readonly VmsDbContext _dbContext = dbContext;

    public void Log<T>(Guid id, T task)
    {
        var taskLog = new TaskLog()
        {
            DocumentId = id,
            Log = JsonSerializer.Serialize(task)
        };
        _dbContext.TaskLogs.Add(taskLog);
    }
}
