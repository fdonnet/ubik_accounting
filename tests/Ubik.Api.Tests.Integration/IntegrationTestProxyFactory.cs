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
using LanguageExt.Pipes;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using LanguageExt;
using System.Net.Http;
using Ubik.ApiService.Common.Errors;
using Ubik.Security.Contracts.Users.Commands;
using System.Text.Json;
using Microsoft.EntityFrameworkCore.Query;
using Testcontainers.Redis;

namespace Ubik.Api.Tests.Integration
{
    public class IntegrationTestProxyFactory
        : WebApplicationFactory<Program>,
      IAsyncLifetime
    {
        private readonly PostgreSqlContainer _dbContainer;
        public KeycloakContainer KeycloackContainer;
        private readonly RabbitMqContainer _rabbitMQContainer;
        private readonly RedisContainer _redisContainer;
        private IContainer? _securityApiContainer;
        private readonly INetwork _network;
        private readonly IFutureDockerImage _securityApiContainerImg;
        private static readonly string[] command = ["--import-realm"];
        private readonly HttpClient _client = new();
        private readonly bool _reUse = true;

        public IntegrationTestProxyFactory()
        {
            _network = new NetworkBuilder()
                .WithName("network-test")
                .WithLabel("reuse-id", "network-test")
                .WithReuse(_reUse)
                .Build();

            _dbContainer = new PostgreSqlBuilder()
                .WithImage("postgres:latest")
                .WithName("postgres-db-test")
                .WithPassword("test01")
                .WithNetwork(_network)
                .WithNetworkAliases("db-test")
                .WithPortBinding(5433, true)
                .WithLabel("reuse-id", "postges-db-test")
                .WithReuse(_reUse)
                .Build();

            _redisContainer = new RedisBuilder()
                .WithImage("redis/redis-stack:latest")
                .WithName("redis-proxy-test")
                .WithNetwork(_network)
                .WithNetworkAliases("redis-proxy-test")
                .WithLabel("reuse-id", "redis-proxy-test")
                .WithReuse(_reUse)
                .Build();


            KeycloackContainer = new KeycloakBuilder()
                                .WithImage("keycloak/keycloak:26.0")
                                .WithBindMount(GetWslAbsolutePath("./import"), "/opt/keycloak/data/import", AccessMode.ReadWrite)
                                .WithCommand(command)
                                .WithNetwork(_network)
                                .WithPortBinding(9000, 9000)
                                .WithPortBinding(8086, 8080)
                                .WithNetworkAliases("keycloak")
                                .WithName("keycloak-test")
                                .WithLabel("reuse-id", "keycloak-test")
                                .WithReuse(_reUse)
                                .Build();

            _rabbitMQContainer = new RabbitMqBuilder()
                                .WithImage("rabbitmq:4.0-management")
                                .WithUsername("guest")
                                .WithPassword("guest")
                                .WithPortBinding(5672, true)
                                .WithNetwork(_network)
                                .WithNetworkAliases("rabbit")
                                .WithName("rabbit-test")
                                .WithLabel("reuse-id", "rabbit-test")
                                .WithReuse(_reUse)
                                .Build();

            _securityApiContainerImg = new ImageFromDockerfileBuilder()
                .WithName("security-api-test")
                .WithDockerfileDirectory(CommonDirectoryPath.GetGitDirectory(), string.Empty)
                .WithDockerfile("src/Ubik.Security.Api/Dockerfile")
                .Build();

        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            SetTestEnvVariablesForProxy();
        }

        public async Task InitializeAsync()
        {
            //Cannot use task when all, I dont' know why.
            var taskDb = _dbContainer.StartAsync();
            var taskRabbit = _rabbitMQContainer.StartAsync();
            var taskKeycloak = KeycloackContainer.StartAsync();
            var taskRedisProxy = _redisContainer.StartAsync();

            await Task.WhenAll(taskDb, taskRabbit, taskRedisProxy);

            //Need to be alone.
            await taskKeycloak;

            //Conf apis
            await _securityApiContainerImg.CreateAsync();
            ConfigureApis();

            //Start apis
            await _securityApiContainer!.StartAsync();
        }

        private void ConfigureApis()
        {
            _securityApiContainer = new ContainerBuilder()
                .WithNetwork(_network)
                .WithNetworkAliases("api-security")
                .WithImage(_securityApiContainerImg)
                .WithEnvironment("AuthServer__MetadataAddress", $"http://keycloak:8080/realms/ubik/.well-known/openid-configuration")
                .WithEnvironment("AuthServer__Authority", $"http://keycloak:8080/realms/ubik")
                .WithEnvironment("AuthServer__AuthorizationUrl", $"http://keycloak:8080/realms/ubik/protocol/openid-connect/auth")
                .WithEnvironment("AuthServer__TokenUrl", $"http://keycloak:8080/realms/ubik/.protocol/openid-connect/token")
                .WithEnvironment("ConnectionStrings__SecurityDbContext", $"Host=db-test;Port=5432;Database=ubik_security;Username=postgres;Password=test01")
                .WithEnvironment("AuthManagerKeyCloakClient__RootUrl", $"http://keycloak:8080/")
                .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Development")
                .WithEnvironment("MessageBroker__Host", $"amqp://rabbit:5672")
                .WithLabel("reuse-id", "security-api")
                .WithPortBinding(8080, true)
                .WithPortBinding(8081, true)
                .Build();
        }

        public new async Task DisposeAsync()
        {
            await KeycloackContainer.DisposeAsync();
            await _dbContainer.DisposeAsync();
            await _rabbitMQContainer.DisposeAsync();
            await _securityApiContainer!.DisposeAsync();
            await _redisContainer.DisposeAsync();

            _client.Dispose();
        }

        private static string GetWslAbsolutePath(string windowRealtivePath)
        {
            var path = Path.GetFullPath(windowRealtivePath);
            return "/" + path.Replace('\\', '/').Replace(":", "");
        }

        private void SetTestEnvVariablesForProxy()
        {
            Environment.SetEnvironmentVariable("AuthServer__MetadataAddress", $"http://localhost:8086/realms/ubik/.well-known/openid-configuration");
            Environment.SetEnvironmentVariable("AuthServer__Authority", $"http://localhost:8086/realms/ubik");
            Environment.SetEnvironmentVariable("AuthServer__AuthorizationUrl", $"http://localhost:8086/realms/ubik/protocol/openid-connect/auth");
            Environment.SetEnvironmentVariable("AuthServer__TokenUrl", $"http://localhost:8086/realms/ubik/protocol/openid-connect/token");
            Environment.SetEnvironmentVariable("ReverseProxy__Clusters__ubik_users_admin__Destinations__destination1__Address", $"http://{_securityApiContainer!.Hostname}:{_securityApiContainer.GetMappedPublicPort(8080)}/");
            Environment.SetEnvironmentVariable("RedisCache__ConnectionString", $"{_redisContainer.Hostname}:{_redisContainer.GetMappedPublicPort(6379)}");
            Environment.SetEnvironmentVariable("ApiSecurityForAdmin__HostAndPort", $"http://{_securityApiContainer.Hostname}:{_securityApiContainer.GetMappedPublicPort(8080)}/");
            var authTokenUrl = $"http://localhost:8086/";

            _client.BaseAddress = new Uri(authTokenUrl);
        }

    }

    [CollectionDefinition("Proxy")]
    public class KeycloackAndDb : ICollectionFixture<IntegrationTestProxyFactory>
    {

    }
}

