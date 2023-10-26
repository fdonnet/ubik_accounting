using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
