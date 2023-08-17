using Microsoft.AspNetCore.Mvc.ActionConstraints;

namespace Catalog.Api.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class AcceptHeaderAttribute(string acceptHeader) : Attribute, IActionConstraint
{
    public int Order { get; set; }

    public bool Accept(ActionConstraintContext context)
    {
        return context.RouteContext.HttpContext.Request.Headers
            .Any(x => x.Key == "Accept" && x.Value == acceptHeader);
    }
}
