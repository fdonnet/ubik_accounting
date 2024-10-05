using Ubik.ApiService.Common.Configure.Options;
using Ubik.ApiService.Common.Configure;
using Ubik.Security.Api.Data;
using Microsoft.EntityFrameworkCore;
using MassTransit;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Ubik.ApiService.Common.Exceptions;
using Ubik.ApiService.Common.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;
using Ubik.Security.Contracts.Users.Commands;
using Ubik.Security.Api.Features;
using Ubik.Security.Contracts.Authorizations.Commands;
using Ubik.Security.Api.Data.Init;
using Ubik.ApiService.Common.Configure.Options.Swagger;
using Ubik.Security.Api.Features.Users.Services;
using Ubik.Security.Api.Features.Authorizations.Services;

var builder = WebApplication.CreateBuilder(args);

//Options used in Program.cs
var msgBrokerOptions = new MessageBrokerOptions();
builder.Configuration.GetSection(MessageBrokerOptions.Position).Bind(msgBrokerOptions);
var swaggerUIOptions = new SwaggerUIOptions();
builder.Configuration.GetSection(SwaggerUIOptions.Position).Bind(swaggerUIOptions);
var authProviderOptions =  new AuthProviderKeycloakOptions();
builder.Configuration.GetSection(AuthProviderKeycloakOptions.Position).Bind(authProviderOptions);

//DB
builder.Services.AddDbContextFactory<SecurityDbContext>(
     options => options.UseNpgsql(builder.Configuration.GetConnectionString("SecurityDbContext")), ServiceLifetime.Scoped);

Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;

//Default httpclient
builder.Services.ConfigureHttpClientDefaults(http =>
{
    http.AddStandardResilienceHandler();
});

//Auth Provider
builder.Services.AddHttpClient<IUserAuthProviderService, UserAuthProviderServiceKeycloak>(client =>
{
    client.BaseAddress = new Uri(authProviderOptions.RootUrl);
});

//MessageBroker with masstransit + outbox
builder.Services.AddMassTransit(config =>
{
    config.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter(prefix: "SecurityApi", includeNamespace: false));
    config.UsingRabbitMq((context, configurator) =>
    {
        configurator.Host(new Uri(msgBrokerOptions.Host), h =>
        {
            h.Username(msgBrokerOptions.User);
            h.Password(msgBrokerOptions.Password);
        });

        configurator.ConfigureEndpoints(context);

        //Use to pass tenantid when message broker is used to contact the api (async)
        //TODO:See if needed
        //configurator.UseSendFilter(typeof(TenantIdSendFilter<>), context);
        //configurator.UsePublishFilter(typeof(TenantIdPublishFilter<>), context);
        //configurator.UseConsumeFilter(typeof(TenantIdConsumeFilter<>), context);
    });

    config.AddEntityFrameworkOutbox<SecurityDbContext>(o =>
    {
        o.UsePostgres();
        o.UseBusOutbox();
    });

    //Add all consumers
    config.AddConsumers(Assembly.GetExecutingAssembly());

    //Add commands clients
    config.AddRequestClient<AddUserCommand>();
    config.AddRequestClient<AddAuthorizationCommand>();


});

//Api versioning
builder.Services.AddApiVersionAndExplorer();

//TODO: Cors
builder.Services.AddCustomCors();

//Logs tracing and metrics
builder.Logging.AddOpenTelemetry(logging =>
{
    logging.IncludeFormattedMessage = true;
    logging.IncludeScopes = true;
});

builder.Services.AddTracingAndMetrics();

//Swagger config
var xmlPath = Path.Combine(AppContext.BaseDirectory,
    $"{Assembly.GetExecutingAssembly().GetName().Name}.xml");

builder.Services.AddSwaggerGen(c =>
{
    c.OperationFilter<SwaggerDefaultValues>();
    c.IncludeXmlComments(xmlPath);
});

//Services injection
//TODO: see if we need to integrate the user service more
builder.Services.AddScoped<IUsersCommandsService, UsersCommandsService>();
builder.Services.AddScoped<IUsersQueriesService, UsersQueriesService>();
builder.Services.AddScoped<IAuthorizationsCommandsService, AuthorizationsCommandsService>();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddTransient<ProblemDetailsFactory, CustomProblemDetailsFactory>();
builder.Services.Configure<AuthProviderKeycloakOptions>(
    builder.Configuration.GetSection(AuthProviderKeycloakOptions.Position));

//Strandard API things
builder.Services.AddControllers(o =>
{
    o.Filters.Add(new ProducesAttribute("application/json"));
}).AddJsonOptions(options =>
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddHttpContextAccessor();
builder.Services.AddEndpointsApiExplorer();

//Build the app
var app = builder.Build();

app.MapPrometheusScrapingEndpoint();
//app.UseSerilogRequestLogging();
app.UseExceptionHandler(app.Logger, app.Environment);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUIWithAuth(swaggerUIOptions);

    //DB Init on DEV
    using var scope = app.Services.CreateScope();
    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<SecurityDbContext>();
    //context.Database.EnsureDeleted();
    context.Database.EnsureCreated();

    var initDb = new DbInitializer();
    await initDb.InitializeAsync(context);
}

//app.UseHttpsRedirection();
//app.UseAuthentication();
//app.UseAuthorization();

app.MapControllers();
app.Run();
