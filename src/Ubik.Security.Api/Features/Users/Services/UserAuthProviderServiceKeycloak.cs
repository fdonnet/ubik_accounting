using LanguageExt;
using Microsoft.Extensions.Options;
using System.Text.Json;
using System.Text;
using System.Text.Json.Serialization;
using Ubik.ApiService.Common.Errors;
using Ubik.Security.Contracts.Users.Commands;
using Ubik.Security.Api.Features.Users.Errors;

namespace Ubik.Security.Api.Features.Users.Services
{
    public class UserAuthProviderServiceKeycloak(HttpClient httpClient, IOptions<AuthProviderKeycloakOptions> authProviderOption) : IUserAuthProviderService
    {
        private readonly HttpClient _httpClient = httpClient;
        private readonly AuthProviderKeycloakOptions _authProviderKeycloakOptions = authProviderOption.Value;

        public async Task<Either<IServiceAndFeatureError, bool>> AddUserAsync(AddUserCommand user)
        {
            return await GetServiceTokenAsync()
                .BindAsync(token => SendAddRequestToAuthProviderAsync(user, token));
        }

        public async Task<Either<IServiceAndFeatureError, bool>> CheckIfUsersPresentInAuth()
        {
            return await GetServiceTokenAsync()
                    .BindAsync(token => CheckIfUsersRequestToAuthProviderAsync(token));

        }

        private async Task<Either<IServiceAndFeatureError, bool>> SendAddRequestToAuthProviderAsync(AddUserCommand user, string token)
        {
            var userPayload = new AddUserInKeycloakRealm()
            {
                Email = user.Email,
                //TODO: change that a put in place a process to verify emails
                EmailVerified = true,
                Firstname = user.Firstname,
                Lastname = user.Lastname,
                Username = user.Email,
                Enabled = true,
                Credentials = [new() { Value = user.Password }]
            };

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

            var request = JsonSerializer.Serialize(userPayload);
            var response = await _httpClient.PostAsync("admin/realms/ubik/users"
                            , new StringContent(request, Encoding.UTF8, "application/json"));

            return response.IsSuccessStatusCode
                ? true
                : response.StatusCode == System.Net.HttpStatusCode.Conflict
                    ? new UserCannotBeAddedInAuthProviderConflict(user)
                    : new UserCannotBeAddedInAuthProviderBadParams(user);
        }

        private async Task<Either<IServiceAndFeatureError, bool>> CheckIfUsersRequestToAuthProviderAsync(string token)
        {
            var code = "{@code null}";

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

            var request = new HttpRequestMessage(HttpMethod.Get, "admin/realms/ubik/users/count")
            {
                Content = new StringContent(code, Encoding.UTF8, "application/json")

            };
            var response = await _httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();

                return result!="0";

            }
            else
                return new UserCannotCheckIfPresentInAuth();
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


            HttpResponseMessage response = _httpClient.PostAsync($"realms/ubik/protocol/openid-connect/token", new FormUrlEncodedContent(dict)).Result;
            if (response.IsSuccessStatusCode)
            {
                var token = await response.Content.ReadFromJsonAsync<GetTokenResult>();
                return token == null
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
            [JsonPropertyName("enabled")]
            public bool Enabled { get; init; } = true;
            [JsonPropertyName("username")]
            public string Username { get; init; } = default!;
            [JsonPropertyName("credentials")]
            public List<Credentials> Credentials { get; init; } = default!;
        }

        private record Credentials
        {
            [JsonPropertyName("temporary")]
            public bool Temporary { get; init; } = false;
            [JsonPropertyName("type")]
            public string Type { get; init; } = "password";
            [JsonPropertyName("value")]
            public string Value { get; init; } = default!;
        }
    }
}
