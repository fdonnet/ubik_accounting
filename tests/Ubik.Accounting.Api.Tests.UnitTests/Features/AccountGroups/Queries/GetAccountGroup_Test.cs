using Ubik.Accounting.Api.Features;
using Ubik.Accounting.Api.Models;
using NSubstitute;
using FluentAssertions;
using Ubik.ApiService.Common.Exceptions;
using Ubik.Accounting.Api.Features.AccountGroups.Exceptions;
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
    public class GetAccountGroup_Test : IAsyncLifetime
    {
        private readonly IServiceManager _serviceManager;
        private readonly GetAccountGroupQuery _query;
        private readonly AccountGroup _accountGroup;
        private ITestHarness _harness = default!;
        private IServiceProvider _provider = default!;
        private readonly ResultT<AccountGroup> _result;

        public GetAccountGroup_Test()
        {
            _serviceManager = Substitute.For<IServiceManager>();

            _query = new GetAccountGroupQuery()
            {
                Id = Guid.NewGuid()
            };

            _accountGroup = new AccountGroup() { Code = "TEST", Label = "Test" };
            _result = new ResultT<AccountGroup>() { IsSuccess = true, Result = _accountGroup };

            _serviceManager.AccountGroupService.GetAsync(_query.Id).Returns(_result);
        }

        public async Task InitializeAsync()
        {
            _provider = new ServiceCollection()
                .AddMassTransitTestHarness(x =>
                {
                    x.AddScoped<IServiceManager>(sm => _serviceManager);
                    x.AddConsumer<GetAccountGroupConsumer>();

                }).BuildServiceProvider(true);

            _harness = _provider.GetRequiredService<ITestHarness>();
            await _harness.Start();
        }


        [Fact]
        public async Task Get_AccountGroup_Ok()
        {
            //Arrange
            var client = _harness.GetRequestClient<GetAccountGroupQuery>();
            var consumerHarness = _harness.GetConsumerHarness<GetAccountGroupConsumer>();

            //Act
            var response = await client.GetResponse<GetAccountGroupResult>(_query);

            //Assert
            var sent = await _harness.Sent.Any<GetAccountGroupResult>();
            var consumed = await _harness.Consumed.Any<GetAccountGroupQuery>();
            var consumerConsumed = await consumerHarness.Consumed.Any<GetAccountGroupQuery>();

            sent.Should().Be(true);
            consumed.Should().Be(true);
            consumerConsumed.Should().Be(true);
            response.Message.Should()
                .BeOfType<GetAccountGroupResult>()
                .And.Match<GetAccountGroupResult>(a => a.Code == _accountGroup.Code);
        }

        public async Task DisposeAsync()
        {
            await _harness.Stop();
        }
    }
}
