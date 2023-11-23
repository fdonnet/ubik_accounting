using Microsoft.AspNetCore.Components.Authorization;

namespace Ubik.Accounting.WebApp.Security
{
    public class AuthenticationStateHandler : DelegatingHandler
    {
        private readonly CircuitServicesAccessor _circuitServicesAccessor;


        public AuthenticationStateHandler(CircuitServicesAccessor circuitServicesAccessor)
        {
            _circuitServicesAccessor = circuitServicesAccessor;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var userService = _circuitServicesAccessor.Services!
                .GetRequiredService<UserService>();
           // var authState = await authStateProvider.GetAuthenticationStateAsync();
            //var user = authState.User;
            var user = userService.GetUser();

            if (user.Identity is not null && user.Identity.IsAuthenticated)
            {
                request.Headers.Add("Authorization", $"Bearer {await userService.GetTokenAsync()}");
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
