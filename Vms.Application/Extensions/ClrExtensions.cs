namespace Vms.Application.Extensions;

public static class ClrExtensions
{
    public static string YesNo(this bool b)
        => b ? "Yes" : "No";
}
