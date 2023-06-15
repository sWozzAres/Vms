namespace Vms.Web.Client.Extensions;

public static class TypeExtensions
{
    public static string ToHtml(this bool b) => b ? "true" : "false";
}
