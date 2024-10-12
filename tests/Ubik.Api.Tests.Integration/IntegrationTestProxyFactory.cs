using Microsoft.AspNetCore.Mvc.Testing;
using Ubik.YarpProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Testcontainers.Keycloak;
using Testcontainers.PostgreSql;
using Testcontainers.RabbitMq;
using DotNet.Testcontainers.Configurations;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using DotNet.Testcontainers.Builders;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using DotNet.Testcontainers.Containers;
using Microsoft.EntityFrameworkCore;
using static System.Net.Mime.MediaTypeNames;
using DotNet.Testcontainers.Images;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using static Ubik.Api.Tests.Integration.IntegrationTestAccoutingFactory;
using Ubik.Accounting.Api.Data;
using Ubik.ApiService.Common.Services;
using Docker.DotNet.Models;
using DotNet.Testcontainers.Networks;

namespace Ubik.Api.Tests.Integration
{
    public class IntegrationTestProxyFactory
        : WebApplicationFactory<Program>,
      IAsyncLifetime
    {
        private readonly PostgreSqlContainer _dbContainer;
        private readonly KeycloakContainer _keycloackContainer;
        private readonly RabbitMqContainer _rabbitMQContainer;
        private IContainer? _securityApiContainer;
        private INetwork _network;
        private readonly IFutureDockerImage _securityApiContainerImg;
        private static readonly string[] command = ["--import-realm"];

        public IntegrationTestProxyFactory()
        {
            _network = new NetworkBuilder()
                .WithName("network-test")
                .Build();

            _dbContainer = new PostgreSqlBuilder()
                .WithImage("postgres:latest")
                .WithName("postgres-db-test")
                .WithPassword("test01")
                .WithNetwork(_network)
                .WithNetworkAliases("db-test")
                .WithPortBinding(5432, true)
                .Build();

            _keycloackContainer = new KeycloakBuilder()
                                .WithImage("keycloak/keycloak:26.0")
                                .WithBindMount(GetWslAbsolutePath("./import"), "/opt/keycloak/data/import", AccessMode.ReadWrite)
                                .WithCommand(command)
                                .WithNetwork(_network)
                                .WithPortBinding(9000,true)
                                .WithNetworkAliases("keycloak")
                                .WithName("keycloak-test")
                                .Build();

            _rabbitMQContainer = new RabbitMqBuilder()
                                .WithImage("rabbitmq:4.0-management")
                                .WithUsername("guest")
                                .WithPassword("guest")
                                .WithPortBinding(5672, true)
                                .WithNetwork(_network)
                                .WithNetworkAliases("rabbit")
                                .WithName("rabbit-test")
                                .Build();

            _securityApiContainerImg = new ImageFromDockerfileBuilder()
                .WithName("security-api-test")
                .WithDockerfileDirectory(CommonDirectoryPath.GetGitDirectory(),string.Empty)
                .WithDockerfile("src/Ubik.Security.Api/Dockerfile")
                .Build();

        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            SetTestEnvVariables();

            //builder.ConfigureTestServices(services =>
            //{
            //    //var descriptor = services
            //    //    .SingleOrDefault(s => s.ServiceType == typeof(DbContextOptions<SecurityDbContext>));

            //    //if (descriptor is not null)
            //    //{
            //    //    services.Remove(descriptor);
            //    //}

            //    //services.AddDbContext<AccountingDbContext>(options =>
            //    //    options.UseNpgsql(_dbContainer.GetConnectionString()));

            //    //services.AddScoped<ICurrentUser, TestUserService>();
            //    //services.AddSingleton<ILoggerFactory, NullLoggerFactory>();
            //});
        }

        public async Task InitializeAsync()
        {
            await _securityApiContainerImg.CreateAsync();

            await _dbContainer.StartAsync();
            await _rabbitMQContainer.StartAsync();
            await _keycloackContainer.StartAsync();

            //await Task.WhenAll(keycloackTask, dbTask, rabbitTask);


            _securityApiContainer = new ContainerBuilder()
             .WithNetwork(_network)
             .WithNetworkAliases("api-security")
             .WithImage(_securityApiContainerImg)
             .WithEnvironment("AuthServer__MetadataAddress", $"http://keycloak:9000/realms/ubik/.well-known/openid-configuration")
             .WithEnvironment("AuthServer__Authority", $"http://keycloak:9000/realms/ubik")
             .WithEnvironment("AuthServer__AuthorizationUrl", $"http://keycloak:9000/realms/ubik/protocol/openid-connect/auth")
             .WithEnvironment("AuthServer__TokenUrl", $"http://keycloak:9000/realms/ubik/.protocol/openid-connect/token")
             .WithEnvironment("ConnectionStrings__SecurityDbContext", $"Host=db-test;Port=5432;Database=ubik_security;Username=postgres;Password=test01")
             .WithEnvironment("AuthManagerKeyCloakClient__RootUrl", $"http://keycloak:9000/")
             .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Development")
             .WithEnvironment("MessageBroker__Host", $"amqp://rabbit:5672")
             .WithPortBinding(7055,7051)
             .Build();


            await _securityApiContainer!.StartAsync();

        }

        public new async Task DisposeAsync()
        {
            await _keycloackContainer.DisposeAsync();
            await _dbContainer.DisposeAsync();
            await _rabbitMQContainer.DisposeAsync();
            await _securityApiContainer!.DisposeAsync();
        }

        private static string GetWslAbsolutePath(string windowRealtivePath)
        {
            var path = Path.GetFullPath(windowRealtivePath);
            return "/" + path.Replace('\\', '/').Replace(":", "");
        }

         private void SetTestEnvVariables()
        {
            var keycloakPort = _keycloackContainer.GetMappedPublicPort(9000);
            var keycloackHost = _keycloackContainer.Hostname;
            var rabbitMQPort = _rabbitMQContainer.GetMappedPublicPort(5672);
            var rabbitMQHost = _rabbitMQContainer.Hostname;

            Environment.SetEnvironmentVariable("AuthServer__MetadataAddress", $"http://{keycloackHost}:{keycloakPort}/realms/ubik/.well-known/openid-configuration");
            Environment.SetEnvironmentVariable("AuthServer__Authority", $"http://{keycloackHost}:{keycloakPort}/realms/ubik");
            Environment.SetEnvironmentVariable("AuthServer__AuthorizationUrl", $"http://{keycloackHost}:{keycloakPort}/realms/ubik/protocol/openid-connect/auth");
            Environment.SetEnvironmentVariable("AuthServer__TokenUrl", $"http://{keycloackHost}:{keycloakPort}/realms/ubik/protocol/openid-connect/token");
            Environment.SetEnvironmentVariable("ReverseProxy__Clusters__ubik_users_admin__Destinations__destination1__Address", $"https://{_securityApiContainer!.Hostname}:7055/");
        }
    }

    [CollectionDefinition("Proxy")]
    public class KeycloackAndDb : ICollectionFixture<IntegrationTestProxyFactory>
    {

    }
}

