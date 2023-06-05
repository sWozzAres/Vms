using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Identity.API.Data
{
    public class ConfigurationDbContextSeed
    {
        public async Task SeedAsync(ConfigurationDbContext context, IConfiguration configuration)
        {
            //callbacks urls from config:
            var clientUrls = new Dictionary<string, string>
            {
                { "Mvc", configuration.GetValue<string>("MvcClient") },
                { "Blazor", configuration.GetValue<string>("BlazorClient") },
                { "Utopia", configuration.GetValue<string>("UtopiaClient") },
                { "Storefront", configuration.GetValue<string>("StorefrontClient") },

                { "Admin", configuration.GetValue<string>("VmsAdmin") },
                { "Client", configuration.GetValue<string>("VmsClient") }
            };

            if (!context.Clients.Any())
            {
                foreach (var client in Config.GetClients(clientUrls))
                {
                    context.Clients.Add(client.ToEntity());
                }
                await context.SaveChangesAsync();
            }

            if (!context.IdentityResources.Any())
            {
                foreach (var resource in Config.GetIdentityResources())
                {
                    context.IdentityResources.Add(resource.ToEntity());
                }
                await context.SaveChangesAsync();
            }

            if (!context.ApiResources.Any())
            {
                foreach (var api in Config.ApiResources)
                {
                    context.ApiResources.Add(api.ToEntity());
                }

                await context.SaveChangesAsync();
            }

            if (!context.ApiScopes.Any())
            {
                foreach (var api in Config.ApiScopes)
                {
                    context.ApiScopes.Add(api.ToEntity());
                }

                await context.SaveChangesAsync();
            }
        }
    }
}
