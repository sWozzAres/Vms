using Microsoft.AspNetCore.Mvc.ActionConstraints;

namespace Vms.Web.Server;

[AttributeUsage(AttributeTargets.Method)]
public class AcceptHeaderAttribute(string acceptHeader) : Attribute, IActionConstraint
{
    public int Order { get; set; }

    public bool Accept(ActionConstraintContext context)
    {
        return context.RouteContext.HttpContext.Request.Headers
            .Accept.Any(x => x is not null && x.StartsWith(acceptHeader));
    }
}
