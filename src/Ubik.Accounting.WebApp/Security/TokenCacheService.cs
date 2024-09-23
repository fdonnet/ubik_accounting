using MassTransit.Configuration;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using System.Text.Json;
using System.Text.Json.Serialization;
using Ubik.ApiService.Common.Configure.Options;

namespace Ubik.Accounting.WebApp.Security
{
    public class TokenCacheService(IDistributedCache cache, IOptions<AuthServerOptions> authOptions)
    {
        private readonly IDistributedCache _cache = cache;
        private readonly AuthServerOptions _authOptions = authOptions.Value;
        private static readonly JsonSerializerOptions _serializerOptions = new()
        {
            PropertyNamingPolicy = null,
            WriteIndented = true,
            AllowTrailingCommas = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };


        public async Task SetUserTokenAsync(TokenCacheEntry token)
        {
            var toCache = JsonSerializer.SerializeToUtf8Bytes(token, options: _serializerOptions);

            await _cache.SetAsync(token.UserId, toCache, new DistributedCacheEntryOptions { SlidingExpiration = TimeSpan.FromMinutes(_authOptions.CookieRefreshTimeInMinutes + 60) });
        }

        public async Task<TokenCacheEntry?> GetUserTokenAsync(string? userId)
        {
            if (userId == null) return null;

            var token = await _cache.GetAsync(userId);

            if (token == null) return null;

            var cachedResult = JsonSerializer.Deserialize<TokenCacheEntry>(token, _serializerOptions);

            return cachedResult;
        }
    }

   public class TokenCacheEntry
    {
        public string UserId { get; set; } = default!;
        public string AccessToken { get; set; } = default!;
        public string RefreshToken { get; set; } = default!;
        public DateTimeOffset ExpiresUtc { get; set; } = default!;
    }
}
