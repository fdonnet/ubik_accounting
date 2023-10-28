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
    }
}

