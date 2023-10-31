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
        private readonly ResultT<Account> _result;
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

            _result = new ResultT<Account>() { IsSuccess = true, Result = _account };

            _serviceManager.AccountService.GetAsync(_query.Id).Returns(_result);
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

        //[Fact]
        //public async Task Get_AccountNotFoundException_AccountIdNotFound()
        //{
        //    //Arrange
        //    var result = new ResultT<Account> { IsSuccess = false, Exception= new AccountNotFoundException(_account.Id) };
        //    _serviceManager.AccountService.GetAsync(_query.Id).Returns(_result);
        //    var client = _harness.GetRequestClient<AddAccountCommand>();

        //    //Act
        //    var (res, error) = await client.GetResponse<GetAccountResult, IServiceAndFeatureException>(_query);
        //    var response = await error;

        //    //Assert
        //    response.Message.Should().BeAssignableTo<IServiceAndFeatureException>();
        //    response.Message.Should().Match<IServiceAndFeatureException>(e =>
        //        e.ErrorType == ServiceAndFeatureExceptionType.Conflict
        //        && e.CustomErrors[0].ErrorCode == "ACCOUNT_ALREADY_EXISTS");
        //}

        public async Task DisposeAsync()
        {
            await _harness.Stop();
        }
    }
}
