using Bogus.DataSets;
using DotNet.Testcontainers.Configurations;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Testcontainers.Keycloak;
using Testcontainers.PostgreSql;
using Testcontainers.RabbitMq;
using Ubik.Accounting.Api.Data;
using Ubik.Accounting.Api.Data.Init;
using Ubik.ApiService.Common.Services;

//TODO: manage to create container in parallel and see why it's create a container per test group...
namespace Ubik.Accounting.Api.Tests.Integration
{
    public class IntegrationTestWebAppFactory
    : WebApplicationFactory<Program>,
      IAsyncLifetime
    {
        private readonly PostgreSqlContainer _dbContainer;
        private readonly KeycloakContainer _keycloackContainer;
        private readonly RabbitMqContainer _rabbitMQContainer;


        public class TestUserService : ICurrentUserService
        {
            private readonly BaseValuesForUsers _testValuesForUser;
            private readonly BaseValuesForTenants _testValuesForTenants;

            public TestUserService()
            {
                _testValuesForUser = new BaseValuesForUsers();
                _testValuesForTenants = new BaseValuesForTenants();

            }

            public ICurrentUser CurrentUser =>  new CurrentUser()
                { 
                Id = _testValuesForUser.UserId1,
                Name = "TEST",
                Email = "test@test.com",
                TenantIds = new Guid[] { _testValuesForTenants.TenantId }};
           
        }

        public IntegrationTestWebAppFactory()
        {
            _dbContainer = new PostgreSqlBuilder()
                .WithImage("postgres:latest")
                .WithPassword("TEST_PASSWORD")
                .Build();

            _keycloackContainer = new KeycloakBuilder()
                                .WithImage("quay.io/keycloak/keycloak:latest")
                                .WithBindMount(GetWslAbsolutePath("./import"), "/opt/keycloak/data/import", AccessMode.ReadWrite)
                                .WithCommand(new string[] { "--import-realm" })
                                .Build();

            _rabbitMQContainer = new RabbitMqBuilder()
                                .WithImage("rabbitmq:3.12-management")
                                .WithUsername("guest")
                                .WithPassword("guest")
                                .Build();
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            SetTestEnvVariables();

            builder.ConfigureTestServices(services =>
            {
                var descriptor = services
                    .SingleOrDefault(s => s.ServiceType == typeof(DbContextOptions<AccountingContext>));

                if (descriptor is not null)
                {
                    services.Remove(descriptor);
                }

                services.AddDbContext<AccountingContext>(options =>
                    options.UseNpgsql(_dbContainer.GetConnectionString()));

                services.AddScoped<ICurrentUserService, TestUserService>();
                services.AddSingleton<ILoggerFactory, NullLoggerFactory>();
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
    public class KeycloackAndDb : ICollectionFixture<IntegrationTestWebAppFactory>
    {

    }
}
