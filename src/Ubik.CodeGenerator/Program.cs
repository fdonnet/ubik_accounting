// See https://aka.ms/new-console-template for more information
using MassTransit;
using MassTransit.JobService;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;
using Ubik.ApiService.Common.Services;
using Ubik.CodeGenerator;
using Ubik.Security.Api.Data;
using static System.Net.Mime.MediaTypeNames;


var serviceProvider = new ServiceCollection()
    .AddSingleton<ICurrentUserService, FakeUserService>()
    .AddDbContextFactory<SecurityDbContext>(
        options => options.UseNpgsql("Host=localhost;Port=5435;Database=ubik_security;Username=postgres;Password=test01"))
    .AddSingleton<ClassGeneratorV2>()
    .BuildServiceProvider();


var myApp = serviceProvider.GetRequiredService<ClassGeneratorV2>();

myApp.GenerateClassesContractAddCommand();




//FAKER to use the DBcontext
internal class FakeUserService : ICurrentUserService
{
    public ICurrentUser CurrentUser
    {
        get { return GetCurrentUser(); }
    }
    private ICurrentUser GetCurrentUser()
    {

        //TODO: remove and adapt => cannot keep this fake return
        return new CurrentUser()
        {
            Email = "",
            Name = "",
            TenantIds = new Guid[] { Guid.Parse("727449e8-e93c-49e6-a5e5-1bf145d3e62d") },
            Id = NewId.NextGuid()
        };
    }
}

//builder.Services.AddDbContextFactory<SecurityDbContext>(
//     options => options.UseNpgsql(builder.Configuration.GetConnectionString("SecurityDbContext")), ServiceLifetime.Scoped);

//var dbContext = new SecurityDbContext();
//var generator = new ClassGeneratorV2();
//generator.GenerateClassesContractAddCommand();
