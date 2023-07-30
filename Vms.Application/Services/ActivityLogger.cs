using System.Text;
using Vms.Domain.Services;
using Vms.DomainApplication.Services;

namespace Vms.Application.Services;

public interface IActivityLogger
{
    Task LogAsync(Guid id, StringBuilder log, CancellationToken cancellationToken);
}

public class ActivityLogger(VmsDbContext dbContext, IUserProvider userProvider, IEmailSender emailSender) : IActivityLogger
{
    readonly VmsDbContext _dbContext = dbContext;
    readonly IUserProvider _userProvider = userProvider;
    readonly IEmailSender _emailSender = emailSender;

    public async Task LogAsync(Guid id, StringBuilder log, CancellationToken cancellationToken)
    {
        var followers = await _dbContext.Followers.AsNoTracking()
            .Where(f => f.DocumentId == id)
            .ToListAsync(cancellationToken);

        if (followers.Count != 0)
        {
            foreach (var f in followers)
            {
                _emailSender.Send(f.EmailAddress, "Activity", log.ToString());
            }
        }

        _dbContext.ActivityLog.Add(new ActivityLog(id, log.ToString(), _userProvider.UserId, _userProvider.UserName));
    }
}