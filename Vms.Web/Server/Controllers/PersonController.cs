using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vms.Web.Shared;

namespace Vms.Blazor.Server.Controllers;

[ApiController]
[Route("ClientApp/[controller]")]
[Authorize(Policy = "ClientPolicy")]

public class PersonController : ControllerBase
{
    [HttpGet("{id}")]
    public IActionResult Get(int id)
    {
        return Ok(new Person()
        {
            Name = "Mark",
            Age = 19,
            BirthDate = new DateTime(1972, 4, 16),
            Email = "markb@utopiasoftware.co.uk",
            IsAdministrator = true,
            Notes = "notes",
            Status = 0,
            TheManufacturer = Person.Manufacturer.VirginGalactic
        });
    }

    [HttpPost]
    public IActionResult Post(Person person)
    {
        return CreatedAtAction("Get", new { id = 1 }, person);
    }
}
