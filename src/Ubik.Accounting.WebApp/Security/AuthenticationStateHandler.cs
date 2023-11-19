using Microsoft.AspNetCore.Components.Authorization;

namespace Ubik.Accounting.WebApp.Security
{
    public class AuthenticationStateHandler(
        CircuitServicesAccessor circuitServicesAccessor) : DelegatingHandler
    {
        readonly CircuitServicesAccessor circuitServicesAccessor = circuitServicesAccessor;

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var authStateProvider = circuitServicesAccessor.Services!
                .GetRequiredService<AuthenticationStateProvider>();
            var authState = await authStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;

            if (user.Identity is not null && user.Identity.IsAuthenticated)
            {
                request.Headers.Add("Authorization", $"Bearer ");
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
