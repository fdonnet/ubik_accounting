namespace Ubik.Accounting.WebApp.Security
{
    public record TokenProvider
    {
        public string? AccessToken { get; init; }
        public string? RefreshToken { get; init; }
    }
}
