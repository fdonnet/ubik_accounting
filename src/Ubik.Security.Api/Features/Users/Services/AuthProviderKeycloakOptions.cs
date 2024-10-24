using Ubik.ApiService.Common.Configure.Options;

namespace Ubik.Security.Api.Features.Users.Services
{
    public class AuthProviderKeycloakOptions : IOptionsPosition
    {
        public const string Position = "AuthManagerKeyCloakClient";
        public string ClientId { get; set; } = string.Empty;
        public string ClientSecret { get; set; } = string.Empty;
        public string RootUrl { get; set; } = string.Empty;
    }
}
