using FluentAssertions;
using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using System.Reflection.Metadata;
using Ubik.Accounting.Api.Features;
using Ubik.Accounting.Api.Features.Accounts.Commands;
using Ubik.Accounting.Api.Features.Accounts.Exceptions;
using Ubik.Accounting.Api.Features.Accounts.Mappers;
using Ubik.Accounting.Api.Features.Accounts.Queries;
using Ubik.Accounting.Api.Models;
using Ubik.Accounting.Contracts.Accounts.Commands;
using Ubik.Accounting.Contracts.Accounts.Queries;
using Ubik.Accounting.Contracts.Accounts.Results;
using Ubik.ApiService.Common.Exceptions;
using Ubik.ApiService.DB.Enums;

namespace Ubik.Accounting.Api.Tests.UnitTests.Features.Accounts.Commands
{
    public class AddAccount_Test : IAsyncLifetime
    {
        private readonly IServiceManager _serviceManager;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly AddAccountCommand _command;
        private readonly Account _account;
        private ITestHarness _harness = default!;
        private IServiceProvider _provider = default!;

        public AddAccount_Test()
        {
            _serviceManager = Substitute.For<IServiceManager>();
            _publishEndpoint = Substitute.For<IPublishEndpoint>();

            _command = new AddAccountCommand()
            {
                Code = "78888",
                Label = "Test",
                Description = "Test",
                CurrencyId = Guid.NewGuid(),
                Category = AccountCategory.General,
                Domain = AccountDomain.Asset
            };

            _account = _command.ToAccount();
            _serviceManager.AccountService.AddAsync(_account).Returns(_account);

            _serviceManager.AccountService.IfExistsAsync(_command.Code).Returns(false);
            _serviceManager.AccountService.IfExistsCurrencyAsync(_command.CurrencyId).Returns(true);
        }
        public async Task InitializeAsync()
        {
            _serviceManager.AccountService.AddAsync(_account).Returns(_account);
            _provider = new ServiceCollection()
                .AddMassTransitTestHarness(x =>
                {
                    x.AddScoped<IServiceManager>(sm => _serviceManager);
                    x.AddConsumer<AddAccountConsumer>();

                }).BuildServiceProvider(true);

            _harness = _provider.GetRequiredService<ITestHarness>();
            await _harness.Start();
        }


        [Fact]
        public async Task Add_Account_Ok()
        {
            //Arrange
            var client = _harness.GetRequestClient<AddAccountCommand>();
            var consumerHarness = _harness.GetConsumerHarness<AddAccountConsumer>();
            //Act
            var response = await client.GetResponse<AddAccountResult>(_command);

            //Assert
            var sent = await _harness.Sent.Any<AddAccountResult>();
            var consumed = await _harness.Consumed.Any<AddAccountCommand>();
            var consumerConsumed = await consumerHarness.Consumed.Any<AddAccountCommand>();

            sent.Should().Be(true);
            consumed.Should().Be(true);
            consumerConsumed.Should().Be(true);
            response.Message.Should().BeOfType<AddAccountResult>();
            response.Message.Should().Match<AddAccountResult>(a=>a.Code == _command.Code);
        }

        //[Fact]
        //public async Task Add_AccountAlreadyExistsException_AccountCodeAlreadyExists()
        //{
        //    ////Arrange
        //    //_serviceManager.AccountService.IfExistsAsync(_command.Code).Returns(true);
        //    //var client = _harness.GetRequestClient<AddAccountCommand>();
        //    //var consumerHarness = _harness.GetConsumerHarness<AddAccountConsumer>();

        //    ////Act
        //    //Func<Task> act = async () => await client.GetResponse<AddAccountResult>(_command);

        //    ////Assert
        //    //await act.Should().ThrowAsync<RequestFaultException>()
        //    //    .Where(e=>e.Fault<AccountAlreadyExistsException> ==);
        //}

        //[Fact]
        //public async Task Add_AccountCurrencyNotFoundException_CurrencyIdNotFound()
        //{
        //    //Arrange
        //    _serviceManager.AccountService.IfExistsCurrencyAsync(_command.CurrencyId).Returns(false);

        //    //Act
        //    Func<Task> act = async () => await _handler.Handle(_command, CancellationToken.None);

        //    //Assert
        //    await act.Should().ThrowAsync<AccountCurrencyNotFoundException>()
        //        .Where(e => e.ErrorType == ServiceAndFeatureExceptionType.BadParams);
        //}

        public async Task DisposeAsync()
        {
            await _harness.Stop();
        }
    }
}
