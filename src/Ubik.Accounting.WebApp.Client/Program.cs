using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using System.Net.Http;
using Ubik.Accounting.Webapp.Shared.Security;
using Ubik.Accounting.WebApp.Client;
using Ubik.Accounting.WebApp.Client.Security;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddSingleton<AuthenticationStateProvider, PersistentAuthenticationStateProvider>();

builder.Services
    .AddTransient<CookieHandler>()
    .AddHttpClient("WebApp", client => client.BaseAddress = new Uri("https://localhost:7249/")).AddHttpMessageHandler<CookieHandler>();


await builder.Build().RunAsync();
