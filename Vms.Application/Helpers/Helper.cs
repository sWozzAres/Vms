namespace Vms.Application;

public static class Helper
{
    public static DateTime CombineDateAndTime(DateOnly date, string time)
        => DateTime.ParseExact(date.ToString("yyyy-MM-dd") + " " + time, "yyyy-MM-dd HH:mm", null, System.Globalization.DateTimeStyles.None);
}
