using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.Circuits;
using System.Security.Claims;
using Ubik.Security.Contracts.Users.Results;

namespace Ubik.Accounting.WebApp.Security
{
    public class UserService(TokenCacheService cache
        , IHttpClientFactory factory)
    {
        private ClaimsPrincipal _currentUser = new(new ClaimsIdentity());
        private readonly HttpClient _httpClient = factory.CreateClient("UserServiceClient");

        public ClaimsPrincipal GetUser()
        {
            return _currentUser;
        }

        public async Task<string> GetTokenAsync()
        {
            var userEmail = (_currentUser.FindFirst(ClaimTypes.Email)?.Value)
                ?? throw new InvalidOperationException("User not authenticated");

            var token = await cache.GetUserTokenAsync(userEmail);

            if (token == null)
                return string.Empty;

            return token.AccessToken;
        }

        public async Task<UserAdminOrMeResult> GetUserInfo()
        {
            var userEmail = (_currentUser.FindFirst(ClaimTypes.Email)?.Value)
                ?? throw new InvalidOperationException("User not authenticated");

            var userInfo = await cache.GetUserInfoAsync(userEmail);

            if (userInfo == null)
            {
                var token = await GetTokenAsync();
                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", $"Bearer {token}");

                var response = await _httpClient.GetAsync("me/authinfo");
                var result = await response.Content.ReadFromJsonAsync<UserAdminOrMeResult>();

                if (response.IsSuccessStatusCode && result != null)
                {
                    await cache.SetUserInfoAsync(result);
                    return result;
                }
                else
                    throw new InvalidOperationException("Error getting user info");
            }
            else
                return userInfo;
        }

        internal void SetUser(ClaimsPrincipal user)
        {
            if (_currentUser != user)
            {
                _currentUser = user;
            }
        }

        internal sealed class UserCircuitHandler(
            AuthenticationStateProvider authenticationStateProvider,
            UserService userService) : CircuitHandler, IDisposable
        {
            private readonly AuthenticationStateProvider authenticationStateProvider = authenticationStateProvider;
            private readonly UserService userService = userService;

            public override Task OnCircuitOpenedAsync(Circuit circuit,
                CancellationToken cancellationToken)
            {
                authenticationStateProvider.AuthenticationStateChanged +=
                    AuthenticationChanged;

                return base.OnCircuitOpenedAsync(circuit, cancellationToken);
            }

            private void AuthenticationChanged(Task<AuthenticationState> task)
            {
                _ = UpdateAuthentication(task);

                async Task UpdateAuthentication(Task<AuthenticationState> task)
                {
                    try
                    {
                        var state = await task;
                        userService.SetUser(state.User);
                    }
                    catch
                    {
                    }
                }
            }

            public override async Task OnConnectionUpAsync(Circuit circuit,
                CancellationToken cancellationToken)
            {
                var state = await authenticationStateProvider.GetAuthenticationStateAsync();
                userService.SetUser(state.User);
            }

            public void Dispose()
            {
                authenticationStateProvider.AuthenticationStateChanged -=
                    AuthenticationChanged;
            }
        }
    }
}
