using Duende.IdentityServer.Models;

namespace AbjjadMicroblogging.IdentityServerAPI
{
    public static class Config
    {
        public static IEnumerable<Client> Clients =>
            new List<Client>
            {
            new Client
            {
                ClientId = "web-client",
                AllowedGrantTypes = GrantTypes.Code,
                RequirePkce = true,
                RequireClientSecret = false, 
                RedirectUris = { "https://localhost:5001/callback" },
                PostLogoutRedirectUris = { "https://localhost:5001/signout-callback" },
                AllowedScopes = { "api1", "openid", "profile" }
            }
            };

        public static IEnumerable<ApiScope> ApiScopes =>
            new List<ApiScope>
            {
            new ApiScope("api1", "My API")
            };

        public static IEnumerable<ApiResource> ApiResources =>
            new List<ApiResource>
            {
            new ApiResource("api1", "My API")
            {
                Scopes = { "api1" }
            }
            };

        public static IEnumerable<IdentityResource> IdentityResources =>
            new List<IdentityResource>
            {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile()
            };
    }
}
