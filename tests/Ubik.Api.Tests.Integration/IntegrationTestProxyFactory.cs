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

namespace Ubik.Api.Tests.Integration
{
    internal class IntegrationTestProxyFactory
        : WebApplicationFactory<Program>,
      IAsyncLifetime
    {
        private readonly PostgreSqlContainer _dbContainer;
        private readonly KeycloakContainer _keycloackContainer;
        private readonly RabbitMqContainer _rabbitMQContainer;
        private readonly IContainer _securityApiContainer;
        private static readonly string[] command = ["--import-realm"];

        public IntegrationTestProxyFactory()
        {
            _dbContainer = new PostgreSqlBuilder()
                .WithImage("postgres:latest")
                .WithPassword("TEST_PASSWORD")
                .Build();

            _keycloackContainer = new KeycloakBuilder()
                                .WithImage("quay.io/keycloak/latest")
                                .WithBindMount(GetWslAbsolutePath("./import"), "/opt/keycloak/data/import", AccessMode.ReadWrite)
                                .WithCommand(command)
                                .Build();

            _rabbitMQContainer = new RabbitMqBuilder()
                                .WithImage("rabbitmq:4.0-management")
                                .WithUsername("guest")
                                .WithPassword("guest")
                                .Build();

            var image = new ImageFromDockerfileBuilder()
                .WithDockerfileDirectory(CommonDirectoryPath.GetSolutionDirectory(), string.Empty)
                .WithDockerfile("Dockerfile")
                .Build();

            _securityApiContainer = new ContainerBuilder()
                .WithImage(image)
                .Build();
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            SetTestEnvVariables();

            builder.ConfigureTestServices(services =>
            {
                //var descriptor = services
                //    .SingleOrDefault(s => s.ServiceType == typeof(DbContextOptions<SecurityDbContext>));

                //if (descriptor is not null)
                //{
                //    services.Remove(descriptor);
                //}

                //services.AddDbContext<AccountingDbContext>(options =>
                //    options.UseNpgsql(_dbContainer.GetConnectionString()));

                //services.AddScoped<ICurrentUser, TestUserService>();
                //services.AddSingleton<ILoggerFactory, NullLoggerFactory>();
            });
        }

        public async Task InitializeAsync()
        {
            var keycloackTask = _keycloackContainer.StartAsync();
            var dbTask = _dbContainer.StartAsync();
            var rabbitTask = _rabbitMQContainer.StartAsync();

            await Task.WhenAll(keycloackTask, dbTask, rabbitTask);
        }

        public new async Task DisposeAsync()
        {
            await _keycloackContainer.DisposeAsync();
            await _dbContainer.DisposeAsync();
            await _rabbitMQContainer.DisposeAsync();
        }

        private static string GetWslAbsolutePath(string windowRealtivePath)
        {
            var path = Path.GetFullPath(windowRealtivePath);
            return "/" + path.Replace('\\', '/').Replace(":", "");
        }

        private void SetTestEnvVariables()
        {
            var keycloakPort = _keycloackContainer.GetMappedPublicPort(8080);
            var keycloackHost = _keycloackContainer.Hostname;
            var rabbitMQPort = _rabbitMQContainer.GetMappedPublicPort(5672);
            var rabbitMQHost = _rabbitMQContainer.Hostname;

            Environment.SetEnvironmentVariable("AuthServer__MetadataAddress", $"http://{keycloackHost}:{keycloakPort}/realms/ubik/.well-known/openid-configuration");
            Environment.SetEnvironmentVariable("AuthServer__Authority", $"http://{keycloackHost}:{keycloakPort}/realms/ubik");
            Environment.SetEnvironmentVariable("AuthServer__AuthorizationUrl", $"http://{keycloackHost}:{keycloakPort}/realms/ubik/protocol/openid-connect/auth");
            Environment.SetEnvironmentVariable("AuthServer__TokenUrl", $"http://{keycloackHost}:{keycloakPort}/realms/ubik/protocol/openid-connect/token");
            Environment.SetEnvironmentVariable("MessageBroker__Host", $"amqp://{rabbitMQHost}:{rabbitMQPort}");
        }
    }

    [CollectionDefinition("AuthServer and DB")]
    public class KeycloackAndDb : ICollectionFixture<IntegrationTestAccoutingFactory>
    {

    }
}

