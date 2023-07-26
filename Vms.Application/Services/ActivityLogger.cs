using System.Text;
using Vms.Domain.Services;

namespace Vms.Application.Services;

public interface IActivityLogger
{
    void Log(Guid id, StringBuilder log);
}

public class ActivityLogger(VmsDbContext dbContext, IUserProvider userProvider) : IActivityLogger
{
    readonly VmsDbContext _dbContext = dbContext;
    readonly IUserProvider _userProvider = userProvider;
    public void Log(Guid id, StringBuilder log)
    {
        _dbContext.ActivityLog.Add(new ActivityLog(id, log.ToString(), _userProvider.UserId, _userProvider.UserName));
    }
}
