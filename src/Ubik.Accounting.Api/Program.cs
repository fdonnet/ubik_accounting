
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

namespace Ubik.Accounting.Api
{
    public class Program
    {
        public string KeycloackPort = "8080";
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Host.UseSerilog((context, configuration) => configuration.ReadFrom.Configuration(context.Configuration));

            //TODO change https in PROD
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, o =>
            {
                o.MetadataAddress = builder.Configuration["Keycloack:MetadataAddress"]!;
                o.Authority = builder.Configuration["Keycloack:Authority"];
                o.Audience = builder.Configuration["Keycloack:Audience"];
                o.RequireHttpsMetadata = bool.Parse(builder.Configuration["Keycloack:RequireHttpsMetadata"]!);
            });

            // Add services to the container.
            var serverVersion = new MariaDbServerVersion(new Version(11, 1, 2));
            builder.Services.AddDbContextFactory<AccountingContext>(
                    options => options.UseMySql(builder.Configuration.GetConnectionString("AccountingContext"), serverVersion), ServiceLifetime.Scoped);

            builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
            builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationPipelineBehavior<,>));

            builder.Services.AddControllers();
            builder.Services.AddHttpContextAccessor();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();


            //TODO: change that before PROD
            builder.Services.AddCors(policies =>
            {
                policies.AddDefaultPolicy(builder =>
                {
                    builder.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin();
                });
            });

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


            });

            builder.Services.AddScoped<IServiceManager, ServiceManager>();
            //TODO: remove when migrated to service manager
            builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

            builder.Services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });

            var app = builder.Build();

            app.UseSerilogRequestLogging();
            app.UseExceptionHandler(app.Logger, app.Environment);

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(o =>
                {
                    o.EnableTryItOutByDefault();
                });
            }

            //DB in DEV
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                var context = services.GetRequiredService<AccountingContext>();
                context.Database.EnsureDeleted();
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