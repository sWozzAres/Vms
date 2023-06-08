﻿// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using System.Collections.Generic;

namespace Identity.API
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                new IdentityResource(IdentityServerConstants.StandardScopes.Profile,
                    "Profile data",
                    new[] {
                        // mapped to ApplicationUser
                        JwtClaimTypes.PreferredUserName, // UserName
                        JwtClaimTypes.Name,              // UserName if no 'name' claim present
                        //JwtClaimTypes.Email,
                        //JwtClaimTypes.EmailVerified,
                        //JwtClaimTypes.PhoneNumber,
                        //JwtClaimTypes.PhoneNumberVerified,

                        //JwtClaimTypes.GivenName,
                        //JwtClaimTypes.FamilyName,
                        //JwtClaimTypes.WebSite,
                        //JwtClaimTypes.Address,
                        //"myclaim1"
                        "tenantid"
                    }),
                new IdentityResource(IdentityServerConstants.StandardScopes.Email,
                    "Email data",
                    new[]
                    {
                        JwtClaimTypes.Email,
                        JwtClaimTypes.EmailVerified
                    }),
                new IdentityResource(IdentityServerConstants.StandardScopes.Phone,
                    "Phone data",
                    new[]
                    {
                        JwtClaimTypes.PhoneNumber,
                        JwtClaimTypes.PhoneNumberVerified
                    }),
                //new IdentityResource(IdentityServerConstants.StandardScopes.Address,
                //    "Address data",
                //    new[]
                //    {
                //        JwtClaimTypes.Address,
                //    })
            };
        }
        public static IEnumerable<ApiScope> ApiScopes =>
            new ApiScope[]
            {
                new ApiScope("catalog.read", "Read the catalog"),
                new ApiScope("catalog.write", "Write the catalog"),
                new ApiScope("utopia.read", "Read Utopia", new [] { JwtClaimTypes.Name }),
                new ApiScope("utopia.write", "Write Utopia"),

                new ApiScope("vms.admin", "Vms Admin", new [] { "tenantid" }),
                new ApiScope("vms.client", "Vms Client", new [] { "tenantid" })
            };

        /// <summary>
        /// Determines which claims get sent to API
        /// </summary>
        public static IEnumerable<ApiResource> ApiResources =>
            new List<ApiResource>
            {
                new ApiResource("catalog", "Catalog Service")
                {
                    Scopes = { "catalog.read", "catalog.write" },
                    UserClaims = { JwtClaimTypes.Name }
                },
                new ApiResource("utopia", "Utopia Service")
                {
                    Scopes = { "utopia.read", "utopia.write" },
                    UserClaims = { JwtClaimTypes.Name }
                },
                new ApiResource("vmsadmin", "Vms Admin Service")
                {
                    Scopes = { "vms.admin" },
                    UserClaims = { JwtClaimTypes.Name, "tenantid" }
                },
                new ApiResource("vmsclient", "Vms Client Service")
                {
                    Scopes = { "vms.client" },
                    UserClaims = { JwtClaimTypes.Name, "tenantid" }
                }
                //new ApiResource("invoice", "Invoice API")
                //{
                //    Scopes = { "invoice.read", "invoice.pay", "manage" }
                //},

                //new ApiResource("customer", "Customer API")
                //{
                //    Scopes = { "customer.read", "customer.contact", "manage" }
                //}
            };

        public static IEnumerable<Client> GetClients(Dictionary<string, string> clientsUrl)
        {
            return new List<Client>
            {
                new Client
                {
                    // unique name for the application
                    ClientId = "vmsadmin",

                    // Authorization Code grant type and require PKCE
                    // http://docs.identityserver.io/en/latest/topics/grant_types.html
                    AllowedGrantTypes = GrantTypes.Code,
                    RequirePkce = true,

                    // turn off secret for public client
                    RequireClientSecret = false,

                    // whitelisted URL in CORS
                    AllowedCorsOrigins = { $"{clientsUrl["Admin"]}" },

                    // enabling openid and profile as scopes, in order to allow the execution of the OpenID Connect flow 
                    // and retrieve the profile of the user in the ID Token
                    AllowedScopes = {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        "vms.admin"
                    },

                    AlwaysIncludeUserClaimsInIdToken = true,

                    RedirectUris = { $"{clientsUrl["Admin"]}/authentication/login-callback" },
                    PostLogoutRedirectUris = { $"{clientsUrl["Admin"]}/" }
                },
                new Client
                {
                    // unique name for the application
                    ClientId = "vmsclient",

                    // Authorization Code grant type and require PKCE
                    // http://docs.identityserver.io/en/latest/topics/grant_types.html
                    AllowedGrantTypes = GrantTypes.Code,
                    RequirePkce = true,

                    // turn off secret for public client
                    RequireClientSecret = false,

                    // whitelisted URL in CORS
                    AllowedCorsOrigins = { $"{clientsUrl["Client"]}" },

                    // enabling openid and profile as scopes, in order to allow the execution of the OpenID Connect flow 
                    // and retrieve the profile of the user in the ID Token
                    AllowedScopes = {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        "vms.client"
                    },

                    AlwaysIncludeUserClaimsInIdToken = true,

                    RedirectUris = { $"{clientsUrl["Client"]}/authentication/login-callback" },
                    PostLogoutRedirectUris = { $"{clientsUrl["Client"]}/" }
                }
            };
        }
    }
}