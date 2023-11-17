using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.StaticFiles.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Ubik.Accounting.WebApp.Components;
using Ubik.Accounting.WebApp.Security;
using Ubik.ApiService.Common.Configure.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddRazorPages();

var authOptions = new AuthServerOptions();
builder.Configuration.GetSection(AuthServerOptions.Position).Bind(authOptions);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
    .AddCookie()
    .AddOpenIdConnect("Keycloack", options =>
    {
        {
            options.Authority = authOptions.Authority;
            options.MetadataAddress = authOptions.MetadataAddress;
            options.ClientSecret = "ZHwmuIwjkdwTPeTZqfAst1YxiY27FgZq";
            options.ClientId = "ubik_accounting_clientapp";
            options.ResponseType = "code";
            options.SaveTokens = true;
            //options.GetClaimsFromUserInfoEndpoint = true;
            options.Scope.Clear();
            options.Scope.Add("openid");
            options.Scope.Add("profile");
            options.Scope.Add("email");
            options.Scope.Add("role");
            //TODO: change for prod
            options.RequireHttpsMetadata = false;

            options.TokenValidationParameters = new()
            {
                NameClaimType = "name",
            };

            options.CallbackPath = new PathString("/callback");

            options.Events = new OpenIdConnectEvents
            {
                OnRedirectToIdentityProviderForSignOut = (context) =>
                {
                    var logoutUri = $"http://localhost:8080/realms/ubik/protocol/openid-connect/logout";

                    var postLogoutUri = context.Properties.RedirectUri;
                    if (!string.IsNullOrEmpty(postLogoutUri))
                    {
                        if (postLogoutUri.StartsWith("/"))
                        {
                            var request = context.Request;
                            postLogoutUri = request.Scheme + "://" + request.Host + request.PathBase + postLogoutUri;
                        }
                        logoutUri += $"&returnTo={Uri.EscapeDataString(postLogoutUri)}";
                    }

                    context.Response.Redirect(logoutUri);
                    context.HandleResponse();

                    return Task.CompletedTask;
                }
            };
        }
    });

builder.Services.AddScoped<TokenProvider>();
builder.Services.AddHttpClient();
builder.Services.AddCascadingAuthenticationState();


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
app.MapRazorPages();
app.UseAntiforgery();
app.UseAuthentication();
app.UseAuthorization();


app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();



app.Run();
