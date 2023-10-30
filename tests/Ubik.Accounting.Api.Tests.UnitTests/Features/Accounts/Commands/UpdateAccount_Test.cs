﻿using NSubstitute;
using Ubik.Accounting.Api.Features;
using Ubik.Accounting.Api.Models;
using Ubik.Accounting.Api.Features.Accounts.Mappers;
using FluentAssertions;
using Ubik.Accounting.Api.Features.Accounts.Exceptions;
using Ubik.ApiService.Common.Exceptions;
using MassTransit;
using Ubik.Accounting.Contracts.Accounts.Commands;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using Ubik.Accounting.Api.Features.Accounts.Commands;
using Ubik.Accounting.Contracts.Accounts.Results;
using Ubik.Accounting.Contracts.Accounts.Events;

namespace Ubik.Accounting.Api.Tests.UnitTests.Features.Accounts.Commands
{
    public class UpdateAccount_Test : IAsyncLifetime
    {
        private readonly IServiceManager _serviceManager;
        private readonly UpdateAccountCommand _command;
        private readonly Account _account;
        private ITestHarness _harness = default!;
        private IServiceProvider _provider = default!;

        public UpdateAccount_Test()
        {
            _serviceManager = Substitute.For<IServiceManager>();

            _command = new UpdateAccountCommand()
            {
                Id = Guid.NewGuid(),
                Code = "78888",
                Label = "Test",
                Description = "Test",
                CurrencyId = Guid.NewGuid(),
                Version = Guid.NewGuid(),
            };

            _account = new Account() { Code = "1800", Label = "1000", CurrencyId=Guid.NewGuid() };
            _account = _command.ToAccount(_account);

            _serviceManager.AccountService.UpdateAsync(Arg.Any<Account>()).Returns(new ResultT<Account> { Result = _account, IsSuccess = true });
            _serviceManager.AccountService.IfExistsWithDifferentIdAsync(_command.Code, _command.Id).Returns(false);
            _serviceManager.AccountService.IfExistsCurrencyAsync(_command.CurrencyId).Returns(true);
            _serviceManager.AccountService.GetAsync(_command.Id).Returns(new ResultT<Account> { Result = _account, IsSuccess = true });
        }

        public async Task InitializeAsync()
        {
            _provider = new ServiceCollection()
                .AddMassTransitTestHarness(x =>
                {
                    x.AddScoped<IServiceManager>(sm => _serviceManager);
                    x.AddConsumer<UpdateAccountConsumer>();

                }).BuildServiceProvider(true);

            _harness = _provider.GetRequiredService<ITestHarness>();
            await _harness.Start();
        }

        [Fact]
        public async Task Upd_Account_Ok()
        {
            //Arrange
            var client = _harness.GetRequestClient<UpdateAccountCommand>();
            var consumerHarness = _harness.GetConsumerHarness<UpdateAccountConsumer>();
            //Act
            var response = await client.GetResponse<UpdateAccountResult>(_command);

            //Assert
            var sent = await _harness.Sent.Any<UpdateAccountResult>();
            var consumed = await _harness.Consumed.Any<UpdateAccountCommand>();
            var consumerConsumed = await consumerHarness.Consumed.Any<UpdateAccountCommand>();

            sent.Should().Be(true);
            consumed.Should().Be(true);
            consumerConsumed.Should().Be(true);
            response.Message.Should().BeOfType<UpdateAccountResult>();
            response.Message.Should().Match<UpdateAccountResult>(a => a.Code == _command.Code);
        }

        [Fact]
        public async Task Upd_Account_OkAccountUpdatedPublished()
        {
            //Arrange
            var client = _harness.GetRequestClient<UpdateAccountCommand>();

            //Act
            await client.GetResponse<UpdateAccountResult>(_command);

            //Assert
            var sent = await _harness.Published.Any<AccountUpdated>();

            sent.Should().Be(true);
        }

        //[Fact]
        //public async Task Upd_AccountAlreadyExistsException_AccountCodeAlreadyExistsWithDifferentId()
        //{
        //    //Arrange
        //    _serviceManager.AccountService.IfExistsWithDifferentIdAsync(_command.Code, _command.Id).Returns(true);
        //    var client = _harness.GetRequestClient<UpdateAccountCommand>();

        //    //Act
        //    var (result, error) = await client.GetResponse<UpdateAccountResult, IServiceAndFeatureException>(_command);
        //    var response = await error;

        //    //Assert
        //    response.Message.Should().BeAssignableTo<IServiceAndFeatureException>();
        //    response.Message.Should().Match<IServiceAndFeatureException>(e => 
        //        e.ErrorType == ServiceAndFeatureExceptionType.Conflict
        //        && e.CustomErrors[0].ErrorCode == "ACCOUNT_ALREADY_EXISTS");
        //}

        //[Fact]
        //public async Task Upd_AccountNotFoundException_AccountIdNotFound()
        //{
        //    //Arrange
        //    _serviceManager.AccountService.GetAsync(_command.Id).Returns(new ResultT<Account> 
        //    { IsSuccess=false,Exception=new AccountNotFoundException(_command.Id)});

        //    var client = _harness.GetRequestClient<UpdateAccountCommand>();

        //    //Act
        //    var (result, error) = await client.GetResponse<UpdateAccountResult, IServiceAndFeatureException>(_command);
        //    var response = await error;

        //    //Assert
        //    response.Message.Should().BeAssignableTo<IServiceAndFeatureException>();
        //    response.Message.Should().Match<IServiceAndFeatureException>(e =>
        //        e.ErrorType == ServiceAndFeatureExceptionType.NotFound
        //        && e.CustomErrors[0].ErrorCode == "ACCOUNT_NOT_FOUND");
        //}

        //[Fact]
        //public async Task Upd_AccountCurrencyNotFoundException_CurrencyIdNotFound()
        //{
        //    //Arrange
        //    _serviceManager.AccountService.IfExistsCurrencyAsync(_command.CurrencyId).Returns(false);
        //    var client = _harness.GetRequestClient<UpdateAccountCommand>();

        //    //Act
        //    var (result, error) = await client.GetResponse<UpdateAccountResult, IServiceAndFeatureException>(_command);
        //    var response = await error;

        //    //Assert
        //    response.Message.Should().BeAssignableTo<IServiceAndFeatureException>();
        //    response.Message.Should().Match<IServiceAndFeatureException>(e =>
        //        e.ErrorType == ServiceAndFeatureExceptionType.BadParams
        //        && e.CustomErrors[0].ErrorCode == "ACCOUNT_CURRENCY_NOT_FOUND");
        //}

        public async Task DisposeAsync()
        {
            await _harness.Stop();
        }
    }
}
