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
    public class AddAccountInAccountGroup_Test : IAsyncLifetime
    {
        private readonly IServiceManager _serviceManager;
        private readonly AddAccountInAccountGroupCommand _command;
        private ITestHarness _harness = default!;
        private IServiceProvider _provider = default!;

        public AddAccountInAccountGroup_Test()
        {
            _serviceManager = Substitute.For<IServiceManager>();

            _command = new AddAccountInAccountGroupCommand()
            {
               AccountGroupId = NewId.NextGuid(),
               AccountId = NewId.NextGuid()
            };

            _serviceManager.AccountService.AddToAccountGroupAsync(_command.AccountId,_command.AccountGroupId)
                .Returns(new AccountAccountGroup { AccountGroupId = _command.AccountGroupId,
                                                   AccountId = _command.AccountId }); ;
        }

        public async Task InitializeAsync()
        {
            _provider = new ServiceCollection()
                .AddMassTransitTestHarness(x =>
                {
                    x.AddScoped<IServiceManager>(sm => _serviceManager);
                    x.AddConsumer<AddAccountInAccountGroupConsumer>();

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


        public async Task DisposeAsync()
        {
            await _harness.Stop();
        }
    }
}


