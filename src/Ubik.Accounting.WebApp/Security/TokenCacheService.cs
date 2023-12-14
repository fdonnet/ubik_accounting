using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Ubik.Accounting.WebApp.Security
{
    public class TokenCacheService(IDistributedCache cache)
    {
        private readonly IDistributedCache _cache = cache;
        private static readonly JsonSerializerOptions _serializerOptions = new()
        {
            PropertyNamingPolicy = null,
            WriteIndented = true,
            AllowTrailingCommas = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };


        public async Task SetUserTokenAsync(TokenCacheEntry token)
        {
            //if (await GetUserTokenAsync(token.UserId) != null)
            //    await _cache.RemoveAsync(token.UserId);

            var toCache = JsonSerializer.SerializeToUtf8Bytes(token, options: _serializerOptions);

            await _cache.SetAsync(token.UserId, toCache, new DistributedCacheEntryOptions { SlidingExpiration = TimeSpan.FromMinutes(30) });
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
