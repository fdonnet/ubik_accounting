using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;
using Ubik.ApiService.Common.Configure.Options;
using Ubik.ApiService.Common.Configure.Options.Swagger;
using Ubik.YarpProxy.Authorizations;
using Ubik.YarpProxy.Services;
using Yarp.ReverseProxy.Swagger;
using Yarp.ReverseProxy.Swagger.Extensions;
using Ubik.YarpProxy.Extensions;
using Ubik.YarpProxy.Config;
using Ubik.ApiService.Common.Configure;
using Microsoft.OpenApi.Models;
using static IdentityModel.ClaimComparer;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

//Auth from auth provider, for Admin access to usermgt service
var authOptions = new AuthServerOptions();
builder.Configuration.GetSection(AuthServerOptions.Position).Bind(authOptions);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, o =>
            {
                o.MetadataAddress = authOptions.MetadataAddress;
                o.Authority = authOptions.Authority;
                o.Audience = authOptions.Audience;
                o.RequireHttpsMetadata = authOptions.RequireHttpsMetadata;
            });

//Cache, for routes authorizations
var redisOptions = new RedisOptions();
builder.Configuration.GetSection(RedisOptions.Position).Bind(redisOptions);
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = redisOptions.ConnectionString;
});

//Swagger
builder.Services.AddApiVersionAndExplorer();
builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, YarpSwaggerConfigOptions>();

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
                                AuthorizationUrl = new Uri(authOptions.AuthorizationUrl),
                                TokenUrl = new Uri(authOptions.TokenUrl),
                                Scopes = new Dictionary<string, string> { }
                            }
                        },
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
});

//Httpclient for userService (called to retrive auth/authorize the request sent)
//Internal ip or domain not exposed to public accesses
builder.Services.ConfigureHttpClientDefaults(http =>
{
    http.AddStandardResilienceHandler();
});

builder.Services.AddHttpClient<UserService>(client =>
{
    //TODO: change hardcoded
    client.BaseAddress = new Uri("https://localhost:7051/");
});

//Authorization handlers
builder.Services.AddScoped<IAuthorizationHandler, UserInfoOkHandler>();

//Available policy
builder.Services.AddAuthorizationBuilder()
    .AddPolicy("IsUser", policy =>
        policy.Requirements.Add(new UserInfoOkRequirement(false)))
    .AddPolicy("IsMegaAdmin", policy =>
        policy.Requirements.Add(new UserInfoOkRequirement(true)));

//Proxy
var configProxy = builder.Configuration.GetSection("ReverseProxy");
builder.Services.AddReverseProxy()
    .LoadFromConfig(configProxy)
    .AddSwagger(configProxy);

//Build
var app = builder.Build();

//Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(o =>
    {
        var config = app.Services.GetRequiredService<IOptionsMonitor<ReverseProxyDocumentFilterConfig>>().CurrentValue;
        o.ConfigureSwaggerEndpoints(config);
        o.EnableTryItOutByDefault();

        //TODO: change all this value in PROD and don't expose that
        o.OAuthClientId(authOptions.ClientId);
        o.OAuthClientSecret(authOptions.ClientSecret);
    });
}

app.UseAuthentication();
app.UseAuthorization();
app.MapReverseProxy();
app.Run();
