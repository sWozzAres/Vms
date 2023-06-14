using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Rendering;

namespace Vms.Client.Admin.Shared;

public class ValidationMessage2<TValue> : ValidationMessage<TValue>
{
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, "div");
        builder.AddAttribute(1, "class", "validation-message-holder"); 
        
        base.BuildRenderTree(builder); 
        
        builder.AddMarkupContent(2, "&nbsp");
        builder.CloseElement();
    }
}
