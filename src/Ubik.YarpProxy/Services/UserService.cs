using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using System.Text.Json.Serialization;
using Ubik.Security.Contracts.Users.Results;

namespace Ubik.YarpProxy.Services
{
    //Todo: replace with hybrid cache
    public class UserService(HttpClient httpClient, IDistributedCache cache)
    {
        private static readonly JsonSerializerOptions _serializerOptions = new()
        {
            PropertyNamingPolicy = null,
            WriteIndented = true,
            AllowTrailingCommas = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        public async Task<UserAdminOrMeResult?> GetUserInfoAsync(string? email)
        {
            if (email == null) return null;

            //Try cache
            var user = await cache.GetAsync($"proxy_{email}");

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
                return JsonSerializer.Deserialize<UserAdminOrMeResult>(user, _serializerOptions);
            }
        }

        private async Task SetUserInfoInCacheAsync(UserAdminOrMeResult userInfo)
        {
            var toCache = JsonSerializer.SerializeToUtf8Bytes(userInfo, options: _serializerOptions);

            await cache.SetAsync($"proxy_{userInfo.Email}", toCache, new DistributedCacheEntryOptions { SlidingExpiration = TimeSpan.FromMinutes(60) });
        }
    }
}
