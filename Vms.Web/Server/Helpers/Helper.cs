using NetTopologySuite.Geometries;
using System;

namespace Vms.Web.Server.Helpers;

public static class Helper
{
    public static DateTime FromDateAndTime(DateOnly date, string time)
        => DateTime.ParseExact(date.ToString("yyyy-MM-dd") + " " + time, "yyyy-MM-dd HH:mm", null, System.Globalization.DateTimeStyles.None);
}
