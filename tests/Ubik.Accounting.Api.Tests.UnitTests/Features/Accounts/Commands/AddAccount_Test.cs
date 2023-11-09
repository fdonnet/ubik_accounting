using FluentAssertions;
using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Ubik.Accounting.Api.Features;
using Ubik.Accounting.Api.Features.Accounts.Commands;
using Ubik.Accounting.Api.Features.Accounts.Mappers;
using Ubik.Accounting.Api.Models;
using Ubik.Accounting.Contracts.Accounts.Commands;
using Ubik.Accounting.Contracts.Accounts.Events;
using Ubik.Accounting.Contracts.Accounts.Results;
using Ubik.ApiService.DB.Enums;

namespace Ubik.Accounting.Api.Tests.UnitTests.Features.Accounts.Commands
{
    public class AddAccount_Test : IAsyncLifetime
    {
        private readonly IServiceManager _serviceManager;
        private readonly AddAccountCommand _command;
        private readonly Account _account;
        private ITestHarness _harness = default!;
        private IServiceProvider _provider = default!;

        public AddAccount_Test()
        {
            _serviceManager = Substitute.For<IServiceManager>();

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
            _serviceManager.AccountService.AddAsync(Arg.Any<Account>()).Returns(_account); ;
            _serviceManager.AccountService.IfExistsAsync(_command.Code).Returns(false);
            _serviceManager.AccountService.IfExistsCurrencyAsync(_command.CurrencyId).Returns(true);
        }

        public async Task InitializeAsync()
        {
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
            response.Message.Should()
                .BeOfType<AddAccountResult>()
                .And.Match<AddAccountResult>(a=>a.Code == _command.Code);
        }

        [Fact]
        public async Task Add_Account_OkAccountAddedPublished()
        {
            //Arrange
            var client = _harness.GetRequestClient<AddAccountCommand>();

            //Act
            await client.GetResponse<AddAccountResult>(_command);

            //Assert
            var sent = await _harness.Published.Any<AccountAdded>();

            sent.Should().Be(true);
        }

        public async Task DisposeAsync()
        {
            await _harness.Stop();
        }
    }
}
