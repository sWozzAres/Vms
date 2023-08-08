using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Identity.API.Models;
using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Identity.API
{
    public class ProfileService : IProfileService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<ProfileService> _logger;

        public ProfileService(UserManager<ApplicationUser> userManager, ILogger<ProfileService> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        //async public Task GetProfileDataAsync(ProfileDataRequestContext context)
        //{
        //    var subject = context.Subject ?? throw new ArgumentNullException(nameof(context.Subject));

        //    var subjectId = subject.Claims.Where(x => x.Type == "sub").FirstOrDefault().Value;

        //    var user = await _userManager.FindByIdAsync(subjectId);
        //    if (user == null)
        //        throw new ArgumentException("Invalid subject identifier");

        //    var claims = GetClaimsFromUser(user, context);
        //    context.IssuedClaims = claims.ToList();

        //    _logger.LogInformation("ProfileRequested: Client Name: {0} Requested Claims: {1} Caller: {2} Issued Claims: {3} ",
        //        context.Client.ClientName, context.RequestedClaimTypes, context.Caller, context.IssuedClaims);
        //}
        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var subject = context.Subject ?? throw new ArgumentNullException(nameof(context.Subject));

            var subjectId = subject.Claims.Where(x => x.Type == "sub").FirstOrDefault().Value;

            var user = await _userManager.FindByIdAsync(subjectId)
                ?? throw new ArgumentException("Invalid subject identifier");

            // claims from user.claims
            var additionalClaims = (await _userManager.GetClaimsAsync(user))
                .Where(x => context.RequestedClaimTypes.Contains(x.Type));

            // claims from user
            var userClaims = GetClaimsFromUser(user, context)
                .Where(x => !additionalClaims.Select(x => x.Type).Contains(x.Type));

            context.IssuedClaims = additionalClaims
                .Union(userClaims).ToList();

            _logger.LogInformation("ProfileRequested: Client Name: {0} Requested Claims: {1} User Claims: {2} Additional Claims: {3} Caller: {4} Issued Claims: {5} ",
                context.Client.ClientName, context.RequestedClaimTypes, userClaims, additionalClaims, context.Caller, context.IssuedClaims);
        }

        async public Task IsActiveAsync(IsActiveContext context)
        {
            var subject = context.Subject ?? throw new ArgumentNullException(nameof(context.Subject));

            var subjectId = subject.Claims.Where(x => x.Type == "sub").FirstOrDefault().Value;
            var user = await _userManager.FindByIdAsync(subjectId);

            context.IsActive = false;

            if (user != null)
            {
                if (_userManager.SupportsUserSecurityStamp)
                {
                    var security_stamp = subject.Claims.Where(c => c.Type == "security_stamp").Select(c => c.Value).SingleOrDefault();
                    if (security_stamp != null)
                    {
                        var db_security_stamp = await _userManager.GetSecurityStampAsync(user);
                        if (db_security_stamp != security_stamp)
                            return;
                    }
                }

                context.IsActive =
                    !user.LockoutEnabled ||
                    !user.LockoutEnd.HasValue ||
                    user.LockoutEnd <= DateTime.Now;
            }
        }

        private IEnumerable<Claim> GetClaimsFromUser(ApplicationUser user, ProfileDataRequestContext context)
        {
            //if (context.Caller == IdentityServerConstants.ProfileDataCallers.ClaimsProviderAccessToken)
            //{
            //    return new List<Claim>
            //    {
            //        new Claim(JwtClaimTypes.Subject, user.Id.ToString()),
            //        new Claim(JwtClaimTypes.Name, user.UserName),
            //        //new Claim(JwtClaimTypes.PreferredUserName, user.UserName),
            //        //new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName)
            //    };
            //}

            // context.Caller -> UserInfoEndpoint / ClaimsProviderIdentityToken 
            var claims = new List<Claim>();

            // note - matches profile scope defined in Config.cs
            foreach (var claim in context.RequestedClaimTypes)
            {
                switch (claim)
                {
                    case JwtClaimTypes.Subject:
                        claims.Add(new Claim(JwtClaimTypes.Subject, user.Id.ToString()));
                        break;
                    case JwtClaimTypes.Name:
                        claims.Add(new Claim(JwtClaimTypes.Name, user.UserName));
                        break;
                    case JwtClaimTypes.GivenName:
                        claims.Add(new Claim(JwtClaimTypes.GivenName, "x"));
                        break;
                    case JwtClaimTypes.PreferredUserName:
                        claims.Add(new Claim(JwtClaimTypes.PreferredUserName, user.UserName));
                        break;
                    case JwtClaimTypes.Email:
                        if (_userManager.SupportsUserEmail)
                        {
                            claims.Add(new Claim(JwtClaimTypes.Email, user.Email));
                        }
                        break;
                    case JwtClaimTypes.EmailVerified:
                        {
                            claims.Add(new Claim(JwtClaimTypes.EmailVerified, user.EmailConfirmed ? "true" : "false", ClaimValueTypes.Boolean));
                        }
                        break;
                    case JwtClaimTypes.PhoneNumber:
                        if (_userManager.SupportsUserPhoneNumber && !string.IsNullOrWhiteSpace(user.PhoneNumber))
                        {
                            claims.Add(new Claim(JwtClaimTypes.PhoneNumber, user.PhoneNumber));
                        }
                        break;
                    case JwtClaimTypes.PhoneNumberVerified:
                        if (_userManager.SupportsUserPhoneNumber && !string.IsNullOrWhiteSpace(user.PhoneNumber))
                        {
                            claims.Add(new Claim(JwtClaimTypes.PhoneNumberVerified, user.PhoneNumberConfirmed ? "true" : "false", ClaimValueTypes.Boolean));
                        }
                        break;
                }
            }

            //{
            //    new Claim(JwtClaimTypes.Subject, user.Id.ToString()),
            //    new Claim(JwtClaimTypes.Name, user.UserName),
            //    //new Claim(JwtClaimTypes.PreferredUserName, user.UserName),
            //    //new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName)
            //};

            //if (!string.IsNullOrWhiteSpace(user.Name))
            //    claims.Add(new Claim("name", user.Name));

            //if (!string.IsNullOrWhiteSpace(user.LastName))
            //    claims.Add(new Claim("last_name", user.LastName));

            //if (!string.IsNullOrWhiteSpace(user.CardNumber))
            //    claims.Add(new Claim("card_number", user.CardNumber));

            //if (!string.IsNullOrWhiteSpace(user.CardHolderName))
            //    claims.Add(new Claim("card_holder", user.CardHolderName));

            //if (!string.IsNullOrWhiteSpace(user.SecurityNumber))
            //    claims.Add(new Claim("card_security_number", user.SecurityNumber));

            //if (!string.IsNullOrWhiteSpace(user.Expiration))
            //    claims.Add(new Claim("card_expiration", user.Expiration));

            //if (!string.IsNullOrWhiteSpace(user.City))
            //    claims.Add(new Claim("address_city", user.City));

            //if (!string.IsNullOrWhiteSpace(user.Country))
            //    claims.Add(new Claim("address_country", user.Country));

            //if (!string.IsNullOrWhiteSpace(user.State))
            //    claims.Add(new Claim("address_state", user.State));

            //if (!string.IsNullOrWhiteSpace(user.Street))
            //    claims.Add(new Claim("address_street", user.Street));

            //if (!string.IsNullOrWhiteSpace(user.ZipCode))
            //    claims.Add(new Claim("address_zip_code", user.ZipCode));

            //if (_userManager.SupportsUserEmail)
            //{
            //    claims.AddRange(new[]
            //    {
            //        new Claim(JwtClaimTypes.Email, user.Email),
            //        new Claim(JwtClaimTypes.EmailVerified, user.EmailConfirmed ? "true" : "false", ClaimValueTypes.Boolean)
            //    });
            //}

            //if (_userManager.SupportsUserPhoneNumber && !string.IsNullOrWhiteSpace(user.PhoneNumber))
            //{
            //    claims.AddRange(new[]
            //    {
            //        new Claim(JwtClaimTypes.PhoneNumber, user.PhoneNumber),
            //        new Claim(JwtClaimTypes.PhoneNumberVerified, user.PhoneNumberConfirmed ? "true" : "false", ClaimValueTypes.Boolean)
            //    });
            //}

            return claims;
        }
    }
}
