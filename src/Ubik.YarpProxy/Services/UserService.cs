using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Hybrid;
using System.Text.Json;
using System.Text.Json.Serialization;
using Ubik.Security.Contracts.Users.Results;

namespace Ubik.YarpProxy.Services
{
    public class UserService(HttpClient httpClient, HybridCache cache)
    {
        public async Task<UserAdminOrMeResult?> GetUserInfoAsync(string? email)
        {
            if (email == null) return null;

            var user = await cache.GetOrCreateAsync<UserAdminOrMeResult?>($"proxy_{email}", factory: null!,
            new HybridCacheEntryOptions() { Flags = HybridCacheEntryFlags.DisableUnderlyingData });

            if (user == null)
            {
                //TODO: hard coded megaAdmin user_ID => change that
                httpClient.DefaultRequestHeaders.Add("x-user-id", "5c5e0000-3c36-7456-b9da-08dcdf9832e2");
                var response = await httpClient.GetAsync($"admin/api/v1/users?email={email}");

                if (response.IsSuccessStatusCode)
                {
                    try
                    {
                        var userInfo = await response.Content.ReadFromJsonAsync<UserAdminOrMeResult>();

                        if (userInfo == null)
                            return null;
                        else
                        {
                            await SetUserInfoInCacheAsync(userInfo);
                            return userInfo;
                        }
                    }
                    catch
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
            else
            {
                //Cache
                return user;
            }
        }

        private async Task SetUserInfoInCacheAsync(UserAdminOrMeResult userInfo)
        {
            await cache.SetAsync($"proxy_{userInfo.Email}", userInfo, new HybridCacheEntryOptions()
                {
                    Expiration = TimeSpan.FromMinutes(5),
                    LocalCacheExpiration = TimeSpan.FromSeconds(15)
                });
        }
    }
}
