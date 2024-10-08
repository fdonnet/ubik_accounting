// See https://aka.ms/new-console-template for more information
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using Ubik.ApiService.Common.Services;
using Ubik.CodeGenerator;
using Ubik.Security.Api.Data;


var serviceProvider = new ServiceCollection()
    .AddSingleton<ICurrentUser, FakeUserService>()
    .AddDbContextFactory<SecurityDbContext>(
        options => options.UseNpgsql("x"))
    .AddSingleton<ContractsGenerator>()
    .AddSingleton<MappersGenerator>()
    .AddSingleton<ServicesGenerator>()
    .AddSingleton<ControllerGenerator>()
    .BuildServiceProvider();


var myContractsGenerator = serviceProvider.GetRequiredService<ContractsGenerator>();
var myMappersGenerator = serviceProvider.GetRequiredService<MappersGenerator>();
var myServicesGenerator = serviceProvider.GetRequiredService<ServicesGenerator>();
var myControllerGenerator = serviceProvider.GetRequiredService<ControllerGenerator>();

//myContractsGenerator.GenerateAllContracts(false, string.Empty, "RoleAuthorization");
//myMappersGenerator.GenerateMappers("RoleAuthorization");
//myServicesGenerator.GenerateAllServicesAndInterfaces("RoleAuthorization");
myControllerGenerator.GenerateController("RoleAuthorization");

//FAKER to use the DBcontext
internal class FakeUserService : ICurrentUser
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid? TenantId { get; set; } = Guid.NewGuid();
    public bool IsMegaAdmin { get; set; } = false;
}




