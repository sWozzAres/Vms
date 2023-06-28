using Microsoft.AspNetCore.Mvc.ActionConstraints;

namespace Vms.Web.Server;

[AttributeUsage(AttributeTargets.Method)]
public class AcceptHeaderAttribute : Attribute, IActionConstraint
{
    readonly string _acceptHeader;
    public int Order { get; set; }
    public AcceptHeaderAttribute(string acceptHeader)
    {
        _acceptHeader = acceptHeader;
    }

    public bool Accept(ActionConstraintContext context)
    {
        return context.RouteContext.HttpContext.Request.Headers["Accept"]
            .Any(x => x is not null && x.StartsWith(_acceptHeader));
    }
}
