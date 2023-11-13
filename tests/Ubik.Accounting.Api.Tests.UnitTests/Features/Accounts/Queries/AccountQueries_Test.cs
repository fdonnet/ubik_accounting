using FluentAssertions;
using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Ubik.Accounting.Api.Features;
using Ubik.Accounting.Api.Features.Accounts.Queries;
using Ubik.Accounting.Api.Features.Accounts.Queries.CustomPoco;
using Ubik.Accounting.Api.Models;
using Ubik.Accounting.Contracts.Accounts.Queries;
using Ubik.Accounting.Contracts.Accounts.Results;

namespace Ubik.Accounting.Api.Tests.UnitTests.Features.Accounts.Queries
{
    public class AccountQueries_Test : IAsyncLifetime
    {
        private readonly IServiceManager _serviceManager;
        private readonly GetAccountQuery _query;
        private readonly GetAccountGroupsForAccountQuery _queryAccountGroups;
        private readonly Account _account;
        private ITestHarness _harness = default!;
        private IServiceProvider _provider = default!;

        public AccountQueries_Test()
        {
            _serviceManager = Substitute.For<IServiceManager>();

            _query = new GetAccountQuery()
            {
                Id = Guid.NewGuid()
            };

            _queryAccountGroups = new GetAccountGroupsForAccountQuery()
            {
                AccountId = Guid.NewGuid()
            };

            _account = new Account() { Code = "TEST", Label = "Test", CurrencyId = Guid.NewGuid() };
            _serviceManager.AccountService.GetAsync(_query.Id).Returns(_account);
            _serviceManager.AccountService.GetAccountGroupsAsync(_queryAccountGroups.AccountId).Returns(new List<AccountGroupClassification>
            {
                new AccountGroupClassification{ClassificationCode="TEst",ClassificationId=Guid.NewGuid()},
            });
        }

        public async Task InitializeAsync()
        {
            _provider = new ServiceCollection()
                .AddMassTransitTestHarness(x =>
                {
                    x.AddScoped<IServiceManager>(sm => _serviceManager);
                    x.AddConsumer<GetAccountConsumer>();
                    x.AddConsumer<GetAccountGroupsForAccountConsumer>();

                }).BuildServiceProvider(true);

            _harness = _provider.GetRequiredService<ITestHarness>();
            await _harness.Start();
        }

        [Fact]
        public async Task Get_Account_Ok()
        {
            //Arrange
            var client = _harness.GetRequestClient<GetAccountQuery>();
            var consumerHarness = _harness.GetConsumerHarness<GetAccountConsumer>();

            //Act
            var response = await client.GetResponse<GetAccountResult>(_query);

            //Assert
            var sent = await _harness.Sent.Any<GetAccountResult>();
            var consumed = await _harness.Consumed.Any<GetAccountQuery>();
            var consumerConsumed = await consumerHarness.Consumed.Any<GetAccountQuery>();

            sent.Should().Be(true);
            consumed.Should().Be(true);
            consumerConsumed.Should().Be(true);
            response.Message.Should()
                .BeOfType<GetAccountResult>()
                .And.Match<GetAccountResult>(a => a.Code == _account.Code);
        }

        [Fact]
        public async Task GetAccountGroups_AccountGroupsClassification_Ok()
        {
            //Arrange
            var client = _harness.GetRequestClient<GetAccountGroupsForAccountQuery>();
            var consumerHarness = _harness.GetConsumerHarness<GetAccountGroupsForAccountConsumer>();

            //Act
            var response = await client.GetResponse<GetAccountGroupClassificationResults>(_queryAccountGroups);

            //Assert
            var sent = await _harness.Sent.Any<GetAccountGroupClassificationResults>();
            var consumed = await _harness.Consumed.Any<GetAccountGroupsForAccountQuery>();
            var consumerConsumed = await consumerHarness.Consumed.Any<GetAccountGroupsForAccountQuery>();

            sent.Should().Be(true);
            consumed.Should().Be(true);
            consumerConsumed.Should().Be(true);
            response.Message.Should()
                .BeOfType<GetAccountGroupClassificationResults>()
                .And.Match<GetAccountGroupClassificationResults>(a => a.AccountGroups.Any());
        }

        public async Task DisposeAsync()
        {
            await _harness.Stop();
        }
    }
}
