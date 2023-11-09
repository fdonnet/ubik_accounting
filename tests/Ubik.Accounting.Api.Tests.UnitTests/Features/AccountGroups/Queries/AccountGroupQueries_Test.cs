using Ubik.Accounting.Api.Features;
using Ubik.Accounting.Api.Models;
using NSubstitute;
using FluentAssertions;
using Ubik.ApiService.Common.Exceptions;
using MassTransit.Testing;
using Ubik.Accounting.Contracts.AccountGroups.Queries;
using Microsoft.Extensions.DependencyInjection;
using MassTransit;
using Ubik.Accounting.Api.Features.AccountGroups.Queries;
using Ubik.Accounting.Contracts.AccountGroups.Results;

namespace Ubik.Accounting.Api.Tests.UnitTests.Features.AccountGroups.Queries
{
    public class AccountGroupQueries_Test : IAsyncLifetime
    {
        private readonly IServiceManager _serviceManager;
        private ITestHarness _harness = default!;
        private IServiceProvider _provider = default!;


        public AccountGroupQueries_Test()
        {
            _serviceManager = Substitute.For<IServiceManager>();
        }

        public async Task InitializeAsync()
        {
            _provider = new ServiceCollection()
                .AddMassTransitTestHarness(x =>
                {
                    x.AddScoped<IServiceManager>(sm => _serviceManager);
                    x.AddConsumer<GetAccountGroupConsumer>();
                    x.AddConsumer<GetAllAccountGroupsConsumer>();
                    x.AddConsumer<GetChildAccountsConsumer>();

                }).BuildServiceProvider(true);

            _harness = _provider.GetRequiredService<ITestHarness>();
            await _harness.Start();
        }


        [Fact]
        public async Task Get_AccountGroup_Ok()
        {
            //Arrange
            var accountGroup = new AccountGroup() { Code = "TEST", Label = "Test" };
            //var result = new ResultT<AccountGroup>() { IsSuccess = true, Result = accountGroup };
            var query = new GetAccountGroupQuery()
            {
                Id = Guid.NewGuid()
            };

            _serviceManager.AccountGroupService.GetAsync(query.Id).Returns(accountGroup);
            var client = _harness.GetRequestClient<GetAccountGroupQuery>();
            var consumerHarness = _harness.GetConsumerHarness<GetAccountGroupConsumer>();

            //Act
            var response = await client.GetResponse<GetAccountGroupResult>(query);

            //Assert
            var sent = await _harness.Sent.Any<GetAccountGroupResult>();
            var consumed = await _harness.Consumed.Any<GetAccountGroupQuery>();
            var consumerConsumed = await consumerHarness.Consumed.Any<GetAccountGroupQuery>();

            sent.Should().Be(true);
            consumed.Should().Be(true);
            consumerConsumed.Should().Be(true);
            response.Message.Should()
                .BeOfType<GetAccountGroupResult>()
                .And.Match<GetAccountGroupResult>(a => a.Code == accountGroup.Code);
        }

        [Fact]
        public async Task GetAll_AccountGroups_Ok()
        {
            //Arrange
            var accountGroups = new AccountGroup[] { new AccountGroup() { Code = "TEST", Label = "Test" } };

            _serviceManager.AccountGroupService.GetAllAsync().Returns(accountGroups);
            var client = _harness.GetRequestClient<GetAllAccountGroupsQuery>();
            var consumerHarness = _harness.GetConsumerHarness<GetAllAccountGroupsConsumer>();

            //Act
            var response = await client.GetResponse<GetAllAccountGroupsResults>(new { });

            //Assert
            var sent = await _harness.Sent.Any<GetAllAccountGroupsResults>();
            var consumed = await _harness.Consumed.Any<GetAllAccountGroupsQuery>();
            var consumerConsumed = await consumerHarness.Consumed.Any<GetAllAccountGroupsQuery>();

            sent.Should().Be(true);
            consumed.Should().Be(true);
            consumerConsumed.Should().Be(true);
            response.Message.AccountGroups.Should()
                .HaveCount(1)
                .And.AllBeOfType<GetAllAccountGroupsResult>();
        }

        [Fact]
        public async Task GetAllChild_Accounts_Ok()
        {
            //Arrange
            var query = new GetChildAccountsQuery()
            {
                AccountGroupId = Guid.NewGuid()
            };

            var accountGroup = new AccountGroup()
            {
                Code = "TEST",
                Label = "Test",
                Accounts = new Account[]{ new Account
                {
                    Label="TEST",
                    Code="TEST",
                    CurrencyId=Guid.NewGuid()
                }
                }
            };

            _serviceManager.AccountGroupService.GetWithChildAccountsAsync(query.AccountGroupId)
                .Returns(accountGroup);

            var client = _harness.GetRequestClient<GetChildAccountsQuery>();
            var consumerHarness = _harness.GetConsumerHarness<GetChildAccountsConsumer>();

            //Act
            var response = await client.GetResponse<GetChildAccountsResults>(query);

            //Assert
            var sent = await _harness.Sent.Any<GetChildAccountsResults>();
            var consumed = await _harness.Consumed.Any<GetChildAccountsQuery>();
            var consumerConsumed = await consumerHarness.Consumed.Any<GetChildAccountsQuery>();

            sent.Should().Be(true);
            consumed.Should().Be(true);
            consumerConsumed.Should().Be(true);
            response.Message.Should()
                .BeAssignableTo<GetChildAccountsResults>();
        }

        public async Task DisposeAsync()
        {
            await _harness.Stop();
        }
    }
}
