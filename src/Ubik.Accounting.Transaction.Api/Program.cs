using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Text.Json.Serialization;
using Ubik.Accounting.Transaction.Api.Data;
using Ubik.Accounting.Transaction.Api.Data.Init;
using Ubik.Accounting.Transaction.Api.Features.Accounts.Services;
using Ubik.Accounting.Transaction.Api.Features.TaxRates.Services;
using Ubik.Accounting.Transaction.Api.Features.Txs.Services;
using Ubik.ApiService.Common.Configure;
using Ubik.ApiService.Common.Configure.Options;
using Ubik.ApiService.Common.Exceptions;
using Ubik.ApiService.Common.Filters;
using Ubik.ApiService.Common.Middlewares;
using Ubik.ApiService.Common.Services;

var builder = WebApplication.CreateBuilder(args);

//Options
var authOptions = new AuthServerOptions();
builder.Configuration.GetSection(AuthServerOptions.Position).Bind(authOptions);
var msgBrokerOptions = new MessageBrokerOptions();
builder.Configuration.GetSection(MessageBrokerOptions.Position).Bind(msgBrokerOptions);
var swaggerUIOptions = new SwaggerUIOptions();
builder.Configuration.GetSection(SwaggerUIOptions.Position).Bind(swaggerUIOptions);

//Default httpclient
builder.Services.ConfigureHttpClientDefaults(http =>
{
    http.AddStandardResilienceHandler();
});

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

        configurator.ConfigureEndpoints(context);

        //TODO:review that Maybe not needed.... it was before I have the Yarp proxy...
        configurator.UsePublishFilter(typeof(TenantIdPublishFilter<>), context);
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

//Services
builder.Services.AddScoped<ICurrentUser, CurrentUser>();
builder.Services.AddScoped<IAccountCommandService, AccountCommandService>();
builder.Services.AddScoped<ITaxRateCommandService, TaxRateCommandService>();
builder.Services.AddScoped<ITxCommandService, TxCommandService>();
builder.Services.AddTransient<ProblemDetailsFactory, CustomProblemDetailsFactory>();

builder.Services.AddTracingAndMetrics();

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

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

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

//app.UseAuthorization();

app.MapControllers();

app.Run();
