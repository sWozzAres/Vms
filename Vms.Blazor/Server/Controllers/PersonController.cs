using System.Diagnostics.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vms.Blazor.Shared;

namespace Vms.Blazor.Server.Controllers;

[ApiController]
[Route("AdminApp/[controller]")]
[Route("ClientApp/[controller]")]
[Authorize(Policy = "AdminPolicy")]

public class PersonController : ControllerBase
{
    [HttpGet("{id}")]
    public IActionResult Get(int id)
    {
        return Ok(new Person() { Name = "Mark" });
    }

    [HttpPost]
    public IActionResult Post(Person person)
    {
        return CreatedAtAction("Get", new { id = 1 }, person);
    }
}
