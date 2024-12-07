using IdentityModel.Client;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;
using System.Text.Json.Serialization;
using Ubik.Accounting.WebApp.Shared.Security;
using Ubik.ApiService.Common.Configure.Options;
using Ubik.Security.Contracts.Users.Results;

namespace Ubik.Accounting.WebApp.Security
{
    public class TokenCacheService(HybridCache cache, IOptions<AuthServerOptions> authOptions, IHttpClientFactory factory)
    {
        private readonly AuthServerOptions _authOptions = authOptions.Value;
        private readonly HttpClient _httpClient = factory.CreateClient("TokenClient");

        public async Task RemoveUserTokenAsync(string key)
        {
            await cache.RemoveAsync($"webapp_{key}");
        }

        public async Task SetUserTokenAsync(TokenCacheEntry token)
        {
            await cache.SetAsync($"webapp_{token.UserId}", token, new HybridCacheEntryOptions()
            {
                Expiration = TimeSpan.FromMinutes(120),
                LocalCacheExpiration = TimeSpan.FromSeconds(10)
            });
        }

        public async Task<TokenCacheEntry?> GetUserTokenAsync(string? userId)
        {
            if (userId == null) return null;

            var token = await cache.GetOrCreateAsync<TokenCacheEntry?>($"webapp_{userId}", factory: null!,
                    new HybridCacheEntryOptions() { Flags = HybridCacheEntryFlags.DisableUnderlyingData });

            if (token == null) return null;

            token = await RefreshTokenAsync(token, userId);

            return token;
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
            await cache.SetAsync($"webapp_{userInfo.Email}_auth", userInfo, new HybridCacheEntryOptions()
            {
                Expiration = TimeSpan.FromMinutes(_authOptions.CookieRefreshTimeInMinutes + 120),
                LocalCacheExpiration = TimeSpan.FromSeconds(15)
            });

        }

        public async Task<UserAdminOrMeResult?> GetUserInfoAsync(string? userEmail)
        {
            if (userEmail == null) return null;

            var user = await cache.GetOrCreateAsync<UserAdminOrMeResult?>($"webapp_{userEmail}_auth", factory: null!,
                new HybridCacheEntryOptions() { Flags = HybridCacheEntryFlags.DisableUnderlyingData });

            return user ?? null;
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
