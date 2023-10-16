﻿using Docker.DotNet.Models;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Testcontainers.Keycloak;
using Testcontainers.MariaDb;
using Ubik.Accounting.Api.Data;
using IContainer = DotNet.Testcontainers.Containers.IContainer;

namespace Ubik.Accounting.Api.Tests.Integration
{
    public class IntegrationTestWebAppFactory
    : WebApplicationFactory<Program>,
      IAsyncLifetime
    {
        private readonly MariaDbContainer _dbContainer;
        private readonly KeycloakContainer _keycloackContainer;

        public IntegrationTestWebAppFactory()
        {
            _dbContainer = new MariaDbBuilder()
                .WithImage("mariadb:latest")
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
                    options.UseMySql(_dbContainer.GetConnectionString(), ServerVersion.AutoDetect(_dbContainer.GetConnectionString())));
                
                services.AddSingleton<ILoggerFactory, NullLoggerFactory>();
            });
        }

        public async Task InitializeAsync()
        {
            await _keycloackContainer.StartAsync();
            await _dbContainer.StartAsync();
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
            Environment.SetEnvironmentVariable("MetadataAddress", $"http://{host}:{port}/realms/ubik/.well-known/openid-configuration");
            Environment.SetEnvironmentVariable("Authority", $"http://{host}:{port}/realms/ubik");
            Environment.SetEnvironmentVariable("AuthorizationUrl", $"http://{host}:{port}/realms/ubik/protocol/openid-connect/auth");
            Environment.SetEnvironmentVariable("TokenUrl", $"http://{host}:{port}/realms/ubik/protocol/openid-connect/token");
        }
    }
}
