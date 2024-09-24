using Ubik.ApiService.Common.Configure.Options;

namespace Ubik.Security.Api.Features.Users.Services
{
    public class AuthProviderOptions : IOptionsPosition
    {
        public const string Position = "AuthSProvider";
        public string ClientId { get; set; } = string.Empty;
        public string ClientSecret { get; set; } = string.Empty;
        public string RootUrl { get; set; } = string.Empty;
    }
}
