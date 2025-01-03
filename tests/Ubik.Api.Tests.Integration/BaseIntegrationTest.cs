﻿using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace Ubik.Api.Tests.Integration
{
    enum TokenType
    {
        MegaAdmin,
        RW,
        RO,
        NoRole,
        OtherTenant
    }

    [Collection("Proxy")]
    public abstract class BaseIntegrationTest : IDisposable
    {
        private readonly IServiceScope _scope;
        internal IntegrationTestProxyFactory Factory { get; }
        private readonly HttpClient _authHttpClient;

        //TODO: change that
        internal BaseIntegrationTest(IntegrationTestProxyFactory factory)
        {
            Factory = factory;
            _scope = Factory.Services.CreateScope();
            _authHttpClient = new HttpClient()
            { BaseAddress = new Uri("http://localhost:8080/realms/ubik/") };
            CleanupDb().Wait();
        }

        protected abstract Task CleanupDb();


        internal async Task<string> GetAccessTokenAsync(TokenType tokenType)
        {
            var dict = new Dictionary<string, string>();
            switch (tokenType)
            {
                case TokenType.MegaAdmin:
                    dict = ValuesForMegaAdmin();
                    break;
                case TokenType.RW:
                    dict = ValuesForTestRW();
                    break;
                case TokenType.RO:
                    dict = ValuesForTestRO();
                    break;
                case TokenType.NoRole:
                    dict = ValuesForTestNoRole();
                    break;
                case TokenType.OtherTenant:
                    dict = ValuesForOtherTenantUser();
                    break;
            }

            HttpResponseMessage response = await _authHttpClient.PostAsync($"protocol/openid-connect/token", new FormUrlEncodedContent(dict));
            if (response.IsSuccessStatusCode)
            {
                var token = await response.Content.ReadFromJsonAsync<GetTokenResult>();
                if (token != null)
                    return token.AccessToken;
            }

            throw new Exception("Cannot get auth access token to continue with testing.");
        }

        private static Dictionary<string, string> ValuesForTestRW()
        {
            return new Dictionary<string, string>
            {
                { "Content-Type", "application/x-www-form-urlencoded" },
                { "client_id", "ubik_app" },
                { "client_secret", "Ye6Y36ocA4SaGqYzd0HgmqMhVaM2jlkE" },
                { "username", "testrw@test.com" },
                { "password", "test" },
                { "grant_type", "password" },
                { "scope", "openid" },
            };
        }

        private static Dictionary<string, string> ValuesForTestRO()
        {
            return new Dictionary<string, string>
            {
                { "Content-Type", "application/x-www-form-urlencoded" },
                { "client_id", "ubik_app" },
                { "client_secret", "Ye6Y36ocA4SaGqYzd0HgmqMhVaM2jlkE" },
                { "username", "testro@test.com" },
                { "password", "test" },
                { "grant_type", "password" },
                { "scope", "openid" },
            };
        }

        private static Dictionary<string, string> ValuesForTestNoRole()
        {
            return new Dictionary<string, string>
            {
                { "Content-Type", "application/x-www-form-urlencoded" },
                { "client_id", "ubik_app" },
                { "client_secret", "Ye6Y36ocA4SaGqYzd0HgmqMhVaM2jlkE" },
                { "username", "testnorole@test.com" },
                { "password", "test" },
                { "grant_type", "password" },
                { "scope", "openid" },
            };
        }

        private static Dictionary<string, string> ValuesForMegaAdmin()
        {
            return new Dictionary<string, string>
            {
                { "Content-Type", "application/x-www-form-urlencoded" },
                { "client_id", "ubik_app" },
                { "client_secret", "Ye6Y36ocA4SaGqYzd0HgmqMhVaM2jlkE" },
                { "username", "admin@test.com" },
                { "password", "test" },
                { "grant_type", "password" },
                { "scope", "openid" },
            };
        }

        private static Dictionary<string, string> ValuesForOtherTenantUser()
        {
            return new Dictionary<string, string>
            {
                { "Content-Type", "application/x-www-form-urlencoded" },
                { "client_id", "ubik_app" },
                { "client_secret", "Ye6Y36ocA4SaGqYzd0HgmqMhVaM2jlkE" },
                { "username", "testothertenant@test.com" },
                { "password", "test" },
                { "grant_type", "password" },
                { "scope", "openid" },
            };
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
