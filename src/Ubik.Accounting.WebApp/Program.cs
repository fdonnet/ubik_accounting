using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Components.Server.Circuits;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Ubik.Accounting.WebApp.ApiClients;
using Ubik.Accounting.WebApp.Components;
using Ubik.Accounting.WebApp.Security;
using static Ubik.Accounting.WebApp.Security.UserService;
using IdentityModel.Client;
using Ubik.Accounting.Webapp.Shared.Security;
using Ubik.ApiService.Common.Configure.Options;
using Ubik.Accounting.WebApp.Render;
using Ubik.Accounting.Webapp.Shared.Render;
using Ubik.Accounting.Webapp.Shared.Facades;
using Microsoft.AspNetCore.Components.Authorization;
using Ubik.Accounting.WebApp.Client.Components.Accounts;
using Ubik.Accounting.Webapp.Shared.Features.Classifications.Services;
using Microsoft.AspNetCore.Authentication;
using Ubik.Accounting.WebApp.Config;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

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

//TODO: put that in a lib project Auth
//TODO: this is very dependant to distributed cache (if no cache => no site, see if it's bad)
//TODO: do better and use UserId in cache
var authOptions = new AuthServerOptions();
builder.Configuration.GetSection(AuthServerOptions.Position).Bind(authOptions);



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
        options.Cookie.SameSite = SameSiteMode.Strict;
        options.Events = new CookieAuthenticationEvents
        {
            OnValidatePrincipal = async x =>
            {
                var now = DateTimeOffset.UtcNow;
                var timeElapsedForCookie = now.Subtract(x.Properties.IssuedUtc!.Value);
                var timeRemainingForCookie = x.Properties.ExpiresUtc!.Value.Subtract(now);

                var userId = x.Principal!.FindFirst(ClaimTypes.Email)!.Value;

                //Try to get user in cache
                var cache = x.HttpContext.RequestServices.GetRequiredService<TokenCacheService>();
                var actualToken = await cache.GetUserTokenAsync(userId);

                //If not token
                if (actualToken == null)
                    x.RejectPrincipal();
                else
                {
                    //Refresh token
                    if (actualToken.ExpiresUtc < now)
                    {
                        var response = await new HttpClient().RequestRefreshTokenAsync(new RefreshTokenRequest
                        {
                            Address = authOptions.TokenUrl,
                            ClientId = authOptions.ClientId,
                            ClientSecret = authOptions.ClientSecret,
                            RefreshToken = actualToken.RefreshToken,
                            GrantType = "refresh_token",

                        });

                        if (!response.IsError)
                        {
                            await cache.SetUserTokenAsync(new TokenCacheEntry
                            {
                                UserId = userId,
                                RefreshToken = response.RefreshToken!,
                                AccessToken = response.AccessToken!,
                                ExpiresUtc = new JwtSecurityToken(response.AccessToken).ValidTo
                            });
                        }
                        else
                            x.RejectPrincipal();

                        //Refresh cookie
                        if (timeElapsedForCookie > timeRemainingForCookie)
                            x.ShouldRenew = true;
                    }
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

            //options.TokenValidationParameters = new()
            //{
            //    NameClaimType = "name",
            //};

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
                        ExpiresUtc = new JwtSecurityToken(x.TokenEndpointResponse.AccessToken).ValidTo
                    };
                    x.Properties!.IsPersistent = true;
                    x.Properties.ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(authOptions.CookieRefreshTimeInMinutes);

                    await cache.SetUserTokenAsync(token);
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
builder.Services.AddSingleton<IRenderContext, ServerRenderContext>();
builder.Services.AddScoped<AuthenticationStateProvider, PersistingRevalidatingAuthenticationStateProvider>();

//User service with circuit
builder.Services.AddScoped<UserService>();
builder.Services.TryAddEnumerable(
    ServiceDescriptor.Scoped<CircuitHandler, UserCircuitHandler>());

//Http client (the base one for the webassembly component and other typed for external apis
builder.Services
    .AddTransient<CookieHandler>()
    .AddHttpClient("WebApp", client => client.BaseAddress = new Uri("https://localhost:7249/")).AddHttpMessageHandler<CookieHandler>();

builder.Services.AddHttpClient<IAccountingApiClient, AccountingApiClient>();

builder.Services.Configure<ApiOptions>(
    builder.Configuration.GetSection(ApiOptions.Position));
var userServiceClientOpt = new ApiOptions();
builder.Configuration.GetSection(ApiOptions.Position).Bind(userServiceClientOpt);

builder.Services.AddHttpClient("UserServiceClient", options =>
{
    options.BaseAddress = new Uri(userServiceClientOpt.SecurityUrl);
});

builder.Services.AddScoped<ClassificationStateService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
else
{
    app.UseWebAssemblyDebugging();
}

app.UseHttpsRedirection();
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



