using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Simulator.Services;
using Utopia.Api.Application.Services;
using Utopia.Api.Services;
using Vms.Api.Extensions;
using Vms.Domain.Infrastructure;

namespace Simulator
{
    internal class Program
    {
        public const string ConnectionString =
            "Server=SKYLAKE\\SQL2019;Database=Vms_Sim;Trusted_Connection=True;MultipleActiveResultSets=true;Encrypt=False";

        static async Task Main(string[] args)
        {
            await Host.CreateDefaultBuilder()
                .ConfigureServices(ConfigureServices)
                .ConfigureServices(services => services.AddSingleton<Executor>())
                .Build()
                .Services
                .GetRequiredService<Executor>()
                .Execute();
        }

        static void ConfigureServices(HostBuilderContext hostContext, IServiceCollection services)
        {
            services.AddLogging(builder => builder.AddDebug());
            services.AddSingleton<ISimulatorApp, SimulatorApp>();

            services.AddScoped<IUserProvider, UserProvider>();
                services.AddSingleton<INotifyFollowers, NotifyFollowers>();
                services.AddSingleton<ITimeService, SimulateTime>();
                services.AddDbContext<VmsDbContext>(options =>
                {
                    options.EnableSensitiveDataLogging();

                    options.UseSqlServer(ConnectionString, sqlOptions =>
                    {
                        sqlOptions.UseNetTopologySuite();
                        sqlOptions.UseDateOnlyTimeOnly();
                        sqlOptions.MigrationsAssembly(Assembly.GetExecutingAssembly().GetName().Name);
                        //sqlOptions.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                        sqlOptions.EnableRetryOnFailure();
                    });
                });
                services.AddVmsApplication();
        }
    }

    public class Executor(ISimulatorApp test)
    {
        readonly ISimulatorApp _app = test;

        public async Task Execute()
        {
            await _app.Run();
        }
    }
}
