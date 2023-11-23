using Microsoft.AspNetCore.Components.Server.Circuits;

namespace Ubik.Accounting.WebApp.Security
{
    public class CircuitServicesAccessor
    {
        static readonly AsyncLocal<IServiceProvider> blazorServices = new();

        public IServiceProvider? Services
        {
            get => blazorServices.Value;
            set => blazorServices.Value = value!;
        }
    }

    public class ServicesAccessorCircuitHandler(IServiceProvider services,
        CircuitServicesAccessor servicesAccessor) : CircuitHandler
    {
        readonly IServiceProvider services = services;
        readonly CircuitServicesAccessor circuitServicesAccessor = servicesAccessor;

        public override Func<CircuitInboundActivityContext, Task> CreateInboundActivityHandler(
            Func<CircuitInboundActivityContext, Task> next)
        {
            return async context =>
            {
                circuitServicesAccessor.Services = services;
                await next(context);
                circuitServicesAccessor.Services = null;
            };
        }

        public override async Task OnCircuitOpenedAsync(Circuit circuit, CancellationToken cancellationToken)
        {
            await base.OnCircuitOpenedAsync(circuit, cancellationToken);
            circuitServicesAccessor.Services = services;
        }

        public override async Task OnCircuitClosedAsync(Circuit circuit, CancellationToken cancellationToken)
        {
            await base.OnCircuitClosedAsync(circuit, cancellationToken);
            circuitServicesAccessor.Services = null;
        }
    }

    public static class CircuitServicesServiceCollectionExtensions
    {
        public static IServiceCollection AddCircuitServicesAccessor(
            this IServiceCollection services)
        {
            services.AddScoped<CircuitServicesAccessor>();
            services.AddScoped<CircuitHandler, ServicesAccessorCircuitHandler>();
            services.AddScoped<UserService>();

            return services;
        }

    }
}
