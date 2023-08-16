namespace Utopia.Blazor.Application.Common.Extensions;

public static class TypeExtensions
{
    public static string ToHtml(this bool b) => b ? "true" : "false";
    public static string ToFriendlyText(this DateTime time)
    {
        return $"{FriendlyDateText()} at {time:t}";

        string FriendlyDateText()
        {
            if (time.Date == DateTime.Today)
                return "today";
            else if (time.Date == DateTime.Today.AddDays(1))
                return "tommorrow";
            else if (time.Date == DateTime.Today.AddDays(-1))
                return "yesterday";
            else
                return time.ToString("d");
        }
    }
}
