using IdentityModel.Client;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;
using System.Text.Json.Serialization;
using Ubik.Accounting.WebApp.Shared.Security;
using Ubik.ApiService.Common.Configure.Options;
using Ubik.Security.Contracts.Users.Results;

namespace Ubik.Accounting.WebApp.Security
{
    public class TokenCacheService(IDistributedCache cache, IOptions<AuthServerOptions> authOptions, IHttpClientFactory factory)
    {
        private readonly IDistributedCache _cache = cache;
        private readonly AuthServerOptions _authOptions = authOptions.Value;
        private readonly HttpClient _httpClient = factory.CreateClient("TokenClient");

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

            cachedResult = await RefreshTokenAsync(cachedResult, userId);

            return cachedResult;
        }

        private async Task<TokenCacheEntry?> RefreshTokenAsync(TokenCacheEntry actualToken, string userId)
        {

            if (actualToken.ExpiresUtc > DateTimeOffset.UtcNow.AddSeconds(10))
            {
                //No need to refresh
                return actualToken;
            }
            else
            {
                if (actualToken.ExpiresRefreshUtc > DateTimeOffset.UtcNow.AddSeconds(10))
                {
                    //Can try a refresh
                    var dict = ValuesForRefresh(actualToken.RefreshToken);
                    HttpResponseMessage response = await _httpClient.PostAsync("", new FormUrlEncodedContent(dict));

                    if (response.IsSuccessStatusCode)
                    {
                        var token = await ProtocolResponse.FromHttpResponseAsync<TokenResponse>(response);

                        if (token != null)
                        {
                            var newToken = new TokenCacheEntry
                            {
                                UserId = userId,
                                RefreshToken = token.RefreshToken!,
                                AccessToken = token.AccessToken!,
                                ExpiresUtc = new JwtSecurityToken(token.AccessToken).ValidTo,
                                ExpiresRefreshUtc = DateTimeOffset.UtcNow.AddMinutes(_authOptions.RefreshTokenExpTimeInMinutes)
                            };

                            //Refresh successful
                            await SetUserTokenAsync(newToken);
                            return newToken;
                        }
                    }
                }
                //Too old to refresh or refresh not successful
                await RemoveUserTokenAsync(userId);
                return null;
            }
        }

        private Dictionary<string, string> ValuesForRefresh(string token)
        {
            return new Dictionary<string, string>
            {
                { "Content-Type", "application/x-www-form-urlencoded" },
                { "client_id", _authOptions.ClientId },
                { "client_secret", _authOptions.ClientSecret },
                { "refresh_token", token },
                { "grant_type", "refresh_token" },
            };
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
