
using Ubik.Accounting.Api.Data;
using Microsoft.EntityFrameworkCore;
using Ubik.ApiService.Common.Services;
using Ubik.ApiService.Common.Exceptions;
using Ubik.Accounting.Api.Features;
using System.Reflection;
using MediatR;
using Ubik.ApiService.Common.Validators;
using FluentValidation;
using Serilog;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using Ubik.Accounting.Api.Data.Init;
using Microsoft.AspNetCore.Mvc;
using MassTransit;
using Asp.Versioning;
using Microsoft.Extensions.Options;
using Ubik.Accounting.Api.Swagger;
using MassTransit.Configuration;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Ubik.Accounting.Api
{
    public class Program
    {
        public string KeycloackPort = "8080";
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            //Log
            //TODO: Begin to log usefull things
            builder.Host.UseSerilog((context, configuration) => configuration.ReadFrom.Configuration(context.Configuration));

            //Auth schema
            //TODO change https in PROD
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, o =>
            {
                o.MetadataAddress = builder.Configuration["Keycloack:MetadataAddress"]!;
                o.Authority = builder.Configuration["Keycloack:Authority"];
                o.Audience = builder.Configuration["Keycloack:Audience"];
                o.RequireHttpsMetadata = bool.Parse(builder.Configuration["Keycloack:RequireHttpsMetadata"]!);
            });

            //DB
            builder.Services.AddDbContextFactory<AccountingContext>(
                 options => options.UseNpgsql(builder.Configuration.GetConnectionString("AccountingContext")), ServiceLifetime.Scoped);

            //MediatR + remove standard model validations
            builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
            builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationPipelineBehavior<,>));

            builder.Services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });

            //MessageBroker with masstransit
            builder.Services.AddMassTransit(config =>
            {
                config.SetKebabCaseEndpointNameFormatter();
                config.UsingRabbitMq((context, configurator) =>
                {
                    configurator.Host(new Uri(builder.Configuration["MessageBroker:Host"]!), h =>
                    {
                        h.Username(builder.Configuration["MessageBroker:User"]!);
                        h.Password(builder.Configuration["MessageBroker:Password"]!);
                    });

                    configurator.ConfigureEndpoints(context);
                });

                config.AddEntityFrameworkOutbox<AccountingContext>(o =>
                {
                    o.UsePostgres();
                    o.UseBusOutbox();
                });
            });

            //Api versioning
            var apiVersionBuilder = builder.Services.AddApiVersioning(o =>
            {
                o.AssumeDefaultVersionWhenUnspecified = false;
                o.DefaultApiVersion = new ApiVersion(1, 0);
                o.ReportApiVersions = true;
                o.ApiVersionReader = ApiVersionReader.Combine(new UrlSegmentApiVersionReader(),
                                                    new HeaderApiVersionReader("x-api-version"),
                                                    new MediaTypeApiVersionReader("x-api-version"));
            });

            apiVersionBuilder.AddApiExplorer(o =>
            {
                o.GroupNameFormat = "'v'VVV";
                o.SubstituteApiVersionInUrl = true;
            });

            //Strandard API things
            builder.Services.AddControllers();
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddEndpointsApiExplorer();


            //TODO: change that before PROD
            //CORS
            builder.Services.AddCors(policies =>
            {
                policies.AddDefaultPolicy(builder =>
                {
                    builder.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin();
                });
            });

            //Swagger config
            builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
            builder.Services.AddSwaggerGen(c =>
            {
                c.AddSecurityDefinition(
                                "oauth2",
                                new OpenApiSecurityScheme
                                {
                                    Type = SecuritySchemeType.OAuth2,
                                    Flows = new OpenApiOAuthFlows
                                    {
                                        AuthorizationCode = new OpenApiOAuthFlow
                                        {
                                            AuthorizationUrl = new Uri(builder.Configuration["Keycloack:AuthorizationUrl"]!),
                                            TokenUrl = new Uri(builder.Configuration["Keycloack:TokenUrl"]!),
                                            Scopes = new Dictionary<string, string> { }
                                        }
                                    }
                                });


                c.AddSecurityRequirement(
                    new OpenApiSecurityRequirement
                    {
                    {
                        new OpenApiSecurityScheme{
                            Reference = new OpenApiReference{
                                Id = "oauth2", //The name of the previously defined security scheme.
                                Type = ReferenceType.SecurityScheme
                            }
                        },
                            new List<string>()
                    }
                    });

                c.OperationFilter<SwaggerDefaultValues>();

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });


            //Services injection
            //TODO: see if we need to integrate the user service more
            builder.Services.AddScoped<IServiceManager, ServiceManager>();
            builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();


            //Build the app
            var app = builder.Build();

            app.UseSerilogRequestLogging();
            app.UseExceptionHandler(app.Logger, app.Environment);

            if (app.Environment.IsDevelopment())
            {
                //TODO: Maybe we will expose swagger in PROD too
                app.UseSwagger();
                app.UseSwaggerUI(o =>
                {
                    o.EnableTryItOutByDefault();

                    var descriptions = app.DescribeApiVersions();

                    // Build a swagger endpoint for each discovered API version
                    foreach (var description in descriptions)
                    {
                        var url = $"/swagger/{description.GroupName}/swagger.json";
                        var name = description.GroupName.ToUpperInvariant();
                        o.SwaggerEndpoint(url, name);
                    }
                });

                //DB Init on DEV
                using var scope = app.Services.CreateScope();
                var services = scope.ServiceProvider;

                var context = services.GetRequiredService<AccountingContext>();
                //context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                var initDb = new DbInitializer();
                initDb.Initialize(context);
            }

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();
            app.Run();
        }
    }
}