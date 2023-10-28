using FluentAssertions;
using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Ubik.Accounting.Api.Features;
using Ubik.Accounting.Api.Features.Accounts.Queries;
using Ubik.Accounting.Api.Models;
using Ubik.Accounting.Contracts.Accounts.Queries;
using Ubik.Accounting.Contracts.Accounts.Results;

namespace Ubik.Accounting.Api.Tests.UnitTests.Features.Accounts.Queries
{
    public class GetAllAccounts_Test : IAsyncLifetime
    {
        private readonly IServiceManager _serviceManager;
        private readonly IEnumerable<Account> _accounts;
        private ITestHarness _harness = default!;
        private IServiceProvider _provider = default!;

        public GetAllAccounts_Test()
        {
            _serviceManager = Substitute.For<IServiceManager>();
            _accounts = new Account[] { new Account() { Code = "TEST", Label = "Test", CurrencyId=Guid.NewGuid() } };
        }

        [Fact]
        public async Task GetAll_Accounts_Ok()
        {
            //Arrange
            var client = _harness.GetRequestClient<GetAllAccountsQuery>();
            var consumerHarness = _harness.GetConsumerHarness<AccountingGetAllAccountsConsumer>();

            //Act
            var response = await client.GetResponse<IGetAllAccountsResult>(new { });

            //Assert
            var sent = await _harness.Sent.Any<IGetAllAccountsResult>();
            var consumed = await _harness.Consumed.Any<GetAllAccountsQuery>();
            var consumerConsumed = await consumerHarness.Consumed.Any<GetAllAccountsQuery>();

            sent.Should().Be(true);
            consumed.Should().Be(true);
            consumerConsumed.Should().Be(true);
            response.Message.Accounts.Should().HaveCount(1);
            response.Message.Accounts.Should().AllBeOfType<GetAllAccountsResult>();
        }

        public async Task InitializeAsync()
        {
            _serviceManager.AccountService.GetAllAsync().Returns(_accounts);
            _provider = new ServiceCollection()
                .AddMassTransitTestHarness(x =>
                {
                    x.AddScoped<IServiceManager>(sm => _serviceManager);
                    x.AddConsumer<AccountingGetAllAccountsConsumer>();

                }).BuildServiceProvider(true);

            _harness = _provider.GetRequiredService<ITestHarness>();
            await _harness.Start();
        }

        public async Task DisposeAsync()
        {
            await _harness.Stop();
        }
    }
}
