using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ubik.ApiService.Common.Configure.Options;

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
