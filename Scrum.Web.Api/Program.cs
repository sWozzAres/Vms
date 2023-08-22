using System.Reflection;
using Scrum.Api.Domain.Infrastructure;
using Scrum.Api.Extensions;
using Scrum.Web.Api.Extensions;
using Scrum.Web.Api.Infrastructure.Seed;
using Scrum.Web.Api.Server;

namespace Scrum.Web.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddScrumApplication(
                builder.Configuration.GetConnectionString("ScrumDbConnection")
                    ?? throw new InvalidOperationException("Failed to load connection string ScrumDbConnection."));

            builder.Services.AddControllers()
                .AddApplicationPart(typeof(Scrum.Api.Controllers.ProductController).GetTypeInfo().Assembly);

            var app = builder.Build();

            app.MigrateDbContext<ScrumDbContext>((context, services) =>
            {
                var env = services.GetRequiredService<IWebHostEnvironment>();
                var logger = services.GetRequiredService<ILogger<ScrumDbContext>>();

                new ScrumDbContextSeeder(context, services)
                    .SeedAsync(env)
                    .Wait();
            });
            
            // Configure the HTTP request pipeline.
            app.UseMiddleware<ExceptionMiddleware>();

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
