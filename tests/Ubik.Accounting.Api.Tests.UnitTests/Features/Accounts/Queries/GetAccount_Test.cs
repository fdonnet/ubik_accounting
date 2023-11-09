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
using Ubik.ApiService.Common.Exceptions;

namespace Ubik.Accounting.Api.Tests.UnitTests.Features.Accounts.Queries
{
    public class GetAccount_Test : IAsyncLifetime
    {
        private readonly IServiceManager _serviceManager;
        private readonly GetAccountQuery _query;
        private readonly Account _account;
        private ITestHarness _harness = default!;
        private IServiceProvider _provider = default!;

        public GetAccount_Test()
        {
            _serviceManager = Substitute.For<IServiceManager>();

            _query = new GetAccountQuery()
            {
                Id = Guid.NewGuid()
            };

            _account = new Account() { Code = "TEST", Label = "Test", CurrencyId = Guid.NewGuid() };
            _serviceManager.AccountService.GetAsync(_query.Id).Returns(_account);
        }

        public async Task InitializeAsync()
        {
            _provider = new ServiceCollection()
                .AddMassTransitTestHarness(x =>
                {
                    x.AddScoped<IServiceManager>(sm => _serviceManager);
                    x.AddConsumer<GetAccountConsumer>();

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

        public async Task DisposeAsync()
        {
            await _harness.Stop();
        }
    }
}
