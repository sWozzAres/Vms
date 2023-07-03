﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Vms.Domain.Infrastructure;
using Vms.Web.Shared;

namespace Vms.Web.Server.Controllers.ClientApp;

[ApiController]
[Route("ClientApp/api/[controller]")]
[Authorize(Policy = "ClientPolicy")]
[Produces("application/json")]
public class VehicleMakeController : ControllerBase
{
    readonly ILogger<CompanyController> _logger;
    readonly VmsDbContext _context;

    public VehicleMakeController(ILogger<CompanyController> logger, VmsDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    [HttpGet]
    [AcceptHeader("application/vnd.short")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await _context.VehicleMakes
                    .Select(x => new VehicleMakeShortListModel(x.Make))
                    .ToListAsync(cancellationToken);

        return Ok(result);
    }

    [HttpGet]
    [Route("{make}/models")]
    [AcceptHeader("application/vnd.short")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAllModels(
        string make,
        CancellationToken cancellationToken)
        => Ok(await _context.VehicleMakes
            .Include(m => m.VehicleModels)
            .Where(m => m.Make == make)
            .SelectMany(x => x.VehicleModels)
            .Select(m=> new VehicleModelShortListModel(m.Model))
            .ToListAsync(cancellationToken));
}