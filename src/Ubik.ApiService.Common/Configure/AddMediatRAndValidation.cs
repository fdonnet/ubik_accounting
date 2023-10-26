using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Ubik.ApiService.Common.Exceptions;
using Ubik.ApiService.Common.Validators;

namespace Ubik.ApiService.Common.Configure
{
    public static class ServiceConfigurationMediatR
    {
        public static void AddMediatRAndValidation(this IServiceCollection services, Assembly currentAssembly)
        {
            //services.AddValidatorsFromAssembly(currentAssembly);
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(currentAssembly));
            //services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationPipelineBehavior<,>));
        }
    }
}
