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
using Ubik.Accounting.Api.Data;

//TODO: manage to create container in parallel and see why it's create a container per test group...
namespace Ubik.Accounting.Api.Tests.Integration
{
    public class IntegrationTestWebAppFactory
    : WebApplicationFactory<Program>,
      IAsyncLifetime
    {
        private readonly PostgreSqlContainer _dbContainer;
        private readonly KeycloakContainer _keycloackContainer;

        public IntegrationTestWebAppFactory()
        {
            _dbContainer = new PostgreSqlBuilder()
                .WithImage("postgres:latest")
                .WithPassword("TEST_PASSWORD")
                .Build();

            _keycloackContainer = new KeycloakBuilder()
                                .WithImage("keycloak/keycloak:latest")
                                .WithBindMount(GetWslAbsolutePath("./import"), "/opt/keycloak/data/import", AccessMode.ReadWrite)
                                .WithCommand(new string[] { "--import-realm" })
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

                services.AddSingleton<ILoggerFactory, NullLoggerFactory>();
            });
        }

        public async Task InitializeAsync()
        {
            var keycloackTask = _keycloackContainer.StartAsync();
            var dbTask = _dbContainer.StartAsync();

            await Task.WhenAll(keycloackTask, dbTask);
        }

        public new async Task DisposeAsync()
        {
            await _keycloackContainer.DisposeAsync();
            await _dbContainer.DisposeAsync();
        }

        private static string GetWslAbsolutePath(string windowRealtivePath)
        {
            var path = Path.GetFullPath(windowRealtivePath);
            return "/" + path.Replace('\\', '/').Replace(":", "");
        }

        private void SetTestEnvVariables()
        {
            var port = _keycloackContainer.GetMappedPublicPort(8080);
            var host = _keycloackContainer.Hostname;
            Environment.SetEnvironmentVariable("Keycloack__MetadataAddress", $"http://{host}:{port}/realms/ubik/.well-known/openid-configuration");
            Environment.SetEnvironmentVariable("Keycloack__Authority", $"http://{host}:{port}/realms/ubik");
            Environment.SetEnvironmentVariable("Keycloack__AuthorizationUrl", $"http://{host}:{port}/realms/ubik/protocol/openid-connect/auth");
            Environment.SetEnvironmentVariable("Keycloack__TokenUrl", $"http://{host}:{port}/realms/ubik/protocol/openid-connect/token");
        }
    }

    [CollectionDefinition("Keycloack and DB")]
    public class KeycloackAndDb : ICollectionFixture<IntegrationTestWebAppFactory>
    {

    }
}
