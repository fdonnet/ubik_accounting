
using Ubik.Accounting.Api.Data;
using Microsoft.EntityFrameworkCore;
using Ubik.ApiService.Common.Services;
using Ubik.ApiService.Common.Exceptions;
using Ubik.Accounting.Api.Features;
using System.Reflection;
using MediatR;
using Ubik.ApiService.Common.Validators;
using FluentValidation;

namespace Ubik.Accounting.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

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
                context.Database.EnsureCreated();

                var initDb = new DbInitializer();
                initDb.Initialize(context);
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();

            app.MapControllers();
            app.Run();
        }
    }
}