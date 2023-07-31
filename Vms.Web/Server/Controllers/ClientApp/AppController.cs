﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Vms.Domain.Entity;
using Vms.Domain.Infrastructure;
using Vms.Domain.Services;
using Vms.Web.Shared;

namespace Vms.Web.Server.Controllers.ClientApp;

[ApiController]
[Route("ClientApp/api/[controller]")]
[Authorize(Policy = "ClientPolicy")]
[Produces("application/json")]
public class AppController : ControllerBase
{
    readonly ILogger<CompanyController> _logger;
    readonly VmsDbContext _context;

    public AppController(ILogger<CompanyController> logger, VmsDbContext context)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    [HttpGet]
    [Route("users/{code}")]
    public async Task<IActionResult> GetUsersForCompany(string code,
        IUserProvider userProvider,
        CancellationToken cancellationToken)
    {
        if (userProvider.TenantId == "*" || userProvider.TenantId == code)
        {
            var users = await _context.Users.Where(u => u.TenantId == "*" || u.TenantId == code)
                .Select(u => new  UserDto(u.UserId, u.UserName))
                .ToListAsync(cancellationToken);

            return Ok(users);
        }

        throw new InvalidOperationException("User has no access to tenant.");
    }

    [HttpPost]
    [Route("register")]
    public async Task<IActionResult> RegisterLogin(IUserProvider userProvider, CancellationToken cancellationToken)
    {
        var user = await _context.Users.FindAsync(userProvider.UserId, cancellationToken);
        if (user == null)
        {
            user = new Domain.Entity.User()
            {
                UserId = userProvider.UserId,
                UserName = userProvider.UserName,
                TenantId = userProvider.TenantId,
            };

            _context.Users.Add(user);
        }

        // update username / tenantid
        if (user.UserName != userProvider.UserName)
        {
            user.UserName = userProvider.UserName;
        }

        if (user.TenantId != userProvider.TenantId)
        {
            user.TenantId = userProvider.TenantId;
        }

        var login = new Login()
        {
            UserId = userProvider.UserId,
            LoginTime = DateTime.Now,
        };

        _context.Logins.Add(login);

        await _context.SaveChangesAsync(cancellationToken);

        return Ok();
    }
}
