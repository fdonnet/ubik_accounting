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
using Ubik.Accounting.WebApp.Client.Components.Pages;
using Ubik.Accounting.Webapp.Shared.Security;
using Ubik.ApiService.Common.Configure.Options;
using Ubik.Accounting.WebApp.Render;
using Ubik.Accounting.Webapp.Shared.Render;
using Ubik.Accounting.Webapp.Shared.Facades;
using Microsoft.AspNetCore.Components.Authorization;
using Ubik.Accounting.WebApp.Client.Components.Accounts;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services.AddHttpContextAccessor();

builder.Services.AddControllers();
builder.Services.AddCascadingAuthenticationState();

//Cache
builder.Services.AddScoped<TokenCacheService>();
builder.Services.AddDistributedMemoryCache();

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
        options.ExpireTimeSpan = TimeSpan.FromDays(1);
        options.Events = new CookieAuthenticationEvents
        {
            OnValidatePrincipal = async x =>
            {
                // since our cookie lifetime is based on the access token one,
                // check if we're more than halfway of the cookie lifetime
                var now = DateTimeOffset.UtcNow;
                var timeElapsed = now.Subtract(x.Properties.IssuedUtc!.Value);
                var timeRemaining = x.Properties.ExpiresUtc!.Value.Subtract(now);

                if (timeElapsed > timeRemaining)
                {
                    var userId = x.Principal!.FindFirst(ClaimTypes.NameIdentifier)!.Value;
                    var cache = x.HttpContext.RequestServices.GetRequiredService<TokenCacheService>();
                    var actualToken = await cache.GetUserTokenAsync(userId);

                    if (actualToken == null)
                        return;

                    //Refresh from OpenId endpoint
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

                        // indicate to the cookie middleware to renew the session cookie
                        // the new lifetime will be the same as the old one, so the alignment
                        // between cookie and access token is preserved
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
            options.SaveTokens = true;
            options.GetClaimsFromUserInfoEndpoint = true;
            options.Scope.Clear();
            options.Scope.Add("openid");
            options.Scope.Add("offline_access");

            //TODO: change for prod
            options.RequireHttpsMetadata = false;

            options.TokenValidationParameters = new()
            {
                NameClaimType = "name",
            };

            options.Events = new OpenIdConnectEvents
            {
                //Store token in cache
                OnTokenValidated = async x =>
                {
                    var cache = x.HttpContext.RequestServices.GetRequiredService<TokenCacheService>();
                    var token = new TokenCacheEntry
                    {
                        UserId = x.Principal!.FindFirst(ClaimTypes.NameIdentifier)!.Value,
                        AccessToken = x.TokenEndpointResponse!.AccessToken,
                        RefreshToken = x.TokenEndpointResponse.RefreshToken,
                        ExpiresUtc = new JwtSecurityToken(x.TokenEndpointResponse.AccessToken).ValidTo
                    };
                    x.Properties!.IsPersistent = true;
                    x.Properties.ExpiresUtc = new JwtSecurityToken(x.TokenEndpointResponse.AccessToken).ValidTo;

                    await cache.SetUserTokenAsync(token);
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

//builder.Services.AddScoped<IClientContactFacade, ClientContactFacade>();

//Http client (the base one for the webassembly component and other typed for external apis
builder.Services
    .AddTransient<CookieHandler>()
    .AddHttpClient("WebApp", client => client.BaseAddress = new Uri("https://localhost:7249/")).AddHttpMessageHandler<CookieHandler>();

builder.Services.AddHttpClient<IAccountingApiClient, AccountingApiClient>();

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
    .AddAdditionalAssemblies(typeof(ManageAccounts).Assembly);
//.AddAdditionalAssemblies(typeof(AccoutsList).Assembly)
//.AddAdditionalAssemblies(typeof(Auth).Assembly);

app.Run();



