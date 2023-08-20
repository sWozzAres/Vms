using System.Text.RegularExpressions;

namespace Vms.Application.Extensions;

public static partial class ClrExtensions
{
    public static string YesNo(this bool b)
        => b ? "Yes" : "No";
    
    public static string ToDisplayString(this Enum @enum)
        => MyRegex().Replace(@enum.ToString(), "$1 $2");
    
    [GeneratedRegex("([a-z])([A-Z])")]
    private static partial Regex MyRegex();
}
