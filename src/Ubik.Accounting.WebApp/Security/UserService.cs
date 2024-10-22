using IdentityModel.Client;
using MassTransit.Configuration;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.Circuits;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Ubik.ApiService.Common.Configure.Options;

namespace Ubik.Accounting.WebApp.Security
{
    public class UserService(TokenCacheService cache, IOptions<AuthServerOptions> authOptions, I)
    {
        private ClaimsPrincipal _currentUser = new(new ClaimsIdentity());

        public ClaimsPrincipal GetUser()
        {
            return _currentUser;
        }

        public async Task<string> GetTokenAsync()
        {
            var userId = _currentUser.FindFirst(ClaimTypes.Email)?.Value;

            if (userId == null)
                throw new InvalidOperationException("User not authenticated");

            var token = await cache.GetUserTokenAsync(userId);

            if (token == null)
                return string.Empty;

            if(token.ExpiresUtc > DateTimeOffset.UtcNow)
            {
                var response = await new HttpClient().RequestRefreshTokenAsync(new RefreshTokenRequest
                {
                    Address = authOptions.Value.TokenUrl,
                    ClientId = authOptions.Value.ClientId,
                    ClientSecret = authOptions.Value.ClientSecret,
                    RefreshToken = token.RefreshToken,
                    GrantType = "refresh_token",
                });

                if (!response.IsError)
                {
                    await cache.SetUserTokenAsync(new TokenCacheEntry
                    {
                        UserId = userId,
                        RefreshToken = response.RefreshToken!,
                        AccessToken = response.AccessToken!,
                        ExpiresUtc = new JwtSecurityToken(response.AccessToken).ValidTo
                    });
                }
                else
                    throw new InvalidOperationException("Error refreshing token");
            }

            return token.AccessToken;
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
