using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.Circuits;
using System.Security.Claims;

namespace Ubik.Accounting.WebApp.Security
{
    public class UserService(TokenCacheService cache)
    {
        private ClaimsPrincipal _currentUser = new(new ClaimsIdentity());
        private readonly TokenCacheService _cache = cache;

        public ClaimsPrincipal GetUser()
        {
            return _currentUser;
        }

        public async Task<string> GetTokenAsync()
        {
            return (await _cache.GetUserTokenAsync(_currentUser.FindFirst(ClaimTypes.Email)?.Value))?.AccessToken ?? string.Empty;
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
