using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Ubik.Accounting.Webapp.Shared.Facades;
using Ubik.Accounting.Webapp.Shared.Render;
using Ubik.Accounting.Webapp.Shared.Security;
using Ubik.Accounting.WebApp.Client.Facades;
using Ubik.Accounting.WebApp.Client.Render;
using Ubik.Accounting.WebApp.Client.Security;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddSingleton<AuthenticationStateProvider, PersistentAuthenticationStateProvider>();
builder.Services.AddSingleton<IRenderContext, ClientRenderContext>();

builder.Services
    .AddTransient<CookieHandler>()
    .AddHttpClient("WebApp", client => client.BaseAddress = new Uri("https://localhost:7249/")).AddHttpMessageHandler<CookieHandler>();

builder.Services.AddScoped<IAccountingApiClient, HttpApiAccountingFacade>();
builder.Services.AddFluentUIComponents();

await builder.Build().RunAsync();
