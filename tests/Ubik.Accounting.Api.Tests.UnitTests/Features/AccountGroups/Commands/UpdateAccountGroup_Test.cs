using Ubik.Accounting.Api.Features;
using Ubik.Accounting.Api.Models;
using NSubstitute;
using Ubik.Accounting.Api.Features.AccountGroups.Mappers;
using FluentAssertions;
using Ubik.Accounting.Contracts.AccountGroups.Commands;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using Ubik.Accounting.Api.Features.AccountGroups.Commands;
using Ubik.Accounting.Contracts.AccountGroups.Results;
using Ubik.Accounting.Contracts.AccountGroups.Events;
using MassTransit;

namespace Ubik.Accounting.Api.Tests.UnitTests.Features.AccountGroups.Commands
{
    public class UpdateAccountGroup_Test : IAsyncLifetime
    {
        private readonly IServiceManager _serviceManager;
        private readonly UpdateAccountGroupCommand _command;
        private readonly AccountGroup _accountGroup;
        private ITestHarness _harness = default!;
        private IServiceProvider _provider = default!;

        public UpdateAccountGroup_Test()
        {
            _serviceManager = Substitute.For<IServiceManager>();

            _command = new UpdateAccountGroupCommand()
            {
                Id = Guid.NewGuid(),
                Code = "78888",
                Label = "Test",
                Description = "Test",
                ParentAccountGroupId = Guid.NewGuid(),
                AccountGroupClassificationId = Guid.NewGuid(),
                Version = Guid.NewGuid()
            };

            _accountGroup = new AccountGroup() { Code = "1800", Label = "1000" };
            _accountGroup = _command.ToAccountGroup(_accountGroup);

            _serviceManager.AccountGroupService.UpdateAsync(Arg.Any<AccountGroup>())
                .Returns(_accountGroup);

            _serviceManager.AccountGroupService
                .IfExistsWithDifferentIdAsync(_command.Code, _command.AccountGroupClassificationId, _command.Id).Returns(false);

            _serviceManager.AccountGroupService.GetAsync(_command.Id)
                .Returns(_accountGroup);

            _serviceManager.AccountGroupService.IfExistsAsync((Guid)_command.ParentAccountGroupId).Returns(true);
        }

        public async Task InitializeAsync()
        {
            _provider = new ServiceCollection()
                .AddMassTransitTestHarness(x =>
                {
                    x.AddScoped<IServiceManager>(sm => _serviceManager);
                    x.AddConsumer<UpdateAccountGroupConsumer>();

                }).BuildServiceProvider(true);

            _harness = _provider.GetRequiredService<ITestHarness>();
            await _harness.Start();
        }

        [Fact]
        public async Task Upd_AccountGroup_Ok()
        {
            //Arrange
            var client = _harness.GetRequestClient<UpdateAccountGroupCommand>();
            var consumerHarness = _harness.GetConsumerHarness<UpdateAccountGroupConsumer>();
            //Act
            var response = await client.GetResponse<UpdateAccountGroupResult>(_command);

            //Assert
            var sent = await _harness.Sent.Any<UpdateAccountGroupResult>();
            var consumed = await _harness.Consumed.Any<UpdateAccountGroupCommand>();
            var consumerConsumed = await consumerHarness.Consumed.Any<UpdateAccountGroupCommand>();

            sent.Should().Be(true);
            consumed.Should().Be(true);
            consumerConsumed.Should().Be(true);
            response.Message.Should()
                .BeOfType<UpdateAccountGroupResult>()
                .And.Match<UpdateAccountGroupResult>(a => a.Code == _command.Code);
        }

        [Fact]
        public async Task Upd_AccountGroup_OkAccountGroupUpdatedPublished()
        {
            //Arrange
            var client = _harness.GetRequestClient<UpdateAccountGroupCommand>();

            //Act
            await client.GetResponse<UpdateAccountGroupResult>(_command);

            //Assert
            var sent = await _harness.Published.Any<AccountGroupUpdated>();

            sent.Should().Be(true);
        }

        public async Task DisposeAsync()
        {
            await _harness.Stop();
        }
    }
}
