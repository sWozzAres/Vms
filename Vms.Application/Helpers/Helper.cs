namespace Vms.Application;

public static class Helper
{
    //public static DateTime CombineDateAndTime(DateOnly date, string time)
    //    => DateTime.ParseExact(date.ToString("yyyy-MM-dd") + " " + time, "yyyy-MM-dd HH:mm", null, System.Globalization.DateTimeStyles.None);

    public static DateTime CombineDateAndTime(DateOnly date, TimeOnly time) => date.ToDateTime(time);
    //=> new (date.Year, date.Month, date.Day, time.Hour, time.Minute, time.Second, time.Millisecond);
}
