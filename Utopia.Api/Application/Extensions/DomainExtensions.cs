namespace Utopia.Api.Application.Extensions;

public static class DomainExtensions
{
    public static ActivityLogDto ToDto(this ActivityLog activityLog)
       => new(activityLog.Id, activityLog.Text, activityLog.EntryDate, activityLog.UserName, activityLog.IsNote);
}
