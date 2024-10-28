﻿using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using System.Text.Json;
using System.Text.Json.Serialization;
using Ubik.Accounting.WebApp.Shared.Security;
using Ubik.ApiService.Common.Configure.Options;
using Ubik.Security.Contracts.Users.Results;

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

        public async Task RemoveUserTokenAsync(string key)
        {
            await _cache.RemoveAsync($"webapp_{key}");
        }

        public async Task SetUserTokenAsync(TokenCacheEntry token)
        {
            var toCache = JsonSerializer.SerializeToUtf8Bytes(token, options: _serializerOptions);

            await _cache.SetAsync($"webapp_{token.UserId}", toCache,
                new DistributedCacheEntryOptions { SlidingExpiration = TimeSpan.FromMinutes(120) });
        }

        public async Task<TokenCacheEntry?> GetUserTokenAsync(string? userId)
        {
            if (userId == null) return null;

            var token = await _cache.GetAsync($"webapp_{userId}");

            if (token == null) return null;

            var cachedResult = JsonSerializer.Deserialize<TokenCacheEntry>(token, _serializerOptions);

            if (cachedResult == null) return null;

            if (cachedResult.ExpiresUtc < DateTimeOffset.UtcNow.AddSeconds(10))
            {
                if (cachedResult.ExpiresRefreshUtc < DateTimeOffset.UtcNow.AddSeconds(10))
                {
                    await RemoveUserTokenAsync($"webapp_{userId}");
                    return null;
                }
            }

            return cachedResult;
        }

        public async Task SetUserInfoAsync(UserAdminOrMeResult userInfo)
        {
            var toCache = JsonSerializer.SerializeToUtf8Bytes(userInfo, options: _serializerOptions);

            await _cache.SetAsync($"webapp_{userInfo.Email}_auth", toCache, new DistributedCacheEntryOptions { SlidingExpiration = TimeSpan.FromMinutes(_authOptions.CookieRefreshTimeInMinutes + 120) });
        }

        public async Task<UserAdminOrMeResult?> GetUserInfoAsync(string? userEmail)
        {
            if (userEmail == null) return null;

            var user = await _cache.GetAsync($"webapp_{userEmail}_auth");

            if (user == null) return null;

            var cachedResult = JsonSerializer.Deserialize<UserAdminOrMeResult>(user, _serializerOptions);

            return cachedResult;
        }
    }

    public class TokenCacheEntry
    {
        public string UserId { get; set; } = default!;
        public string AccessToken { get; set; } = default!;
        public string RefreshToken { get; set; } = default!;
        public DateTimeOffset ExpiresUtc { get; set; } = default!;
        public DateTimeOffset ExpiresRefreshUtc { get; set; } = default!;
    }
}
