﻿using Dapper;

namespace Vms.Web.Server.Services;

public class UnlockTaskBackgroundService(IConfiguration configuration, ILogger<UnlockTaskBackgroundService> logger,
    CurrentTime timeService) : BackgroundService
{
    const int CheckTimeSeconds = 60;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var options = GetOptions(configuration);

        stoppingToken.Register(() => logger.LogDebug("UnlockTaskBackgroundService is stopping."));

        while (!stoppingToken.IsCancellationRequested)
        {
            //logger.LogDebug("UnlockTaskBackgroundService is doing background work.");
            using var conn = new SqlConnection(options.VmsDbConnection);
            try
            {
                conn.Open();

                await conn.ExecuteAsync("""
                        DELETE FROM ServiceBookingLocks WHERE DATEDIFF(second, Granted, @now) > @checkTime
                        """, new { now = timeService.Now, checkTime = CheckTimeSeconds });
            }
            catch (SqlException exception)
            {
                logger.LogCritical(exception, "Failed to communicate with the database: {Message}", exception.Message);
            }

            await Task.Delay(CheckTimeSeconds * 1000, stoppingToken);
        }

        logger.LogDebug("UnlockTaskBackgroundService is stopping.");
    }

    static ConnectionStringOptions GetOptions(IConfiguration configuration)
    {
        var options = new ConnectionStringOptions();
        configuration.GetSection("ConnectionStrings").Bind(options);
        return options;
    }
}

