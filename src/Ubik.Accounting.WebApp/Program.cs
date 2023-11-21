﻿using Microsoft.AspNetCore.Authentication.Cookies;
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

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddRazorPages();

var authOptions = new AuthServerOptions();
builder.Configuration.GetSection(AuthServerOptions.Position).Bind(authOptions);

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
        options.ExpireTimeSpan = TimeSpan.FromSeconds(10);
        //options.Events = new CookieAuthenticationEvents
        //{
        //    // this event is fired everytime the cookie has been validated by the cookie middleware,
        //    // so basically during every authenticated request
        //    // the decryption of the cookie has already happened so we have access to the user claims
        //    // and cookie properties - expiration, etc..
        //    OnValidatePrincipal = async x =>
        //    {
        //        // since our cookie lifetime is based on the access token one,
        //        // check if we're more than halfway of the cookie lifetime
        //        var now = DateTimeOffset.UtcNow;
        //        var timeElapsed = now.Subtract(x.Properties.IssuedUtc.Value);
        //        var timeRemaining = x.Properties.ExpiresUtc.Value.Subtract(now);

        //        if (timeElapsed > timeRemaining)
        //        {
        //            var identity = (ClaimsIdentity)x.Principal.Identity;
        //            var accessTokenClaim = identity.FindFirst("access_token");
        //            var refreshTokenClaim = identity.FindFirst("refresh_token");

        //            // if we have to refresh, grab the refresh token from the claims, and request
        //            // new access token and refresh token
        //            var refreshToken = refreshTokenClaim.Value;
        //            var response = await new HttpClient().RequestRefreshTokenAsync(new RefreshTokenRequest
        //            {
        //                Address = authOptions.Authority,
        //                ClientId = "ubik_accounting_clientapp",
        //                ClientSecret = "ZHwmuIwjkdwTPeTZqfAst1YxiY27FgZq",
        //                RefreshToken = refreshToken
        //            });

        //            if (!response.IsError)
        //            {
        //                // everything went right, remove old tokens and add new ones
        //                identity.RemoveClaim(accessTokenClaim);
        //                identity.RemoveClaim(refreshTokenClaim);

        //                identity.AddClaims(new[]
        //                {
        //                                new Claim("access_token", response.AccessToken),
        //                                new Claim("refresh_token", response.RefreshToken)
        //                            });

        //                // indicate to the cookie middleware to renew the session cookie
        //                // the new lifetime will be the same as the old one, so the alignment
        //                // between cookie and access token is preserved
        //                x.ShouldRenew = true;
        //            }
        //        }
        //    }
        //};
    })
    .AddOpenIdConnect(options =>
    {
        {
            options.Authority = authOptions.Authority;
            options.MetadataAddress = authOptions.MetadataAddress;
            options.ClientSecret = "ZHwmuIwjkdwTPeTZqfAst1YxiY27FgZq";
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

            //options.Events = new OpenIdConnectEvents
            //{
            //    // that event is called after the OIDC middleware received the auhorisation code,
            //    // redeemed it for an access token and a refresh token,
            //    // and validated the identity token
            //    OnTokenValidated = x =>
            //    {
            //        // store both access and refresh token in the claims - hence in the cookie
            //        var identity = (ClaimsIdentity)x.Principal.Identity;
            //        identity.AddClaims(new[]
            //        {
            //                    new Claim("access_token", x.TokenEndpointResponse.AccessToken),
            //                    new Claim("refresh_token", x.TokenEndpointResponse.RefreshToken)
            //                });

            //        // so that we don't issue a session cookie but one with a fixed expiration
            //        x.Properties.IsPersistent = true;

            //        // align expiration of the cookie with expiration of the
            //        // access token
            //        var accessToken = new JwtSecurityToken(x.TokenEndpointResponse.AccessToken);
            //        x.Properties.ExpiresUtc = accessToken.ValidTo;

            //        return Task.CompletedTask;
            //    }
            //};

            //options.CallbackPath = new PathString("/callback");
        }
    });

builder.Services.AddAuthorization();

builder.Services.AddScoped<UserService>();
//builder.Services.TryAddEnumerable(
//    ServiceDescriptor.Scoped<CircuitHandler, UserCircuitHandler>());


//CircuitServicesServiceCollectionExtensions.AddCircuitServicesAccessor(builder.Services);
//builder.Services.AddTransient<AuthenticationStateHandler>();
//builder.Services.AddHttpClient("ClientWithAuth", options =>
//{
//    options.BaseAddress = new Uri("https://localhost:7289/api/v1/");
//})
//    .AddHttpMessageHandler<AuthenticationStateHandler>();



builder.Services.AddCascadingAuthenticationState();
builder.Services.AddHttpClient<AccountingApiClient>(options =>
{
    options.BaseAddress = new Uri("https://localhost:7289/api/v1/");
});
//builder.Services.AddScoped<AccountingApiClient>();


var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<UserServiceMiddleware>();


app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapRazorPages();

app.Run();



