using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

using IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        var configuration = new ConfigurationBuilder()
        .AddJsonFile($"appsettings.json");

        var settings = configuration.Build();
        
        services.AddMassTransit(config =>
        {
            config.AddConsumer<AccountDeletedConsumer>();
            config.AddConsumer<AccountAddedConsumer>();
            config.AddConsumer<AccountUpdatedConsumer>();
            config.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter(prefix: "Console", includeNamespace: false));
            config.UsingRabbitMq((context, configurator) =>
            {

                configurator.Host(new Uri(settings["MessageBroker:Host"]!), h =>
                {
                    h.Username(settings["MessageBroker:User"]!);
                    h.Password(settings["MessageBroker:Password"]!);
                });

                configurator.ConfigureEndpoints(context);
            });
        });
    })
    .Build();

await host.RunAsync();




