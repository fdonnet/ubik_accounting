using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Ubik.ApiService.Common.Configure
{
    public static class ServiceConfigurationMediatR
    {
        public static void AddMediatRAndValidation(this IServiceCollection services, Assembly currentAssembly)
        {
            //TODO: maybe we will reactivate mediatr validation pipeline...
            //services.AddValidatorsFromAssembly(currentAssembly);
            //services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(currentAssembly));
            //services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationPipelineBehavior<,>));
        }
    }
}
