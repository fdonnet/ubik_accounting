using MassTransit.Logging;
using MassTransit.Monitoring;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;


namespace Ubik.ApiService.Common.Configure
{
    public static class ServiceConfigurationTracingAndMetrics
    {
        public static void AddTracingAndMetrics(this IServiceCollection services)
        {
            static void ConfigureResource(ResourceBuilder r)
            {
                r.AddService("Service Name",
                    serviceVersion: "Version",
                    serviceInstanceId: Environment.MachineName);
            }

            var otel = services.AddOpenTelemetry();

            otel.ConfigureResource(ConfigureResource);

            otel.WithTracing(b => b
                    .AddSource(DiagnosticHeaders.DefaultListenerName)
                    .AddConsoleExporter());// MassTransit ActivitySource
                    

            otel.WithMetrics(b => b
                    .AddMeter(InstrumentationOptions.MeterName) // MassTransit Meter
                    .AddAspNetCoreInstrumentation()
                    .AddPrometheusExporter());
        }
    }
}
