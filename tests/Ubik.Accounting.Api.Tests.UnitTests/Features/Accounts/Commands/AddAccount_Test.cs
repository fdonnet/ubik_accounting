using Bogus;
using FluentAssertions;
using MassTransit;
using NSubstitute;
using Ubik.Accounting.Api.Features;
using Ubik.Accounting.Api.Features.Accounts.Commands;
using Ubik.Accounting.Api.Features.Accounts.Exceptions;
using Ubik.Accounting.Api.Features.Accounts.Mappers;
using Ubik.Accounting.Api.Models;
using Ubik.ApiService.Common.Exceptions;
using Ubik.ApiService.Common.Validators;
using Ubik.ApiService.DB.Enums;
using static Ubik.Accounting.Api.Features.Accounts.Commands.AddAccount;

namespace Ubik.Accounting.Api.Tests.UnitTests.Features.Accounts.Commands
{
    public class AddAccount_Test
    {
        private readonly IServiceManager _serviceManager;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly AddAccountHandler _handler;
        private readonly AddAccountCommand _command;
        private readonly Account _account;

        public AddAccount_Test()
        {
            _serviceManager = Substitute.For<IServiceManager>();
            _publishEndpoint = Substitute.For<IPublishEndpoint>();
            _handler = new AddAccountHandler(_serviceManager,_publishEndpoint);

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
            //_serviceManager.AccountService.IfExistsAccountGroupAsync((Guid)_command.AccountGroupId!).Returns(true);
        }

        [Fact]
        public async Task Add_Account_Ok()
        {
            //Arrange

            //Act
            var result = await _handler.Handle(_command, CancellationToken.None);

            //Assert
            result.Should()
                    .NotBeNull()
                    .And.BeOfType<AddAccountResult>();
        }

        [Fact]
        public async Task Add_AccountAlreadyExistsException_AccountCodeAlreadyExists()
        {
            //Arrange
            _serviceManager.AccountService.IfExistsAsync(_command.Code).Returns(true);

            //Act
            Func<Task> act = async () => await _handler.Handle(_command, CancellationToken.None);

            //Assert
            await act.Should().ThrowAsync<AccountAlreadyExistsException>()
                .Where(e => e.ErrorType == ServiceAndFeatureExceptionType.Conflict);
        }
    }
}
