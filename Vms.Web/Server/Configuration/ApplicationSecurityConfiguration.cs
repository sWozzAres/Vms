using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace Vms.Web.Server.Configuration;

public static class ApplicationSecurityConfiguration
{
    public static void AddApplicationSecurity(this IServiceCollection services)
    {
        JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
        services.AddAuthentication("Bearer")
            .AddJwtBearer("Bearer", options =>
            {
                options.Authority = "https://localhost:5000";
                options.Audience = "utopia";

                //options.MapInboundClaims = false;

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = false,
                    NameClaimType = "name",
                    ClockSkew = TimeSpan.FromSeconds(5)
                };

                
            });
        services.AddTransient<IClaimsTransformation, MyClaimsTransformation>();
        services.AddAuthorization(options =>
        {
            options.AddPolicy("AdminPolicy", policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireClaim("scope", "vms.admin");
            });
            options.AddPolicy("ClientPolicy", policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireClaim("scope", "vms.client");
            });
        });
    }
}
