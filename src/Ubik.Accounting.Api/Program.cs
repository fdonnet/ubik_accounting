
using Ubik.Accounting.Api.Data;
using Microsoft.EntityFrameworkCore;
using Ubik.ApiService.Common.Services;
using Ubik.ApiService.Common.Exceptions;
using Ubik.Accounting.Api.Features;
using System.Reflection;
using Ubik.Accounting.Api.Data.Init;
using Microsoft.AspNetCore.Mvc;
using MassTransit;
using Ubik.ApiService.Common.Configure;
using Ubik.ApiService.Common.Configure.Options;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Ubik.Accounting.Contracts.Accounts.Commands;
using Ubik.ApiService.Common.Filters;
using Ubik.Accounting.Contracts.AccountGroups.Commands;
using Ubik.Accounting.Contracts.Classifications.Commands;

namespace Ubik.Accounting.Api
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            //Log
            //TODO: Begin to log usefull things
            //builder.Host.UseSerilog((context, configuration) => configuration.ReadFrom.Configuration(context.Configuration));

            //Options
            var authOptions = new AuthServerOptions();
            builder.Configuration.GetSection(AuthServerOptions.Position).Bind(authOptions);
            var msgBrokerOptions = new MessageBrokerOptions();
            builder.Configuration.GetSection(MessageBrokerOptions.Position).Bind(msgBrokerOptions);
            var swaggerUIOptions = new SwaggerUIOptions();
            builder.Configuration.GetSection(SwaggerUIOptions.Position).Bind(swaggerUIOptions);

            //Auth server and JWT
            builder.Services.AddAuthServerAndJwt(authOptions);

            //DB
            builder.Services.AddDbContextFactory<AccountingDbContext>(
                 options => options.UseNpgsql(builder.Configuration.GetConnectionString("AccountingContext")), ServiceLifetime.Scoped);

            Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;

            //MessageBroker with masstransit + outbox
            builder.Services.AddMassTransit(config =>
            {
                config.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter(prefix: "AccountingApi", includeNamespace: false));
                config.UsingRabbitMq((context, configurator) =>
                {
                    configurator.Host(new Uri(msgBrokerOptions.Host), h =>
                    {
                        h.Username(msgBrokerOptions.User);
                        h.Password(msgBrokerOptions.Password);
                    });

                    configurator.ConfigureEndpoints(context);

                    //Use to pass tenantid when message broker is used to contact the api (async)
                    configurator.UseSendFilter(typeof(TenantIdSendFilter<>), context);
                    configurator.UsePublishFilter(typeof(TenantIdPublishFilter<>), context);
                    configurator.UseConsumeFilter(typeof(TenantIdConsumeFilter<>), context);
                });

                config.AddEntityFrameworkOutbox<AccountingDbContext>(o =>
                {
                    o.UsePostgres();
                    o.UseBusOutbox();
                });

                //Add all consumers
                config.AddConsumers(Assembly.GetExecutingAssembly());

                //Add commands clients
                config.AddRequestClient<AddAccountCommand>();
                config.AddRequestClient<AddAccountInAccountGroupCommand>();
                config.AddRequestClient<DeleteAccountInAccountGroupCommand>();
                config.AddRequestClient<DeleteAccountCommand>();
                config.AddRequestClient<UpdateAccountCommand>();
                config.AddRequestClient<AddAccountGroupCommand>();
                config.AddRequestClient<DeleteAccountGroupCommand>();
                config.AddRequestClient<UpdateAccountGroupCommand>();
                config.AddRequestClient<AddClassificationCommand>();
                config.AddRequestClient<UpdateClassificationCommand>();

            });


            //Api versioning
            builder.Services.AddApiVersionAndExplorer();

            //TODO: Cors
            builder.Services.AddCustomCors();

            //Tracing and metrics
            builder.Services.AddTracingAndMetrics();

            //Swagger config
            var xmlPath = Path.Combine(AppContext.BaseDirectory, 
                $"{Assembly.GetExecutingAssembly().GetName().Name}.xml");

            builder.Services.AddSwaggerGenWithAuth(authOptions,xmlPath);

            //Services injection
            //TODO: see if we need to integrate the user service more
            builder.Services.AddScoped<IServiceManager, ServiceManager>();
            builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
            builder.Services.AddTransient<ProblemDetailsFactory, CustomProblemDetailsFactory>();

            //Strandard API things
            builder.Services.AddControllers(o =>
            {
                o.Filters.Add(new ProducesAttribute("application/json"));
            }).AddJsonOptions(options =>
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter())); 

            builder.Services.AddHttpContextAccessor();
            builder.Services.AddEndpointsApiExplorer();

            //Build the app
            var app = builder.Build();

            app.MapPrometheusScrapingEndpoint();
            //app.UseSerilogRequestLogging();
            app.UseExceptionHandler(app.Logger, app.Environment);

            if (app.Environment.IsDevelopment())
            {
                //TODO: Maybe we will expose swagger in PROD too
                app.UseSwagger();
                app.UseSwaggerUIWithAuth(swaggerUIOptions);

                //DB Init on DEV
                using var scope = app.Services.CreateScope();
                var services = scope.ServiceProvider;

                var context = services.GetRequiredService<AccountingDbContext>();
                //context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                var initDb = new DbInitializer();
                await initDb.InitializeAsync(context);
            }

            //app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();
            app.Run();
        }
    }
}
