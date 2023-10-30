﻿using Ubik.Accounting.Api.Features;
using Ubik.Accounting.Api.Models;
using NSubstitute;
using FluentAssertions;
using Ubik.Accounting.Api.Features.Accounts.Exceptions;
using Ubik.ApiService.Common.Exceptions;
using MassTransit;
using Ubik.Accounting.Contracts.Accounts.Commands;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Principal;
using Ubik.Accounting.Api.Features.Accounts.Commands;
using Ubik.Accounting.Contracts.Accounts.Results;
using Ubik.Accounting.Contracts.Accounts.Events;

namespace Ubik.Accounting.Api.Tests.UnitTests.Features.Accounts.Commands
{
    public class DeleteAccount_Test : IAsyncLifetime
    {
        private readonly IServiceManager _serviceManager;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly DeleteAccountCommand _command;
        private readonly Guid _idToDelete;
        private ITestHarness _harness = default!;
        private IServiceProvider _provider = default!;


        public DeleteAccount_Test()
        {
            _serviceManager = Substitute.For<IServiceManager>();
            _publishEndpoint = Substitute.For<IPublishEndpoint>();
            _idToDelete = Guid.NewGuid();
            _command = new DeleteAccountCommand() { Id=_idToDelete};

            _serviceManager.AccountService.ExecuteDeleteAsync(_idToDelete).Returns(true);
            _serviceManager.AccountService.GetAsync(_idToDelete).Returns
                (new Account() { Id = _idToDelete, Code = "test", Label = "test", CurrencyId = Guid.NewGuid() });
        }

        public async Task InitializeAsync()
        {
            _provider = new ServiceCollection()
                .AddMassTransitTestHarness(x =>
                {
                    x.AddScoped<IServiceManager>(sm => _serviceManager);
                    x.AddConsumer<DeleteAccountConsumer>();

                }).BuildServiceProvider(true);

            _harness = _provider.GetRequiredService<ITestHarness>();
            await _harness.Start();
        }


        [Fact]
        public async Task Del_Account_Ok()
        {
            //Arrange
            var client = _harness.GetRequestClient<DeleteAccountCommand>();
            var consumerHarness = _harness.GetConsumerHarness<DeleteAccountConsumer>();
            //Act
            var response = await client.GetResponse<DeleteAccountResult>(_command);

            //Assert
            var sent = await _harness.Sent.Any<DeleteAccountResult>();
            var consumed = await _harness.Consumed.Any<DeleteAccountCommand>();
            var consumerConsumed = await consumerHarness.Consumed.Any<DeleteAccountCommand>();

            sent.Should().Be(true);
            consumed.Should().Be(true);
            consumerConsumed.Should().Be(true);
            response.Message.Should().BeOfType<DeleteAccountResult>();
            response.Message.Should().Match<DeleteAccountResult>(a => a.Deleted == true);
        }

        [Fact]
        public async Task Del_Account_OkAccountDeletedPublished()
        {
            //Arrange
            var client = _harness.GetRequestClient<DeleteAccountCommand>();

            //Act
            await client.GetResponse<DeleteAccountResult>(_command);

            //Assert
            var sent = await _harness.Published.Any<AccountDeleted>();

            sent.Should().Be(true);
        }

        [Fact]
        public async Task Del_AccountNotFoundException_AccountIdNotFound()
        {
            //Arrange
            _serviceManager.AccountService.GetAsync(_idToDelete).Returns(Task.FromResult<Account?>(null));
            var client = _harness.GetRequestClient<DeleteAccountCommand>();

            //Act
            var (result, error) = await client.GetResponse<DeleteAccountResult, IServiceAndFeatureException>(_command);
            var response = await error;

            //Assert
            response.Message.Should().BeAssignableTo<IServiceAndFeatureException>();
            response.Message.Should().Match<IServiceAndFeatureException>(e => 
                e.ErrorType == ServiceAndFeatureExceptionType.NotFound
                && e.CustomErrors[0].ErrorCode == "ACCOUNT_NOT_FOUND");
        }

        public async Task DisposeAsync()
        {
            await _harness.Stop();
        }
    }
}
