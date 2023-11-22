using Google.Protobuf.WellKnownTypes;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json.Linq;
using System.Text;
using System.Text.Json;

namespace Ubik.Accounting.WebApp.Security
{
    public class TokenCacheService
    {
        private readonly IDistributedCache _cache;
        public TokenCacheService(IDistributedCache cache)
        {
            _cache = cache;
        }

        public async Task SetUserToken(TokenCacheEntry token)
        {
            if (await GetUserToken(token.UserId) != null)
                await _cache.RemoveAsync(token.UserId);

            var bytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(token));
            await _cache.SetAsync(token.UserId, bytes, new DistributedCacheEntryOptions { SlidingExpiration=TimeSpan.FromMinutes(30)});
        }

        public async Task<TokenCacheEntry?> GetUserToken(string userId)
        {
            var tokenString = await _cache.GetAsync(userId);

            return tokenString != null ? JsonSerializer.Deserialize<TokenCacheEntry>(tokenString) : null;
        }
    }

    public class TokenCacheEntry
    {
        public string UserId = default!;
        public string AccessToken = default!;
        public string RefreshToken = default!;
        public DateTimeOffset ExpiresUtc = default!;
    }
}
