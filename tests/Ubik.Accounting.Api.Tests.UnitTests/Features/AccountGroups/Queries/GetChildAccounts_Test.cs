using Ubik.Accounting.Api.Features;
using Ubik.Accounting.Api.Models;
using NSubstitute;
using FluentAssertions;
using Ubik.Accounting.Api.Features.AccountGroups.Exceptions;
using Ubik.ApiService.Common.Exceptions;
using MassTransit.Testing;
using Ubik.Accounting.Contracts.AccountGroups.Queries;
using Microsoft.Extensions.DependencyInjection;
using MassTransit;
using Ubik.Accounting.Api.Features.AccountGroups.Queries;
using Ubik.Accounting.Api.Features.Accounts.Queries;
using Ubik.Accounting.Contracts.Accounts.Queries;
using Ubik.Accounting.Contracts.Accounts.Results;
using Ubik.Accounting.Contracts.AccountGroups.Results;

namespace Ubik.Accounting.Api.Tests.UnitTests.Features.AccountGroups.Queries
{
    public class GetChildAccounts_Test : IAsyncLifetime
    {
        private readonly IServiceManager _serviceManager;
        private readonly GetChildAccountsQuery _query;
        private readonly AccountGroup _accountGroup;
        private readonly ResultT<AccountGroup> _result;
        private ITestHarness _harness = default!;
        private IServiceProvider _provider = default!;

        public GetChildAccounts_Test()
        {
            _serviceManager = Substitute.For<IServiceManager>();

            _query = new GetChildAccountsQuery()
            {
                AccountGroupId = Guid.NewGuid()
            };

            _accountGroup = new AccountGroup()
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

            _result = new ResultT<AccountGroup>() { IsSuccess = true, Result = _accountGroup };

            _serviceManager.AccountGroupService.GetWithChildAccountsAsync(_query.AccountGroupId)
                .Returns(_result);
        }
        public async Task InitializeAsync()
        {
            _provider = new ServiceCollection()
                .AddMassTransitTestHarness(x =>
                {
                    x.AddScoped<IServiceManager>(sm => _serviceManager);
                    x.AddConsumer<GetChildAccountsConsumer>();

                }).BuildServiceProvider(true);

            _harness = _provider.GetRequiredService<ITestHarness>();
            await _harness.Start();
        }

        [Fact]
        public async Task GetAll_Accounts_Ok()
        {
            //Arrange
            var client = _harness.GetRequestClient<GetChildAccountsQuery>();
            var consumerHarness = _harness.GetConsumerHarness<GetChildAccountsConsumer>();

            //Act
            var response = await client.GetResponse<IGetChildAccountsResults>(_query);

            //Assert
            var sent = await _harness.Sent.Any<IGetChildAccountsResults>();
            var consumed = await _harness.Consumed.Any<GetChildAccountsQuery>();
            var consumerConsumed = await consumerHarness.Consumed.Any<GetChildAccountsQuery>();

            sent.Should().Be(true);
            consumed.Should().Be(true);
            consumerConsumed.Should().Be(true);
            response.Message.Should()
                .BeAssignableTo<IGetChildAccountsResults>();
        }

        public async Task DisposeAsync()
        {
            await _harness.Stop();
        }
    }
}
