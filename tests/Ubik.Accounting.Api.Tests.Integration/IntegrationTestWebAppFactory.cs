using Docker.DotNet.Models;
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
                                .WithBindMount("/f/Dev/ubik/tests/Ubik.Accounting.Api.Tests.Integration/import", "/opt/keycloak/data/import", AccessMode.ReadWrite)
                                .WithCommand(new string[] { "--import-realm" })
                                .Build();

        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
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

                //services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                //    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, o =>
                //    {
                //        o.MetadataAddress = $"http://localhost:{_keycloackContainer.GetMappedPublicPort(8080)}/realms/ubik/.well-known/openid-configuration";
                //        o.Authority = $"http://localhost:{_keycloackContainer.GetMappedPublicPort(8080)}/realms/ubik";
                //        o.Audience = "account";
                //        o.RequireHttpsMetadata = false;
                //    });

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
    }
}
