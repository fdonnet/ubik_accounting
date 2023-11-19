using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.Circuits;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace Ubik.Accounting.WebApp.Security
{
    public class UserService
    {
        private ClaimsPrincipal _currentUser = new(new ClaimsIdentity());
        private string _token = string.Empty;


        public ClaimsPrincipal GetUser()
        {
            return _currentUser;
        }

        public string GetToken()
        {
            return _token;
        }

        internal void SetUser(ClaimsPrincipal user)
        {
            if (_currentUser != user)
            {
                _currentUser = user;
            }
        }

        internal void SetToken(string token)
        {
            if (_token != token)
            {
                _token = token;
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
                userService.SetToken(await authenticationStateProvider.GetT("access_token"));
            }

            public void Dispose()
            {
                authenticationStateProvider.AuthenticationStateChanged -=
                    AuthenticationChanged;
            }
        }
    }
}
