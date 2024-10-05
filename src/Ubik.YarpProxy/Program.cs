using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Ubik.ApiService.Common.Configure.Options;
using Ubik.YarpProxy.Authorizations;
using Ubik.YarpProxy.Services;

var builder = WebApplication.CreateBuilder(args);

//Auth from auth provider
var authOptions = new AuthServerOptions();
builder.Configuration.GetSection(AuthServerOptions.Position).Bind(authOptions);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, o =>
            {
                o.MetadataAddress = authOptions.MetadataAddress;
                o.Authority = authOptions.Authority;
                o.Audience = authOptions.Audience;
                o.RequireHttpsMetadata = authOptions.RequireHttpsMetadata;
            });

//Cache
var redisOptions = new RedisOptions();
builder.Configuration.GetSection(RedisOptions.Position).Bind(redisOptions);
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = redisOptions.ConnectionString;
});

//Httpclient for userService
builder.Services.AddHttpClient<UserService>(client =>
{
    //TODO: change hardcoded
    client.BaseAddress = new Uri("https://localhost:7051/");
});

builder.Services.AddScoped<IAuthorizationHandler, UserInfoOkHandler>();
builder.Services.AddScoped<IAuthorizationHandler, UserIsMegaAdminHandler>();

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("UserInfoOk", policy =>
        policy.Requirements.Add(new UserInfoOkRequirement()))
    .AddPolicy("IsMegaAdmin", policy =>
        policy.Requirements.Add(new UserIsMegaAdminRequirement()));

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();
app.MapReverseProxy();
app.Run();
