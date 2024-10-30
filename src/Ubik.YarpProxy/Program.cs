using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;
using Ubik.ApiService.Common.Configure.Options;
using Ubik.YarpProxy.Authorizations;
using Ubik.YarpProxy.Services;
using Yarp.ReverseProxy.Swagger;
using Yarp.ReverseProxy.Swagger.Extensions;
using Ubik.YarpProxy.Extensions;
using Ubik.YarpProxy.Config;
using Ubik.ApiService.Common.Configure;
using Yarp.ReverseProxy.Transforms;
using System.Security.Claims;

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

                if (authOptions.AuthorizeBadCert)
                {
                    //TODO; remove that shit on prod... only for DEV keycloak Minikube
                    HttpClientHandler handler = new HttpClientHandler();
                    handler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
                    o.BackchannelHttpHandler = handler;
                }
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

builder.Services.AddSwaggerGen();

//Httpclient for userService (called to retrive auth/authorize the request sent)
//Internal ip or domain not exposed to public accesses
builder.Services.ConfigureHttpClientDefaults(http =>
{
    http.AddStandardResilienceHandler();
});

builder.Services.AddHttpClient<UserService>(client =>
{
    //TODO: change hardcoded
    client.BaseAddress = new Uri(builder.Configuration.GetSection("ApiSecurityForAdmin:HostAndPort").Value!);
});

//Authorization handlers
builder.Services.AddScoped<IAuthorizationHandler, UserInfoOkHandler>();
builder.Services.AddScoped<IAuthorizationHandler, UserWithAuthorizationsHandler>();

//Available policies
builder.Services.AddAuthorizationBuilder()
    .AddPolicy("IsUser", policy =>
        policy.Requirements.Add(new UserInfoOnlyRequirement(false)))
    .AddPolicy("IsMegaAdmin", policy =>
        policy.Requirements.Add(new UserInfoOnlyRequirement(true)))
    .AddPolicy("CanUsersRead", policy =>
        policy.Requirements.Add(new UserWithAuthorizationsRequirement(["security_user_read"])))
    .AddPolicy("CanUsersAndRolesRead", policy =>
        policy.Requirements.Add(new UserWithAuthorizationsRequirement(["security_user_read", "security_role_read"])))
    .AddPolicy("CanUsersAndRolesWrite", policy =>
        policy.Requirements.Add(new UserWithAuthorizationsRequirement(["security_user_write", "security_role_write"])))
    .AddPolicy("CanAccountGroupsRead", policy =>
        policy.Requirements.Add(new UserWithAuthorizationsRequirement(["accounting_accountgroup_read"])))
    .AddPolicy("CanAccountGroupsAndAccountsRead", policy =>
        policy.Requirements.Add(new UserWithAuthorizationsRequirement(["accounting_accountgroup_read", "accounting_account_read"])))
    .AddPolicy("CanAccountGroupsAndAccountsWrite", policy =>
        policy.Requirements.Add(new UserWithAuthorizationsRequirement(["accounting_accountgroup_write", "accounting_account_write"])))
    .AddPolicy("CanAccountGroupsAndAccountsAndClassificationsRead", policy =>
        policy.Requirements.Add(new UserWithAuthorizationsRequirement(["accounting_accountgroup_read", "accounting_account_read", "accounting_classification_read"])))
    .AddPolicy("CanAccountGroupsWrite", policy =>
        policy.Requirements.Add(new UserWithAuthorizationsRequirement(["accounting_accountgroup_write"])))
    .AddPolicy("CanAccountsRead", policy =>
        policy.Requirements.Add(new UserWithAuthorizationsRequirement(["accounting_account_read"])))
    .AddPolicy("CanAccountsWrite", policy =>
        policy.Requirements.Add(new UserWithAuthorizationsRequirement(["accounting_account_write"])))
    .AddPolicy("CanClassificationsWrite", policy =>
        policy.Requirements.Add(new UserWithAuthorizationsRequirement(["accounting_classification_write"])))
    .AddPolicy("CanClassificationsRead", policy =>
        policy.Requirements.Add(new UserWithAuthorizationsRequirement(["accounting_classification_read"])))
    .AddPolicy("CanClassificationsAndAccountsRead", policy =>
        policy.Requirements.Add(new UserWithAuthorizationsRequirement(["accounting_classification_read", "accounting_account_read"])))
    .AddPolicy("CanClassificationsAndAccountGroupsWrite", policy =>
        policy.Requirements.Add(new UserWithAuthorizationsRequirement(["accounting_classification_write", "accounting_accountgroup_write"])))
    .AddPolicy("CanCurrenciesRead", policy =>
        policy.Requirements.Add(new UserWithAuthorizationsRequirement(["accounting_currency_read"])));

//Proxy
var configProxy = builder.Configuration.GetSection("ReverseProxy");
builder.Services.AddReverseProxy()
    .LoadFromConfig(configProxy)
    .AddSwagger(configProxy)
    .AddTransforms(builderContext =>
    {
        //Transform the request to be sent with headers x-user-id and x-tenant-id
        var serviceProvider = builderContext.Services;

        builderContext.AddRequestTransform(async transformContext =>
        {
            var userService = serviceProvider.GetRequiredService<UserService>();
            var user = await userService.GetUserInfoAsync(transformContext.HttpContext.User.FindFirst(ClaimTypes.Email)?.Value);

            if (user != null)
            {
                transformContext.ProxyRequest.Headers.Add("x-user-id", user.Id.ToString());
                transformContext.ProxyRequest.Headers.Add("x-tenant-id", user.SelectedTenantId.ToString());

            }
        });
    });


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

public partial class Program { } //FOR TEST
