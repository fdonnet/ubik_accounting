using FluentAssertions;
using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Ubik.Accounting.Api.Features;
using Ubik.Accounting.Api.Features.AccountGroups.Commands;
using Ubik.Accounting.Api.Features.AccountGroups.Mappers;
using Ubik.Accounting.Api.Models;
using Ubik.Accounting.Contracts.AccountGroups.Commands;
using Ubik.Accounting.Contracts.AccountGroups.Events;
using Ubik.Accounting.Contracts.AccountGroups.Results;
using Ubik.ApiService.Common.Exceptions;

namespace Ubik.Accounting.Api.Tests.UnitTests.Features.AccountGroups.Commands
{
    public class AddAccountGroup_Test : IAsyncLifetime
    {
        private readonly IServiceManager _serviceManager;
        private readonly AddAccountGroupCommand _command;
        private readonly AccountGroup _accountGroup;
        private ITestHarness _harness = default!;
        private IServiceProvider _provider = default!;

        public AddAccountGroup_Test()
        {
            _serviceManager = Substitute.For<IServiceManager>();

            _command = new AddAccountGroupCommand()
            {
                Code = "78888",
                Label = "Test",
                Description = "Test",
                ParentAccountGroupId = null,
                AccountGroupClassificationId = Guid.NewGuid(),
            };

            _accountGroup = _command.ToAccountGroup();
            _serviceManager.AccountGroupService.AddAsync(Arg.Any<AccountGroup>()).Returns(_accountGroup);
            _serviceManager.AccountGroupService.IfExistsAsync(_command.Code,_command.AccountGroupClassificationId).Returns(false);
        }

        public async Task InitializeAsync()
        {
            _provider = new ServiceCollection()
                .AddMassTransitTestHarness(x =>
                {
                    x.AddScoped<IServiceManager>(sm => _serviceManager);
                    x.AddConsumer<AddAccountGroupConsumer>();

                }).BuildServiceProvider(true);

            _harness = _provider.GetRequiredService<ITestHarness>();
            await _harness.Start();
        }

        [Fact]
        public async Task Add_AccountGroup_Ok()
        {
            //Arrange
            var client = _harness.GetRequestClient<AddAccountGroupCommand>();
            var consumerHarness = _harness.GetConsumerHarness<AddAccountGroupConsumer>();
            //Act
            var response = await client.GetResponse<AddAccountGroupResult>(_command);

            //Assert
            var sent = await _harness.Sent.Any<AddAccountGroupResult>();
            var consumed = await _harness.Consumed.Any<AddAccountGroupCommand>();
            var consumerConsumed = await consumerHarness.Consumed.Any<AddAccountGroupCommand>();

            sent.Should().Be(true);
            consumed.Should().Be(true);
            consumerConsumed.Should().Be(true);
            response.Message.Should()
                .BeOfType<AddAccountGroupResult>()
                .And.Match<AddAccountGroupResult>(a => a.Code == _command.Code);
        }
        [Fact]
        public async Task Add_AccountGroup_OkAccountGroupAddedPublished()
        {
            //Arrange
            var client = _harness.GetRequestClient<AddAccountGroupCommand>();

            //Act
            await client.GetResponse<AddAccountGroupResult>(_command);

            //Assert
            var sent = await _harness.Published.Any<AccountGroupAdded>();

            sent.Should().Be(true);
        }

        public async Task DisposeAsync()
        {
            await _harness.Stop();
        }
    }
}
