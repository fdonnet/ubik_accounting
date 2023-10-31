using Microsoft.Extensions.DependencyInjection;

namespace Ubik.ApiService.Common.Configure
{
    public static class ServiceConfigurationCors
    {
        public static void AddCustomCors(this IServiceCollection services)
        {
            //TODO: change that before PROD
            services.AddCors(policies =>
            {
                policies.AddDefaultPolicy(builder =>
                {
                    builder.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin();
                });
            });
        }
    }
}
