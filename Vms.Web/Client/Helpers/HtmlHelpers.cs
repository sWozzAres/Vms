namespace Vms.Web.Client.Helpers;

public static class HtmlHelpers
{
    static readonly Random random = new();
    public static string GetRandomHtmlId(int length = 10)
    {
        // define the valid characters
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";

        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)])
            .ToArray());
    }
}
