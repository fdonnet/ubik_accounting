
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

namespace Ubik.Accounting.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Host.UseSerilog((context, configuration) => configuration.ReadFrom.Configuration(context.Configuration));

            //TODO change https in PROD
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, o =>
            {
                o.MetadataAddress = "http://localhost:8080/realms/Ubik/.well-known/openid-configuration";
                o.Authority = "http://localhost:8080/realms/Ubik";
                o.Audience = "account";
                o.RequireHttpsMetadata = false;
            });


            // Add services to the container.
            var serverVersion = new MariaDbServerVersion(new Version(11, 1, 2));
            builder.Services.AddDbContextFactory<AccountingContext>(
                    options => options.UseMySql(builder.Configuration.GetConnectionString("AccountingContext"), serverVersion),ServiceLifetime.Scoped);

            builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
            builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationPipelineBehavior<,>));

            builder.Services.AddControllers();
            builder.Services.AddHttpContextAccessor();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddScoped<IServiceManager, ServiceManager>();
            //TODO: remove when migrated to service manager
            builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

            var app = builder.Build();

            app.UseSerilogRequestLogging();
            app.UseExceptionHandler(app.Logger, app.Environment);

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
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