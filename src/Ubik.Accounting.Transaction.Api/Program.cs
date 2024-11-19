using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Text.Json.Serialization;
using Ubik.Accounting.Transaction.Api.Data;
using Ubik.Accounting.Transaction.Api.Data.Init;
using Ubik.Accounting.Transaction.Api.Features.Accounts.Services;
using Ubik.Accounting.Transaction.Api.Features.Application.Services;
using Ubik.Accounting.Transaction.Api.Features.TaxRates.Services;
using Ubik.Accounting.Transaction.Api.Features.Txs.Services;
using Ubik.ApiService.Common.Configure;
using Ubik.ApiService.Common.Configure.Options;
using Ubik.ApiService.Common.Exceptions;
using Ubik.ApiService.Common.Filters;
using Ubik.ApiService.Common.Middlewares;
using Ubik.ApiService.Common.Services;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddServiceApiDefaults();

//Options
var authOptions = new AuthServerOptions();
builder.Configuration.GetSection(AuthServerOptions.Position).Bind(authOptions);
var msgBrokerOptions = new MessageBrokerOptions();
builder.Configuration.GetSection(MessageBrokerOptions.Position).Bind(msgBrokerOptions);
var swaggerUIOptions = new SwaggerUIOptions();
builder.Configuration.GetSection(SwaggerUIOptions.Position).Bind(swaggerUIOptions);

builder.Services.AddDbContextFactory<AccountingTxContext>(
     options => options.UseNpgsql(builder.Configuration.GetConnectionString("AccountingTxContext")), ServiceLifetime.Scoped);

Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;

//MessageBroker with masstransit + outbox
builder.Services.AddMassTransit(config =>
{
    config.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter(prefix: "AccountingTxApi", includeNamespace: false));
    config.UsingRabbitMq((context, configurator) =>
    {
        configurator.Host(new Uri(msgBrokerOptions.Host), h =>
        {
            h.Username(msgBrokerOptions.User);
            h.Password(msgBrokerOptions.Password);
        });

        configurator.ConfigureJsonSerializerOptions(o =>
        {
            o.Converters.Add(new JsonStringEnumConverter());
            return o;
        });

        configurator.ConfigureEndpoints(context);

        //TODO:review that Maybe not needed.... it was before I have the Yarp proxy...
        configurator.UsePublishFilter(typeof(TenantAndUserIdsPublishFilter<>), context);
        configurator.UseConsumeFilter(typeof(TenantAndUserIdsConsumeFilter<>), context);
    });

    config.AddEntityFrameworkOutbox<AccountingTxContext>(o =>
    {
        o.UsePostgres();
        o.UseBusOutbox();
    });

    //Add all consumers
    config.AddConsumers(Assembly.GetExecutingAssembly());

    //Add commands clients

});

//Services
builder.Services.AddScoped<ICurrentUser, CurrentUser>();
builder.Services.AddScoped<IAccountCommandService, AccountCommandService>();
builder.Services.AddScoped<ITaxRateCommandService, TaxRateCommandService>();
builder.Services.AddScoped<ITxCommandService, TxCommandService>();
builder.Services.AddScoped<IApplicationCommandService, ApplicationCommandService>();
builder.Services.AddTransient<ProblemDetailsFactory, CustomProblemDetailsFactory>();

//Swagger config
var xmlPath = Path.Combine(AppContext.BaseDirectory,
    $"{Assembly.GetExecutingAssembly().GetName().Name}.xml");

builder.Services.AddSwaggerGenWithAuth(authOptions, xmlPath);

var app = builder.Build();

app.MapDefaultEndpoints();

app.UseExceptionHandler(app.Logger, app.Environment);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUIWithAuth(swaggerUIOptions);

    //DB Init on DEV
    using var scope = app.Services.CreateScope();
    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<AccountingTxContext>();
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

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
