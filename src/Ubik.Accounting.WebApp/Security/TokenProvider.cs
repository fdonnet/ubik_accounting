namespace Ubik.Accounting.WebApp.Security
{
    public class TokenProvider
    {
        public string? AccessToken { get; init; }
        public string? RefreshToken { get; set; }
        public string? ExpiresAt { get; set; }
        public DateTimeOffset RefreshAt { get; set; }
    }
}
