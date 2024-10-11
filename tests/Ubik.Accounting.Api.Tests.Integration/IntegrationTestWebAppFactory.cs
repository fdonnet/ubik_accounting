using DotNet.Testcontainers.Configurations;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Testcontainers.Keycloak;
using Testcontainers.PostgreSql;
using Testcontainers.RabbitMq;
using Ubik.Accounting.Api.Data;
using Ubik.Accounting.Api.Data.Init;
using Ubik.ApiService.Common.Services;

namespace Ubik.Accounting.Api.Tests.Integration
{
    public class IntegrationTestWebAppFactory
    : WebApplicationFactory<Program>,
      IAsyncLifetime
    {
        private readonly PostgreSqlContainer _dbContainer;
        private readonly KeycloakContainer _keycloackContainer;
        private readonly RabbitMqContainer _rabbitMQContainer;
        private static readonly string[] command = ["--import-realm"];

        public class TestUserService : ICurrentUser
        {
            private readonly BaseValuesForUsers _testValuesForUser;
            private readonly BaseValuesForTenants _testValuesForTenants;
            private readonly CurrentUser _currentUser;

            public TestUserService()
            {
                _testValuesForUser = new BaseValuesForUsers();
                _testValuesForTenants = new BaseValuesForTenants();
                _currentUser = new CurrentUser()
                {
                    Id = _testValuesForUser.UserId1,
                    TenantId =  _testValuesForTenants.TenantId 
                };

            }

            public Guid Id   
            {
                get { return _currentUser.Id; } 
                set { _currentUser.Id = value; }
            }

            public Guid? TenantId
            {
                get { return _currentUser.TenantId; }
                set { _currentUser.TenantId = value; }
            }

            public bool IsMegaAdmin
            {
                get { return _currentUser.IsMegaAdmin; }
                set { _currentUser.IsMegaAdmin = value; }
            }

        }

        public IntegrationTestWebAppFactory()
        {
            _dbContainer = new PostgreSqlBuilder()
                .WithImage("postgres:latest")
                .WithPassword("TEST_PASSWORD")
                .Build();

            _keycloackContainer = new KeycloakBuilder()
                                .WithImage("quay.io/keycloak/keycloak:21.1")
                                .WithBindMount(GetWslAbsolutePath("./import"), "/opt/keycloak/data/import", AccessMode.ReadWrite)
                                .WithCommand(command)
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
                    .SingleOrDefault(s => s.ServiceType == typeof(DbContextOptions<AccountingDbContext>));

                if (descriptor is not null)
                {
                    services.Remove(descriptor);
                }

                services.AddDbContext<AccountingDbContext>(options =>
                    options.UseNpgsql(_dbContainer.GetConnectionString()));

                services.AddScoped<ICurrentUser, TestUserService>();
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
