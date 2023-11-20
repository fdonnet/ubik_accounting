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
using System.Security.Claims;
using Ubik.Accounting.WebApp.ApiClients;
using Ubik.Accounting.WebApp.Components;
using Ubik.Accounting.WebApp.Security;
using Ubik.ApiService.Common.Configure.Options;
using static Ubik.Accounting.WebApp.Security.UserService;

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
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
    .AddCookie()
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

            options.CallbackPath = new PathString("/callback");

            options.Events = new OpenIdConnectEvents
            {
                OnRedirectToIdentityProvider = context => {
                    context.ProtocolMessage.SetParameter("audience", "https://my/api");
                    return Task.CompletedTask;
                }
            };
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



