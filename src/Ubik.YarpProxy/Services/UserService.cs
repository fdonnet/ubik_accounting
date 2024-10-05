using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using System.Text.Json.Serialization;
using Ubik.ApiService.Common.Errors;
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

        public async Task<UserAdminResult?> GetUserInfoAsync(string email)
        {
            //Try cache
            var user = await cache.GetAsync(email);

            if (user == null)
            {
                //No cache
                var response = await httpClient.GetAsync($"admin/api/v1/users?email={email}");

                if (response.IsSuccessStatusCode)
                {
                    try
                    {
                        var userInfo = await response.Content.ReadFromJsonAsync<UserAdminResult>();

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
                return JsonSerializer.Deserialize<UserAdminResult>(user, _serializerOptions);
            }
        }

        private async Task SetUserInfoInCacheAsync(UserAdminResult userInfo)
        {
            var toCache = JsonSerializer.SerializeToUtf8Bytes(userInfo, options: _serializerOptions);

            await cache.SetAsync(userInfo.Email, toCache, new DistributedCacheEntryOptions { SlidingExpiration = TimeSpan.FromMinutes(60) });
        }
    }
}
