using FluentAssertions;
using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Ubik.Accounting.Api.Features;
using Ubik.Accounting.Api.Features.Accounts.Commands;
using Ubik.Accounting.Api.Models;
using Ubik.Accounting.Contracts.Accounts.Commands;
using Ubik.Accounting.Contracts.Accounts.Events;
using Ubik.Accounting.Contracts.Accounts.Results;

namespace Ubik.Accounting.Api.Tests.UnitTests.Features.Accounts.Commands
{
    public class AccountCommands_Test : IAsyncLifetime
    {
        private readonly IServiceManager _serviceManager;
        private readonly AddAccountInAccountGroupCommand _command;
        private readonly DeleteAccountInAccountGroupCommand _commandDel;
        private ITestHarness _harness = default!;
        private IServiceProvider _provider = default!;

        public AccountCommands_Test()
        {
            _serviceManager = Substitute.For<IServiceManager>();

            _command = new AddAccountInAccountGroupCommand()
            {
               AccountGroupId = NewId.NextGuid(),
               AccountId = NewId.NextGuid()
            };

            _commandDel = new DeleteAccountInAccountGroupCommand()
            {
                AccountGroupId = NewId.NextGuid(),
                AccountId = NewId.NextGuid()
            };

            _serviceManager.AccountService.AddInAccountGroupAsync(_command.AccountId,_command.AccountGroupId)
                .Returns(new AccountAccountGroup { AccountGroupId = _command.AccountGroupId,
                                                   AccountId = _command.AccountId });

            _serviceManager.AccountService.DeleteFromAccountGroupAsync(_commandDel.AccountId, _commandDel.AccountGroupId)
                .Returns(new AccountAccountGroup
                {
                    AccountGroupId = _commandDel.AccountGroupId,
                    AccountId = _commandDel.AccountId
                });
        }

        public async Task InitializeAsync()
        {
            _provider = new ServiceCollection()
                .AddMassTransitTestHarness(x =>
                {
                    x.AddScoped<IServiceManager>(sm => _serviceManager);
                    x.AddConsumer<AddAccountInAccountGroupConsumer>();
                    x.AddConsumer<DeleteAccountInAccountGroupConsumer>();

                }).BuildServiceProvider(true);

            _harness = _provider.GetRequiredService<ITestHarness>();
            await _harness.Start();
        }
        [Fact]
        public async Task AddInAccountGroup_AccountAccountGroup_Ok()
        {
            //Arrange
            var client = _harness.GetRequestClient<AddAccountInAccountGroupCommand>();
            var consumerHarness = _harness.GetConsumerHarness<AddAccountInAccountGroupConsumer>();
            //Act
            var response = await client.GetResponse<AddAccountInAccountGroupResult>(_command);

            //Assert
            var sent = await _harness.Sent.Any<AddAccountInAccountGroupResult>();
            var consumed = await _harness.Consumed.Any<AddAccountInAccountGroupCommand>();
            var consumerConsumed = await consumerHarness.Consumed.Any<AddAccountInAccountGroupCommand>();

            sent.Should().Be(true);
            consumed.Should().Be(true);
            consumerConsumed.Should().Be(true);
            response.Message.Should()
                .BeOfType<AddAccountInAccountGroupResult>()
                .And.Match<AddAccountInAccountGroupResult>(a => a.AccountId == _command.AccountId);
        }

        [Fact]
        public async Task AddInAccountGroup_AccountAddedInAccountGroup_OkAccountAddedInAccountGroupPublished()
        {
            //Arrange
            var client = _harness.GetRequestClient<AddAccountInAccountGroupCommand>();

            //Act
            await client.GetResponse<AddAccountInAccountGroupResult>(_command);

            //Assert
            var sent = await _harness.Published.Any<AccountAddedInAccountGroup>();

            sent.Should().Be(true);
        }

        [Fact]
        public async Task DeleteInAccountGroup_AccountAccountGroup_Ok()
        {
            //Arrange
            var client = _harness.GetRequestClient<DeleteAccountInAccountGroupCommand>();
            var consumerHarness = _harness.GetConsumerHarness<DeleteAccountInAccountGroupConsumer>();
            //Act
            var response = await client.GetResponse<DeleteAccountInAccountGroupResult>(_commandDel);

            //Assert
            var sent = await _harness.Sent.Any<DeleteAccountInAccountGroupResult>();
            var consumed = await _harness.Consumed.Any<DeleteAccountInAccountGroupCommand>();
            var consumerConsumed = await consumerHarness.Consumed.Any<DeleteAccountInAccountGroupCommand>();

            sent.Should().Be(true);
            consumed.Should().Be(true);
            consumerConsumed.Should().Be(true);
            response.Message.Should()
                .BeOfType<DeleteAccountInAccountGroupResult>()
                .And.Match<DeleteAccountInAccountGroupResult>(a => a.AccountId == _commandDel.AccountId);
        }

        [Fact]
        public async Task DeleteInAccountGroup_AccountDeletedInAccountGroup_OkAccountDeletedFromAccountGroupPublished()
        {
            //Arrange
            var client = _harness.GetRequestClient<DeleteAccountInAccountGroupCommand>();

            //Act
            await client.GetResponse<DeleteAccountInAccountGroupResult>(_commandDel);

            //Assert
            var sent = await _harness.Published.Any<AccountDeletedInAccountGroup>();

            sent.Should().Be(true);
        }


        public async Task DisposeAsync()
        {
            await _harness.Stop();
        }
    }
}


