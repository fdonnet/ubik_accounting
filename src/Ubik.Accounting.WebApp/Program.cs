using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Ubik.Accounting.WebApp.ApiClients;
using Ubik.Accounting.WebApp.Components;
using Ubik.Accounting.WebApp.Security;
using Ubik.ApiService.Common.Configure.Options;
using Ubik.Accounting.WebApp.Render;
using Ubik.Accounting.Webapp.Shared.Render;
using Ubik.Accounting.Webapp.Shared.Facades;
using Microsoft.AspNetCore.Components.Authorization;
using Ubik.Accounting.WebApp.Client.Components.Accounts;
using Ubik.Accounting.Webapp.Shared.Features.Classifications.Services;
using Microsoft.AspNetCore.Authentication;
using Ubik.Accounting.WebApp.Config;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.AspNetCore.Components.Server.Circuits;
using static Ubik.Accounting.WebApp.Security.UserService;
using Ubik.Accounting.Webapp.Shared.Features.Global.Services;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services.AddHttpContextAccessor();

builder.Services.AddControllers();
builder.Services.AddCascadingAuthenticationState();

//Cache
var redisOptions = new RedisOptions();
builder.Configuration.GetSection(RedisOptions.Position).Bind(redisOptions);
builder.Services.AddScoped<TokenCacheService>();
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = redisOptions.ConnectionString;
});
#pragma warning disable EXTEXP0018 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
builder.Services.AddHybridCache();
#pragma warning restore EXTEXP0018 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

//TODO: put that in a lib project Auth
//TODO: this is very dependant to distributed cache (if no cache => no site, see if it's bad)
var authOptions = new AuthServerOptions();
builder.Configuration.GetSection(AuthServerOptions.Position).Bind(authOptions);
builder.Services.Configure<AuthServerOptions>(
    builder.Configuration.GetSection(AuthServerOptions.Position));


builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
    {
        options.ExpireTimeSpan = TimeSpan.FromMinutes(authOptions.CookieRefreshTimeInMinutes);
        options.SlidingExpiration = true;

        options.Events = new CookieAuthenticationEvents
        {
            OnValidatePrincipal = async x =>
            {
                var now = DateTimeOffset.UtcNow;
                var userId = x.Principal!.FindFirst(ClaimTypes.Email)!.Value;

                //Try to get user in cache
                var cache = x.HttpContext.RequestServices.GetRequiredService<TokenCacheService>();
                var actualToken = await cache.GetUserTokenAsync(userId);

                //If no token
                if (actualToken == null)
                {
                    x.RejectPrincipal();
                    return;
                }
            }
        };
    })
    .AddOpenIdConnect(options =>
    {
        {
            options.Authority = authOptions.Authority;
            options.MetadataAddress = authOptions.MetadataAddress;
            options.ClientSecret = authOptions.ClientSecret;
            options.ClientId = authOptions.ClientId;
            options.ResponseType = "code";
            options.SaveTokens = false;
            options.GetClaimsFromUserInfoEndpoint = true;
            options.Scope.Clear();
            options.Scope.Add("openid");
            options.Scope.Add("offline_access");
            options.RequireHttpsMetadata = authOptions.RequireHttpsMetadata;

            if (authOptions.AuthorizeBadCert)
            {
                //TODO; remove that shit on prod... only for DEV keycloak Minikube
                HttpClientHandler handler = new()
                {
                    ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                };
                options.BackchannelHttpHandler = handler;
            }

            options.Events = new OpenIdConnectEvents
            {
                //Store token in cache
                OnTokenValidated = async x =>
                {
                    var cache = x.HttpContext.RequestServices.GetRequiredService<TokenCacheService>();
                    var token = new TokenCacheEntry
                    {
                        UserId = x.Principal!.FindFirst(ClaimTypes.Email)!.Value,
                        AccessToken = x.TokenEndpointResponse!.AccessToken,
                        RefreshToken = x.TokenEndpointResponse.RefreshToken,
                        ExpiresUtc = new JwtSecurityToken(x.TokenEndpointResponse.AccessToken).ValidTo,
                        ExpiresRefreshUtc = DateTimeOffset.UtcNow.AddMinutes(authOptions.RefreshTokenExpTimeInMinutes)
                    };
                    x.Properties!.IsPersistent = true;
                    x.Properties.ExpiresUtc = new JwtSecurityToken(x.TokenEndpointResponse.AccessToken).ValidTo;

                    await cache.SetUserTokenAsync(token);
                    x.Success();
                },
                //Only store the Id token for more security
                OnTokenResponseReceived = async x =>
                {
                    ////Only store id_token in cookie
                    x.Properties!.StoreTokens([ new AuthenticationToken
                        {
                            Name = "id_token",
                            Value = x.TokenEndpointResponse.IdToken
                        }]);

                    await Task.CompletedTask;
                },
            };
        }
    });

builder.Services.AddAuthorization();

//Services
builder.Services.AddSingleton<IRenderContext, ServerRenderContext>();
builder.Services.AddScoped<BreakpointsService>();
builder.Services.AddScoped<AuthenticationStateProvider, PersistingRevalidatingAuthenticationStateProvider>();
builder.Services.AddScoped<ClassificationStateService>();

//User service with circuit
builder.Services.AddScoped<UserService>();
builder.Services.TryAddEnumerable(
    ServiceDescriptor.Scoped<CircuitHandler, UserCircuitHandler>());

//Typed clients
builder.Services.AddHttpClient<IAccountingApiClient, AccountingApiClient>();

builder.Services.Configure<ApiOptions>(
    builder.Configuration.GetSection(ApiOptions.Position));
var userServiceClientOpt = new ApiOptions();
builder.Configuration.GetSection(ApiOptions.Position).Bind(userServiceClientOpt);

builder.Services.AddHttpClient("UserServiceClient", options =>
{
    options.BaseAddress = new Uri(userServiceClientOpt.SecurityUrl);
});

builder.Services.AddHttpClient("TokenClient", options =>
{
    options.BaseAddress = new Uri(authOptions.TokenUrl);
}).ConfigurePrimaryHttpMessageHandler(() => {

    //TODO; remove that shit on prod... only for DEV keycloak Minikube
    var httpClientHandler = new HttpClientHandler();

    if(authOptions.AuthorizeBadCert)
    {
        httpClientHandler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
    }
    return httpClientHandler;
});

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders =
        ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    options.KnownNetworks.Clear(); // Clear the default networks
    options.KnownProxies.Clear(); // Clear the default proxies
});

var app = builder.Build();

app.MapDefaultEndpoints();
app.UseForwardedHeaders();
app.UseHttpsRedirection();
//app.UseHsts();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    //app.UseHsts();
}
else
{
    app.UseWebAssemblyDebugging();
}

//app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<UserServiceMiddleware>();

app.MapControllers();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(Accounts).Assembly);

app.Run();



