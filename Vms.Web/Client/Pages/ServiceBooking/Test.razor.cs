using Microsoft.AspNetCore.Components;

namespace Vms.Web.Client.Pages.ServiceBooking;
public partial class Test
{
    //private Type componentType = typeof(MyComponent);
    private Dictionary<string, object> parameters = new();
    

    private async Task OnClick(bool saved)
    {
        // Handle the click event here
    }

    public Test()
    {
        parameters.Add("OnClick", EventCallback.Factory.Create<bool>(this, OnClick));   
    }
}
