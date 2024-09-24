using LanguageExt;
using LanguageExt.Pipes;
using MassTransit.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Text.Json;
using System.Text;
using System.Text.Json.Serialization;
using Ubik.ApiService.Common.Errors;
using Ubik.Security.Api.Models;
using Ubik.Security.Api.Features.Users.Errors;
using System.Reflection.Metadata.Ecma335;

namespace Ubik.Security.Api.Features.Users.Services
{
    public class UserAuthProviderServiceKeycloak(HttpClient httpClient, IOptions<AuthProviderKeycloakOptions> authProviderOption) : IUserAuthProviderService
    {
        private readonly HttpClient _httpClient = httpClient;
        private readonly AuthProviderKeycloakOptions _authProviderKeycloakOptions = authProviderOption.Value;

        public async Task<Either<IServiceAndFeatureError, bool>> AddUserAsync(User user)
        {
            return await GetServiceTokenAsync().ToAsync()
                .Bind(token => SendAddRequestToAuthProviderAsync(user, token).ToAsync()
                .Map(isOk =>
                {
                    return isOk;
                }));
        }

        private async Task<Either<IServiceAndFeatureError, bool>> SendAddRequestToAuthProviderAsync(User user, string token)
        {
            var userPayload = new AddUserInKeycloakRealm()
            {
                Email = user.Email,
                //TODO: change that a put in place a process to verify emails
                EmailVerified = true,
                Firstname = user.Firstname,
                Lastname = user.Lastname,
                Username = user.Email
            };

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

            var request = JsonSerializer.Serialize(userPayload);
            var response = await _httpClient.PostAsync("users"
                            , new StringContent(request, Encoding.UTF8, "application/json"));

            return response.IsSuccessStatusCode
            ? true
            : new UserCannotBeAddedInAuthProvider(user);
        }

        private async Task<Either<IServiceAndFeatureError, string>> GetServiceTokenAsync()
        {
            var dict = new Dictionary<string, string>
            {
                { "Content-Type", "application/x-www-form-urlencoded" },
                { "client_id", _authProviderKeycloakOptions.ClientId },
                { "client_secret", _authProviderKeycloakOptions.ClientSecret },
                { "grant_type", "client_credentials" }
            };


            HttpResponseMessage response = _httpClient.PostAsync($"protocol/openid-connect/token", new FormUrlEncodedContent(dict)).Result;
            if (response.IsSuccessStatusCode)
            {
                var token = await response.Content.ReadFromJsonAsync<GetTokenResult>();
                return token==null
                    ? new CannotGetAuthToken()
                    : token.AccessToken;
            }
            else
                return new CannotGetAuthToken();
        }

        private record GetTokenResult
        {
            [JsonPropertyName("access_token")]
            public string AccessToken { get; init; } = default!;
        }

        private record AddUserInKeycloakRealm
        {
            [JsonPropertyName("email")]
            public string Email { get; init; } = default!;
            [JsonPropertyName("firstName")]
            public string Firstname { get; init; } = default!;
            [JsonPropertyName("lastName")]
            public string Lastname { get; init; } = default!;
            [JsonPropertyName("emailVerified")]
            public bool EmailVerified { get; init; } = default!;
            [JsonPropertyName("username")]
            public string Username { get; init; } = default!;
        }
    }
}
