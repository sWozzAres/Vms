﻿using Serilog.Context;

namespace Vms.Web.Server.Middleware;

public class TransactionMiddleware(RequestDelegate next, ILogger<TransactionMiddleware> logger)
{
    readonly RequestDelegate _next = next;
    readonly ILogger<TransactionMiddleware> _logger = logger;

    public async Task InvokeAsync(HttpContext context, VmsDbContext _context)
    {
        await _context.Database.CreateExecutionStrategy().ExecuteAsync(async () =>
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
