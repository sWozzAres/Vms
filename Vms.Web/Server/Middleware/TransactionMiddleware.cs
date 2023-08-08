using Microsoft.EntityFrameworkCore;
using Serilog.Context;
using Vms.Domain.Infrastructure;

namespace Vms.Web.Server.Middleware;

public class TransactionMiddleware
{
    readonly RequestDelegate _next;
    readonly ILogger<TransactionMiddleware> _logger;

    public TransactionMiddleware(RequestDelegate next, ILogger<TransactionMiddleware> logger)
        => (_next, _logger) = (next, logger);

    public async Task InvokeAsync(HttpContext context, VmsDbContext _context)
    {
        if (context.Request.Method == "GET")
        {
            await _next(context);
        }
        else
        {
            var strategy = _context.Database.CreateExecutionStrategy();

            await strategy.ExecuteAsync(async () =>
            {
                using var transaction = _context.Database.BeginTransaction();
                using (LogContext.PushProperty("TransactionContext", transaction.TransactionId))
                {
                    _logger.LogInformation("----- Begin transaction {TransactionId}", transaction.TransactionId);
                    try
                    {
                        await _next(context);

                        await _context.SaveChangesAsync();

                        _logger.LogInformation("----- Commit transaction {TransactionId}", transaction.TransactionId);

                        await transaction.CommitAsync();

                        return true;
                    }
                    catch (Exception)
                    {
                        _logger.LogInformation("----- Rollback transaction {TransactionId}", transaction.TransactionId);
                        await transaction.RollbackAsync();
                        throw;
                        //return false;
                    }
                }
            });
        }
    }
}
