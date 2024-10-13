using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Ubik.Accounting.Api.Data;
using Microsoft.Extensions.Configuration;
using System.Text.Json.Serialization;
using LanguageExt.Pretty;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Ubik.Api.Tests.Integration
{
    [Collection("Proxy")]
    public abstract class BaseIntegrationTest : IDisposable
    {
        private readonly IServiceScope _scope;
        internal IntegrationTestProxyFactory Factory { get; }
        private HttpClient _authHttpClient;


        internal BaseIntegrationTest(IntegrationTestProxyFactory factory)
        {
            Factory = factory;
            _scope = Factory.Services.CreateScope();
            _authHttpClient = new HttpClient()
            { BaseAddress = new Uri("http://localhost:8086/realms/ubik/") };
        }

        internal async Task<string> GetAccessTokenAsync()
        {
            //TODO: hard coded, change that

            var dict = new Dictionary<string, string>
            {
                { "Content-Type", "application/x-www-form-urlencoded" },
                { "client_id", "ubik_app" },
                { "client_secret", "Ye6Y36ocA4SaGqYzd0HgmqMhVaM2jlkE" },
                { "username", "testrw@test.com" },
                { "password", "test" },
                { "grant_type", "password" },
                { "scope", "openid" },
            };


            HttpResponseMessage response = _authHttpClient.PostAsync($"protocol/openid-connect/token", new FormUrlEncodedContent(dict)).Result;
            if (response.IsSuccessStatusCode)
            {
                var token = await response.Content.ReadFromJsonAsync<GetTokenResult>();
                if(token != null)
                    return token.AccessToken;
            }

            throw new Exception("Cannot get auth access token to continue with testing.");
        }

#pragma warning disable CA1816 // Dispose methods should call SuppressFinalize
        public void Dispose()
#pragma warning restore CA1816 // Dispose methods should call SuppressFinalize
        {
            _scope?.Dispose();
        }

        private record GetTokenResult
        {
            [JsonPropertyName("access_token")]
            public string AccessToken { get; init; } = default!;
        }
    }
}
