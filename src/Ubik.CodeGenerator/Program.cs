// See https://aka.ms/new-console-template for more information
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using Ubik.ApiService.Common.Services;
using Ubik.CodeGenerator;
using Ubik.Security.Api.Data;


var serviceProvider = new ServiceCollection()
    .AddSingleton<ICurrentUserService, FakeUserService>()
    .AddDbContextFactory<SecurityDbContext>(
        options => options.UseNpgsql("x"))
    .AddSingleton<ContractsGenerator>()
    .AddSingleton<MappersGenerator>()
    .BuildServiceProvider();


var myContractsGenerator = serviceProvider.GetRequiredService<ContractsGenerator>();
var myMappersGenerator = serviceProvider.GetRequiredService<MappersGenerator>();

//GenerateForAddContract(true,@"F:/Dev/ubik/src/ubik_accounting/src/Ubik.Security.Contracts");
//GenerateForAddContract(true,@"C:/Dev/gitPriv/ubik_accounting/src/Ubik.Security.Contracts");
//myContractsGenerator.GenerateAllContracts(false, string.Empty, "RoleAuthorization");
myMappersGenerator.GenerateMappers("RoleAuthorization");




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




