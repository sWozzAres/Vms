using Microsoft.AspNetCore.Mvc;

namespace Vms.Blazor.Server.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
