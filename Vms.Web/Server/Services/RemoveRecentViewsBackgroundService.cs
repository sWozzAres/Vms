﻿using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Vms.Web.Server.Services;

public class RemoveRecentViewsBackgroundService(IConfiguration configuration, ILogger<RemoveRecentViewsBackgroundService> logger,
    CurrentTime timeService) : BackgroundService
{
    const int CheckTimeSeconds = 60 * 60;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var options = GetOptions(configuration);

        stoppingToken.Register(() => logger.LogDebug("RemoveRecentViewsBackgroundService is stopping."));

        while (!stoppingToken.IsCancellationRequested)
        {
            logger.LogDebug("RemoveRecentViewsBackgroundService is doing background work. Time is '{time}'.", timeService.GetTime());

            using var conn = new SqlConnection(options.VmsDbConnection);
            try
            {
                conn.Open();

                await conn.ExecuteAsync("""
                DELETE FROM System.RecentViews WHERE DATEDIFF(day, ViewDate, @now) > 0
                """, new { now = timeService.GetTime() });
            }
            catch (SqlException exception)
            {
                logger.LogCritical(exception, "Failed to communicate with the database: {Message}", exception.Message);
            }

            await Task.Delay(CheckTimeSeconds * 1000, stoppingToken);
        }

        logger.LogDebug("RemoveRecentViewsBackgroundService is stopping.");
    }

    static ConnectionStringOptions GetOptions(IConfiguration configuration)
    {
        var options = new ConnectionStringOptions();
        configuration.GetSection("ConnectionStrings").Bind(options);
        return options;
    }

    class ConnectionStringOptions
    {
        public string VmsDbConnection { get; set; } = null!;
    }
}
