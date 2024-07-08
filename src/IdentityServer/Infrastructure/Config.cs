using Duende.IdentityServer;
using Duende.IdentityServer.Models;

namespace IdentityServer.Infrastructure;

public static class Config
{
    public static IEnumerable<IdentityResource> IdentityResources =>
        new IdentityResource[]
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
            new IdentityResources.Email(),
            new IdentityResource("roles", "User roles", new[] {"Basic"})
        };

    public static IEnumerable<ApiScope> ApiScopes =>
        new ApiScope[]
        { 
            new ApiScope("api1", "API 1"),
            new ApiScope("api2", "API 2"),
            new ApiScope("api3", "API 3"),
        };

    public static IEnumerable<ApiResource> ApiResources =>
        new List<ApiResource>
        {
            new ApiResource("api1", "API 1")
            {
                Scopes = { "api1" }
            },
            new ApiResource("api2", "API 2")
            {
                Scopes = { "api2" }
            },
            new ApiResource("api3", "API 3")
            {
                Scopes = { "api3" }
            }
        };

    public static IEnumerable<Client> Clients =>
        new Client[]
        {
             new Client
            {
                ClientId = "client",
                ClientSecrets = { new Secret("secret".Sha256()) },
                //AllowedGrantTypes = GrantTypes.ResourceOwnerPasswordAndClientCredentials,
                AllowedGrantTypes = GrantTypes.Code,
                //AllowedScopes = { "openid", "profile", "roles", "api1", "api2", "api3" },
                RedirectUris = { "https://localhost:5002/signin-oidc" },
                PostLogoutRedirectUris = { "https://localhost:5002/signout-callback-oidc" },
                AllowOfflineAccess = true,
                AllowedScopes=
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    "api1"
                }
            }
        };
}