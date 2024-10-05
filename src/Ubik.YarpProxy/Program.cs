using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Ubik.ApiService.Common.Configure.Options;
using Ubik.YarpProxy.Authorizations;
using Ubik.YarpProxy.Services;

var builder = WebApplication.CreateBuilder(args);

//Auth from auth provider, for Admin access to usermgt service
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

//Cache, for routes authorizations
var redisOptions = new RedisOptions();
builder.Configuration.GetSection(RedisOptions.Position).Bind(redisOptions);
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = redisOptions.ConnectionString;
});

//Httpclient for userService (called to retrive auth/authorize the request sent)
builder.Services.AddHttpClient<UserService>(client =>
{
    //TODO: change hardcoded
    client.BaseAddress = new Uri("https://localhost:7051/");
});

//Authorization handlers
builder.Services.AddScoped<IAuthorizationHandler, UserInfoOkHandler>();
builder.Services.AddScoped<IAuthorizationHandler, UserIsMegaAdminHandler>();

//Available policy
builder.Services.AddAuthorizationBuilder()
    .AddPolicy("UserInfoOk", policy =>
        policy.Requirements.Add(new UserInfoOkRequirement()))
    .AddPolicy("IsMegaAdmin", policy =>
        policy.Requirements.Add(new UserIsMegaAdminRequirement()));

//Proxy
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

//Build
var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();
app.MapReverseProxy();
app.Run();
