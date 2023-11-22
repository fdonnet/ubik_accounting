using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Server.Circuits;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.StaticFiles.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Ubik.Accounting.WebApp.ApiClients;
using Ubik.Accounting.WebApp.Components;
using Ubik.Accounting.WebApp.Security;
using Ubik.ApiService.Common.Configure.Options;
using System.Net.Http;
using static Ubik.Accounting.WebApp.Security.UserService;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components.Authorization;
using Ubik.Accounting.WebApp.Client.Pages;
using Ubik.Accounting.Webapp.Shared.Security;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services.AddControllers();

builder.Services.AddCascadingAuthenticationState();
//builder.Services.AddScoped<AuthenticationStateProvider, UserService>();


var authOptions = new AuthServerOptions();
builder.Configuration.GetSection(AuthServerOptions.Position).Bind(authOptions);

builder.Services.AddScoped<TokenCacheService>();
builder.Services.AddDistributedMemoryCache();

builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.CheckConsentNeeded = _ => false;
    options.MinimumSameSitePolicy = Microsoft.AspNetCore.Http.SameSiteMode.None;
});

builder.Services.AddAuthentication(options =>
{
    //options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
    {
        options.ExpireTimeSpan = TimeSpan.FromDays(1);
        options.Events = new CookieAuthenticationEvents
        {
            // this event is fired everytime the cookie has been validated by the cookie middleware,
            // so basically during every authenticated request
            // the decryption of the cookie has already happened so we have access to the user claims
            // and cookie properties - expiration, etc..
            OnValidatePrincipal = async x =>
            {
                // since our cookie lifetime is based on the access token one,
                // check if we're more than halfway of the cookie lifetime
                var now = DateTimeOffset.UtcNow;
                var timeElapsed = now.Subtract(x.Properties.IssuedUtc!.Value);
                var timeRemaining = x.Properties.ExpiresUtc!.Value.Subtract(now);

                if (timeElapsed > timeRemaining)
                {
                    var identity = (ClaimsIdentity)x.Principal!.Identity!;
                    //var accessTokenClaim = identity.FindFirst("access_token");
                    //var refreshTokenClaim = identity.FindFirst("refresh_token");

                    var cache = x.HttpContext.RequestServices.GetRequiredService<TokenCacheService>();
                    var actualToken = await cache.GetUserToken(identity.Name!);


                    if (actualToken == null)
                        return;
                    // if we have to refresh, grab the refresh token from the claims, and request
                    // new access token and refresh token
                    //var refreshToken = refreshTokenClaim.Value;

                    //Refresh
                    var response = await new HttpClient().RequestRefreshTokenAsync(new RefreshTokenRequest
                    {
                        Address = authOptions.TokenUrl,
                        ClientId = "ubik_accounting_clientapp",
                        ClientSecret = "iVw3c2Qs762cGMRcLTKJbeiaweeZhrge",
                        RefreshToken = actualToken.RefreshToken,
                        GrantType = "refresh_token",

                    });

                    if (!response.IsError)
                    {
                        // everything went right, remove old tokens and add new ones
                        //identity.RemoveClaim(accessTokenClaim);
                        //identity.RemoveClaim(refreshTokenClaim);

                        //identity.AddClaims(new[]
                        //{
                        //                new Claim("access_token", response.AccessToken),
                        //                new Claim("refresh_token", response.RefreshToken)
                        //            });

                        await cache.SetUserToken(new TokenCacheEntry
                        {
                            UserId = identity.Name!,
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
            options.ClientSecret = "iVw3c2Qs762cGMRcLTKJbeiaweeZhrge";
            options.ClientId = "ubik_accounting_clientapp";
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
                // that event is called after the OIDC middleware received the auhorisation code,
                // redeemed it for an access token and a refresh token,
                // and validated the identity token
                //OnTokenValidated = x =>
                //{
                //    // store both access and refresh token in the claims - hence in the cookie
                //    var identity = (ClaimsIdentity)x.Principal.Identity;
                //    identity.AddClaims(new[]
                //    {
                //                new Claim("access_token", x.TokenEndpointResponse.AccessToken),
                //                new Claim("refresh_token", x.TokenEndpointResponse.RefreshToken)
                //            });

                //    // so that we don't issue a session cookie but one with a fixed expiration
                //    x.Properties.IsPersistent = true;

                //    // align expiration of the cookie with expiration of the
                //    // access token
                //    var accessToken = new JwtSecurityToken(x.TokenEndpointResponse.AccessToken);
                //    x.Properties.ExpiresUtc = accessToken.ValidTo;
                //    //x.Properties.ExpiresUtc = DateTimeOffset.UtcNow.AddSeconds(3);

                //    return Task.CompletedTask;
                //}
                OnTokenValidated = async x =>
                {
                    var cache = x.HttpContext.RequestServices.GetRequiredService<TokenCacheService>();
                    var token = new TokenCacheEntry
                    {
                        UserId = x.Principal!.Identity!.Name!,
                        AccessToken = x.TokenEndpointResponse!.AccessToken,
                        RefreshToken = x.TokenEndpointResponse.RefreshToken,
                        ExpiresUtc = new JwtSecurityToken(x.TokenEndpointResponse.AccessToken).ValidTo
                    };
                    x.Properties!.IsPersistent = true;
                    x.Properties.ExpiresUtc = token.ExpiresUtc;

                    await cache.SetUserToken(token);
                }
            };

            //options.CallbackPath = new PathString("/callback");
        }
    });

builder.Services.AddHttpContextAccessor();
builder.Services
    .AddTransient<CookieHandler>()
    .AddHttpClient("WebApp", client => client.BaseAddress = new Uri("https://localhost:7249/")).AddHttpMessageHandler<CookieHandler>();

builder.Services.AddHttpClient<AccountingApiClient>();

builder.Services.AddAuthorization();

//builder.Services.AddScoped<UserService>();
//builder.Services.TryAddEnumerable(
//    ServiceDescriptor.Scoped<CircuitHandler, UserCircuitHandler>());


//CircuitServicesServiceCollectionExtensions.AddCircuitServicesAccessor(builder.Services);
//builder.Services.AddTransient<AuthenticationStateHandler>();
//builder.Services.AddHttpClient("ClientWithAuth", options =>
//{
//    options.BaseAddress = new Uri("https://localhost:7289/api/v1/");
//})
//    .AddHttpMessageHandler<AuthenticationStateHandler>();



//builder.Services.AddHttpClient<AccountingApiClient>(options =>
//{
//    options.BaseAddress = new Uri("https://localhost:7289/api/v1/");
//});
//builder.Services.AddScoped<AccountingApiClient>();


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
//app.UseMiddleware<UserServiceMiddleware>();

//app.MapGet("/Account/Login", async (HttpContext httpContext, string? returnUrl) =>
//{
//    await httpContext.ChallengeAsync(OpenIdConnectDefaults.AuthenticationScheme, new AuthenticationProperties
//    {
//        RedirectUri = returnUrl ?? "/"
//    });
//});

//app.MapPost("/Account/Logout", async (HttpContext httpContext, string returnUrl = "/") =>
//{
//    await httpContext.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme, new AuthenticationProperties { RedirectUri = "/" });
//    await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
//});

//app.MapGet("/Hello", (HttpContext httpContext) => Results.Ok("Hi!"))
//   .RequireAuthorization();

app.MapControllers();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(Counter).Assembly);

//app.MapRazorPages();

app.Run();



