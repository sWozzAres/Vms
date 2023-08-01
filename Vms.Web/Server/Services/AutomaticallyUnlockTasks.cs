using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Polly;
using Vms.Domain.Infrastructure;

namespace Vms.Web.Server.Services;

public class AutomaticallyUnlockTasks(IConfiguration configuration, ILogger<AutomaticallyUnlockTasks> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var options = new ConnectionStringOptions();
        configuration.GetSection("ConnectionStrings").Bind(options);


        stoppingToken.Register(() => logger.LogDebug("#1 AutomaticallyUnlockTasks background task is stopping."));

        while (!stoppingToken.IsCancellationRequested)
        {
            logger.LogDebug("AutomaticallyUnlockTasks background task is doing background work.");

            using var conn = new SqlConnection(options.VmsDbConnection);
            try
            {
                conn.Open();
                
                await conn.ExecuteAsync("""
                DELETE FROM ServiceBookingLock WHERE DATEDIFF(second, Granted, @now) > 5
                """, new { now = DateTime.Now });
            }
            catch (SqlException exception)
            {
                logger.LogCritical(exception, "FATAL ERROR: Database connection could not be opened: {Message}", exception.Message);
            }

            await Task.Delay(60000, stoppingToken);  
        }

        logger.LogDebug("AutomaticallyUnlockTasks background task is stopping.");
    }
}

public class ConnectionStringOptions
{
    public string VmsDbConnection { get; set; } = null!;
}