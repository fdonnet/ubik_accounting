// See https://aka.ms/new-console-template for more information
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Ubik.ApiService.Common.Services;
using Ubik.CodeGenerator;
using Ubik.Security.Api.Data;


var serviceProvider = new ServiceCollection()
    .AddSingleton<ICurrentUserService, FakeUserService>()
    .AddDbContextFactory<SecurityDbContext>(
        options => options.UseNpgsql("x"))
    .AddSingleton<ClassGeneratorV2>()
    .BuildServiceProvider();


var myApp = serviceProvider.GetRequiredService<ClassGeneratorV2>();

myApp.GenerateClassesContractAddCommand(true, @"F:/Dev/ubik/src/Ubik.Security.Contracts");


//FAKER to use the DBcontext
internal class FakeUserService : ICurrentUserService
{
    public ICurrentUser CurrentUser
    {
        get { return CurrentUserTmp; }
    }
    private static ICurrentUser CurrentUserTmp =>

        //TODO: remove and adapt => cannot keep this fake return
        new CurrentUser()
        {
            Email = "",
            Name = "",
            TenantIds = [NewId.NextGuid()],
            Id = NewId.NextGuid()
        };
}


