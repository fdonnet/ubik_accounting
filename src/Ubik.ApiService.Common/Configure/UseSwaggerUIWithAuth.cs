using Microsoft.AspNetCore.Builder;
using Ubik.ApiService.Common.Configure.Options;

namespace Ubik.ApiService.Common.Configure
{
    public static class AppBuildSwaggerUIWithAuth
    {
        public static void UseSwaggerUIWithAuth(this IApplicationBuilder app, SwaggerUIOptions options)
        {
            app.UseSwaggerUI(o =>
            {
                o.EnableTryItOutByDefault();

                //TODO: change all this value in PROD and don't expose that
                o.OAuthClientId(options.ClientId);
                o.OAuthClientSecret(options.ClientSecret);

                var descriptions = ((WebApplication)app).DescribeApiVersions();

                // Build a swagger endpoint for each discovered API version
                foreach (var description in descriptions)
                {
                    var url = $"/swagger/{description.GroupName}/swagger.json";
                    var name = description.GroupName.ToUpperInvariant();
                    o.SwaggerEndpoint(url, name);
                }
            });
        }
    }
}
