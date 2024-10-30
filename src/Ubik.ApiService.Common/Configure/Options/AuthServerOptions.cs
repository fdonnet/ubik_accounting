namespace Ubik.ApiService.Common.Configure.Options
{
    public class AuthServerOptions : IOptionsPosition
    {
        public const string Position = "AuthServer";
        public string MetadataAddress { get; set; } = string.Empty;
        public string Authority { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
        public bool RequireHttpsMetadata { get; set; } = true;
        public string AuthorizationUrl { get; set; } = string.Empty;
        public string TokenUrl { get; set; } = string.Empty;
        public string ClientId { get; set; } = string.Empty;
        public string ClientSecret { get; set; } = string.Empty;
        public int CookieRefreshTimeInMinutes { get; set; } = 0;
        public int RefreshTokenExpTimeInMinutes { get; set; } = 25;
        public bool AuthorizeBadCert { get; set; } = false;
    }
}

