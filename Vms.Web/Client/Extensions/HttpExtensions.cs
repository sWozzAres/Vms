using System.Net.Http.Headers;

namespace Vms.Web.Client.Extensions;

public static class HttpExtensions
{
    public static void ClearAndAdd(this HttpHeaderValueCollection<MediaTypeWithQualityHeaderValue> headers, params string[] values)
    {
        headers.Clear();
        foreach (var value in values)
        {
            headers.Add(new MediaTypeWithQualityHeaderValue(value));
        }
    }
}