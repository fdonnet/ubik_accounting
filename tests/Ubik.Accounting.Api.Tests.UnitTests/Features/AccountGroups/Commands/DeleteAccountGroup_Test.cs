using FluentAssertions;
using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Ubik.Accounting.Api.Features;
using Ubik.Accounting.Api.Features.AccountGroups.Commands;
using Ubik.Accounting.Api.Models;
using Ubik.Accounting.Contracts.AccountGroups.Commands;
using Ubik.Accounting.Contracts.AccountGroups.Events;
using Ubik.Accounting.Contracts.AccountGroups.Results;
using Ubik.ApiService.Common.Exceptions;

namespace Ubik.Accounting.Api.Tests.UnitTests.Features.AccountGroups.Commands
{
    //TODO: test child account and child account group cannot delete ;
    public class DeleteAccountGroup_Test : IAsyncLifetime
    {
        private readonly IServiceManager _serviceManager;
        private readonly DeleteAccountGroupCommand _command;
        private readonly Guid _idToDelete;
        private ITestHarness _harness = default!;
        private IServiceProvider _provider = default!;

        public DeleteAccountGroup_Test()
        {
            _serviceManager = Substitute.For<IServiceManager>();
            _idToDelete = Guid.NewGuid();
            _command = new DeleteAccountGroupCommand() { Id = _idToDelete };

            _serviceManager.AccountGroupService.DeleteAsync(_idToDelete)
                .Returns(new ResultT<bool> { Result = true, IsSuccess = true });

            var accountGroup = new AccountGroup() { Id = _idToDelete, Code = "test", Label = "test" };
            _serviceManager.AccountGroupService.GetAsync(_idToDelete).Returns
                (new ResultT<AccountGroup> { IsSuccess = true, Result = accountGroup });

        }
        public async Task InitializeAsync()
        {
            _provider = new ServiceCollection()
                .AddMassTransitTestHarness(x =>
                {
                    x.AddScoped<IServiceManager>(sm => _serviceManager);
                    x.AddConsumer<DeleteAccountGroupConsumer>();

                }).BuildServiceProvider(true);

            _harness = _provider.GetRequiredService<ITestHarness>();
            await _harness.Start();
        }

        [Fact]
        public async Task Del_AccountGroup_Ok()
        {
            //Arrange
            var client = _harness.GetRequestClient<DeleteAccountGroupCommand>();
            var consumerHarness = _harness.GetConsumerHarness<DeleteAccountGroupConsumer>();
            //Act
            var response = await client.GetResponse<DeleteAccountGroupResult>(_command);

            //Assert
            var sent = await _harness.Sent.Any<DeleteAccountGroupResult>();
            var consumed = await _harness.Consumed.Any<DeleteAccountGroupCommand>();
            var consumerConsumed = await consumerHarness.Consumed.Any<DeleteAccountGroupCommand>();

            sent.Should().Be(true);
            consumed.Should().Be(true);
            consumerConsumed.Should().Be(true);
            response.Message.Should()
                .BeOfType<DeleteAccountGroupResult>()
                .And.Match<DeleteAccountGroupResult>(a => a.Deleted == true);
        }

        [Fact]
        public async Task Del_AccountGroup_OkAccountGroupDeletedPublished()
        {
            //Arrange
            var client = _harness.GetRequestClient<DeleteAccountGroupCommand>();

            //Act
            await client.GetResponse<DeleteAccountGroupResult>(_command);

            //Assert
            var sent = await _harness.Published.Any<AccountGroupDeleted>();

            sent.Should().Be(true);
        }

        public async Task DisposeAsync()
        {
            await _harness.Stop();
        }
    }
}
