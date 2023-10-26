using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ubik.ApiService.Common.Configure.Options;

namespace Ubik.ApiService.Common.Configure
{
    public static class ServiceConfigurationMessageBroker
    {
        public static void AddMasstransitMsgBroker<TDbContext>(this IServiceCollection services, MessageBrokerOptions options)
            where TDbContext : DbContext
        {
            services.AddMassTransit(config =>
            {
                config.SetKebabCaseEndpointNameFormatter();
                config.UsingRabbitMq((context, configurator) =>
                {
                    configurator.Host(new Uri(options.Host), h =>
                    {
                        h.Username(options.User);
                        h.Password(options.Password);
                    });

                    configurator.ConfigureEndpoints(context);
                });

                config.AddEntityFrameworkOutbox<TDbContext>(o =>
                {
                    o.UsePostgres();
                    o.UseBusOutbox();
                });
            });
        }
    }
}
