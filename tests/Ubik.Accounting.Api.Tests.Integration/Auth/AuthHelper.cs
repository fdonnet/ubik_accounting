using System.Net.Http.Json;

namespace Ubik.Accounting.Api.Tests.Integration.Auth
{
    internal static class AuthHelper
    {
        internal async static Task<string> GetAccessTokenReadOnly()
        {
            using var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, Environment.GetEnvironmentVariable("AuthServer__TokenUrl"));

            var collection = new List<KeyValuePair<string, string>>
            {
                new("grant_type", "password"),
                new("client_id", "ubik_accounting_api"),
                new("username", "testro"),
                new("password", "test"),
                new("client_secret", "GQEyHjeBUThKta1eItucb5LFGj5Hduwd")
            };

            var content = new FormUrlEncodedContent(collection);
            request.Content = content;

            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<TokenObjFromKeycloack>();

            return result!.AccessToken;
        }

        internal async static Task<string> GetAccessTokenReadWrite()
        {
            using var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, Environment.GetEnvironmentVariable("AuthServer__TokenUrl"));

            var collection = new List<KeyValuePair<string, string>>
            {
                new("grant_type", "password"),
                new("client_id", "ubik_accounting_api"),
                new("username", "testrw"),
                new("password", "test"),
                new("client_secret", "GQEyHjeBUThKta1eItucb5LFGj5Hduwd")
            };

            var content = new FormUrlEncodedContent(collection);
            request.Content = content;

            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<TokenObjFromKeycloack>();

            return result!.AccessToken;
        }

        internal async static Task<string> GetAccessTokenNoRole()
        {
            using var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, Environment.GetEnvironmentVariable("AuthServer__TokenUrl"));

            var collection = new List<KeyValuePair<string, string>>
            {
                new("grant_type", "password"),
                new("client_id", "ubik_accounting_api"),
                new("username", "testnorole"),
                new("password", "test"),
                new("client_secret", "GQEyHjeBUThKta1eItucb5LFGj5Hduwd")
            };

            var content = new FormUrlEncodedContent(collection);
            request.Content = content;

            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<TokenObjFromKeycloack>();

            return result!.AccessToken;
        }
    }
}
