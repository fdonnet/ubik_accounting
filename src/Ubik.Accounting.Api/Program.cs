
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
using Ubik.ApiService.Common.Middlewares;
using Ubik.Accounting.Api.Features.Application.Services;
using Ubik.Accounting.Api.Features.AccountGroups.Services;
using Ubik.Accounting.Api.Features.Accounts.Services;
using Ubik.Accounting.Api.Features.Classifications.Services;
using Ubik.Accounting.Api.Features.Currencies.Services;

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

            //Auth server and JWT (no need, no auth)
            //builder.Services.AddAuthServerAndJwt(authOptions);

            //Default httpclient
            builder.Services.ConfigureHttpClientDefaults(http =>
            {
                http.AddStandardResilienceHandler();
            });

            //DB
            builder.Services.AddDbContextFactory<AccountingDbContext>(
                 options => options.UseNpgsql(builder.Configuration.GetConnectionString("AccountingContext")), ServiceLifetime.Scoped);

            Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;

            //TODO: remove useless parts (only pub/sub)
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

                    //TODO:review that
                    //Use to pass tenantid when message broker is used to contact the api (async)
                    //configurator.UseSendFilter(typeof(TenantIdSendFilter<>), context);
                    configurator.UsePublishFilter(typeof(TenantIdPublishFilter<>), context);
                    //configurator.UseConsumeFilter(typeof(TenantIdConsumeFilter<>), context);
                });

                config.AddEntityFrameworkOutbox<AccountingDbContext>(o =>
                {
                    o.UsePostgres();
                    o.UseBusOutbox();
                });

                //Add all consumers
                //config.AddConsumers(Assembly.GetExecutingAssembly());

                //Add commands clients

            });


            //Api versioning
            builder.Services.AddApiVersionAndExplorer();

            //TODO: Cors
            builder.Services.AddCustomCors();

            //Tracing and metrics
            builder.Logging.AddOpenTelemetry(logging =>
            {
                logging.IncludeFormattedMessage = true;
                logging.IncludeScopes = true;
            });

            builder.Services.AddTracingAndMetrics();

            //Swagger config
            var xmlPath = Path.Combine(AppContext.BaseDirectory,
                $"{Assembly.GetExecutingAssembly().GetName().Name}.xml");

            builder.Services.AddSwaggerGenWithAuth(authOptions, xmlPath);

            //Services injection
            //TODO: see if we need to integrate the user service more
            builder.Services.AddScoped<IApplicationCommandService, ApplicationCommandService>();
            builder.Services.AddScoped<IAccountGroupQueryService, AccountGroupQueryService>();
            builder.Services.AddScoped<IAccountGroupCommandService, AccountGroupCommandService>();
            builder.Services.AddScoped<IAccountQueryService, AccountQueryService>();
            builder.Services.AddScoped<IAccountCommandService, AccountCommandService>();
            builder.Services.AddScoped<IClassificationQueryService, ClassificationQueryService>();
            builder.Services.AddScoped<IClassificationCommandService, ClassificationCommandService>();
            builder.Services.AddScoped<ICurrencyQueryService, CurrencyQueryService>();
            builder.Services.AddScoped<ICurrentUser, CurrentUser>();
            builder.Services.AddTransient<ProblemDetailsFactory, CustomProblemDetailsFactory>();

            //Strandard API things
            builder.Services.AddControllers(o =>
            {
                o.Filters.Add(new ProducesAttribute("application/json"));
            }).AddJsonOptions(options =>
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

            builder.Services.AddHttpContextAccessor();
            builder.Services.AddEndpointsApiExplorer();

            //Route config
            builder.Services.Configure<RouteOptions>(options =>
            {
                options.LowercaseUrls = true;
            });

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
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                await DbInitializer.InitializeAsync(context);
            }

            app.UseWhen(
                httpContext => httpContext.Request.Path.StartsWithSegments("/admin"),
                subApp => subApp.UseMiddleware<MegaAdminUserInHeaderMiddleware>()
            );

            app.UseWhen(
                httpContext => !httpContext.Request.Path.StartsWithSegments("/admin")
                    && !httpContext.Request.Path.StartsWithSegments("/swagger"),

                subApp => subApp.UseMiddleware<UserInHeaderMiddleware>()
            );

            //app.UseHttpsRedirection();
            //app.UseAuthentication();
            //app.UseAuthorization();

            app.MapControllers();
            app.Run();
        }
    }
}
