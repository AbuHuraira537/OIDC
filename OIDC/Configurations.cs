using IdentityModel;
using IdentityServer4.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OIDC
{
    public static class Configurations
    {
        public static IEnumerable<IdentityResource> GetIdentityResources() => new List<IdentityResource>
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
            
        };
        public static IEnumerable<ApiResource> GetApis() => 
            new List<ApiResource>
            {
               new ApiResource()
               {
                   Enabled=true,
                   Scopes = { "Application-Management" },
                   Name="Application-Management",
                   DisplayName = "Application Management Api",
                   ShowInDiscoveryDocument = true
               }

              
            
            };
        public static IEnumerable<Client> GetClients() => new List<Client>
        {
           
            new Client()
            {
                ClientId = "clientIdMVC",
                ClientSecrets = {new Secret("client_secret".ToSha256()) },
                AllowedGrantTypes = GrantTypes.Code,
                RedirectUris = { "https://localhost:44347/signin-oidc" },
                AllowedScopes = {
                    "Application-Management", 
                    OidcConstants.StandardScopes.Profile ,
                    OidcConstants.StandardScopes.OpenId, 
                },
                AllowOfflineAccess = true,
                PostLogoutRedirectUris = { "https://localhost:44347/Home/Index" }

            }
           

        };
        public static IEnumerable<ApiScope> GetApiScopes() =>
            new List<ApiScope>
            {
                new ApiScope("Application-Management"),
            };
    }
}
