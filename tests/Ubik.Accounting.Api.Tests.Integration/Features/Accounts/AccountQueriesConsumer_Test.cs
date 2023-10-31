using Ubik.Accounting.Api.Data.Init;
using Ubik.Accounting.Contracts.Accounts.Results;
using MassTransit;
using Ubik.Accounting.Contracts.Accounts.Queries;
using Microsoft.Extensions.DependencyInjection;
using FluentAssertions;
using Ubik.Accounting.Contracts.Accounts.Commands;
using System.Diagnostics;
using MassTransit.Testing;
using Ubik.Accounting.Api.Features.Accounts.Commands;
using Ubik.Accounting.Api.Features;
using Ubik.ApiService.Common.Filters;
using Ubik.ApiService.Common.Services;
using Ubik.Accounting.Api.Tests.Integration.Fake;

namespace Ubik.Accounting.Api.Tests.Integration.Features.Accounts
{
    public class AccountQueriesConsumer_Test : BaseIntegrationTest, IAsyncLifetime
    {
        private ITestHarness _harness = default!;
        private IServiceProvider _provider = default!;

        public AccountQueriesConsumer_Test(IntegrationTestWebAppFactory factory) : base(factory)
        {
            
        }

        public async Task InitializeAsync()
        {
            _provider = new ServiceCollection()
                .AddMassTransitTestHarness(x =>
                {
                    x.AddScoped<ICurrentUserService>(us => new FakeUserService());
                    x.AddRequestClient<GetAllAccountsQuery>();
                    x.UsingRabbitMq((context, configurator) =>
                    {

                        configurator.Host(new Uri(Environment.GetEnvironmentVariable("MessageBroker__Host")!), h =>
                        {
                            h.Username("guest");
                            h.Password("guest");
                        });

                        configurator.ConfigureEndpoints(context);
                        configurator.UseSendFilter(typeof(TenantIdSendFilter<>), context);
                        configurator.UsePublishFilter(typeof(TenantIdPublishFilter<>), context);
                    });

                }).BuildServiceProvider(true);

            _harness = _provider.GetRequiredService<ITestHarness>();
            await _harness.Start();
        }

        [Fact]
        public async Task GetAll_Accounts_Ok()
        {
            //Arrange
            var client = _harness.GetRequestClient<GetAllAccountsQuery>();

            //Act
            var result= await client.GetResponse<IGetAllAccountsResult>(new { });

            //Assert
            result.Message.Should().BeAssignableTo<IGetAllAccountsResult>();
            result.Message.Should().Match<IGetAllAccountsResult>(a => a.Accounts[0] is GetAllAccountsResult);
        }

        public async Task DisposeAsync()
        {
            await _harness.Stop();
        }

    }
}
