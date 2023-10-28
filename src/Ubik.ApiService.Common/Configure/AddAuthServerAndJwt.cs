using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Ubik.ApiService.Common.Configure.Options;

namespace Ubik.ApiService.Common.Configure
{
    public static class ServiceConfigurationAuth
    {
        public static void AddAuthServerAndJwt(this IServiceCollection services, AuthServerOptions options)
        {
            //Auth schema
            //TODO change https in PROD
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, o =>
            {
                o.MetadataAddress = options.MetadataAddress;
                o.Authority = options.Authority;
                o.Audience = options.Audience;
                o.RequireHttpsMetadata = options.RequireHttpsMetadata;
            });
        }
    }
}
