using FluentAssertions;
using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Ubik.Accounting.Api.Features;
using Ubik.Accounting.Api.Features.AccountGroups.Mappers;
using Ubik.Accounting.Api.Features.AccountGroups.Queries;
using Ubik.Accounting.Api.Features.Accounts.Queries;
using Ubik.Accounting.Api.Models;
using Ubik.Accounting.Contracts.AccountGroups.Queries;
using Ubik.Accounting.Contracts.AccountGroups.Results;
using Ubik.Accounting.Contracts.Accounts.Queries;
using Ubik.Accounting.Contracts.Accounts.Results;

namespace Ubik.Accounting.Api.Tests.UnitTests.Features.AccountGroups.Queries
{

    public class GetAllAccountGroups_Test : IAsyncLifetime
    {
        private readonly IServiceManager _serviceManager;
        private readonly IEnumerable<AccountGroup> _accountGroups;
        private ITestHarness _harness = default!;
        private IServiceProvider _provider = default!;

        public GetAllAccountGroups_Test()
        {
            _serviceManager = Substitute.For<IServiceManager>();

            _accountGroups = new AccountGroup[] { new AccountGroup() { Code = "TEST", Label = "Test" } };
            _serviceManager.AccountGroupService.GetAllAsync().Returns(_accountGroups);
        }

        public async Task InitializeAsync()
        {
            
            _provider = new ServiceCollection()
                .AddMassTransitTestHarness(x =>
                {
                    x.AddScoped<IServiceManager>(sm => _serviceManager);
                    x.AddConsumer<GetAllAccountGroupsConsumer>();

                }).BuildServiceProvider(true);

            _harness = _provider.GetRequiredService<ITestHarness>();
            await _harness.Start();
        }

        [Fact]
        public async Task GetAll_AccountGroups_Ok()
        {
            //Arrange
            var client = _harness.GetRequestClient<GetAllAccountGroupsQuery>();
            var consumerHarness = _harness.GetConsumerHarness<GetAllAccountGroupsConsumer>();

            //Act
            var response = await client.GetResponse<IGetAllAccountGroupsResult>(new { });

            //Assert
            var sent = await _harness.Sent.Any<IGetAllAccountGroupsResult>();
            var consumed = await _harness.Consumed.Any<GetAllAccountGroupsQuery>();
            var consumerConsumed = await consumerHarness.Consumed.Any<GetAllAccountGroupsQuery>();

            sent.Should().Be(true);
            consumed.Should().Be(true);
            consumerConsumed.Should().Be(true);
            response.Message.AccountGroups.Should()
                .HaveCount(1)
                .And.AllBeOfType<GetAllAccountGroupsResult>();
        }
        public async Task DisposeAsync()
        {
            await _harness.Stop();
        }
    }
}
