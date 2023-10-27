using NSubstitute;
using Ubik.Accounting.Api.Features.Accounts.Commands;
using Ubik.Accounting.Api.Features;
using Ubik.Accounting.Api.Models;
using Ubik.ApiService.Common.Validators;
using static Ubik.Accounting.Api.Features.Accounts.Commands.UpdateAccount;
using Ubik.Accounting.Api.Features.Accounts.Mappers;
using FluentAssertions;
using Ubik.Accounting.Api.Features.Accounts.Exceptions;
using Bogus;
using Ubik.ApiService.Common.Exceptions;
using MassTransit;
using System.Diagnostics;

namespace Ubik.Accounting.Api.Tests.UnitTests.Features.Accounts.Commands
{
    public class UpdateAccount_Test
    {
        private readonly IServiceManager _serviceManager;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly UpdateAccountHandler _handler;
        private readonly UpdateAccountCommand _command;
        private readonly Account _account;

        public UpdateAccount_Test()
        {
            _serviceManager = Substitute.For<IServiceManager>();
            _publishEndpoint = Substitute.For<IPublishEndpoint>();
            _handler = new UpdateAccountHandler(_serviceManager, _publishEndpoint);

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

            _serviceManager.AccountService.Update(_account).Returns(_account);
            _serviceManager.AccountService.IfExistsWithDifferentIdAsync(_command.Code, _command.Id).Returns(false);
            _serviceManager.AccountService.IfExistsCurrencyAsync(_command.CurrencyId).Returns(true);
            _serviceManager.AccountService.GetAsync(_command.Id).Returns(_account);
        }

        [Fact]
        public async Task Upd_Account_Ok()
        {
            //Arrange

            //Act
            var result = await _handler.Handle(_command, CancellationToken.None);

            //Assert
            result.Should()
                    .NotBeNull()
                    .And.BeOfType<UpdateAccountResult>();
        }

        [Fact]
        public async Task Upd_AccountAlreadyExistsException_AccountCodeAlreadyExistsWithDifferentId()
        {
            //Arrange
            _serviceManager.AccountService.IfExistsWithDifferentIdAsync(_command.Code, _command.Id).Returns(true);

            //Act
            Func<Task> act = async () => await _handler.Handle(_command, CancellationToken.None);

            //Assert
            await act.Should().ThrowAsync<AccountAlreadyExistsException>()
                .Where(e => e.ErrorType == ServiceAndFeatureExceptionType.Conflict);
        }

        [Fact]
        public async Task Upd_AccountNotFoundException_AccountIdNotFound()
        {
            //Arrange
            _serviceManager.AccountService.GetAsync(_command.Id).Returns(Task.FromResult<Account?>(null));

            //Act
            Func<Task> act = async () => await _handler.Handle(_command, CancellationToken.None);

            //Assert
            await act.Should().ThrowAsync<AccountNotFoundException>()
                .Where(e => e.ErrorType == ServiceAndFeatureExceptionType.NotFound);
        }

        [Fact]
        public async Task Upd_AccountCurrencyNotFoundException_CurrencyIdNotFound()
        {
            //Arrange
            _serviceManager.AccountService.IfExistsCurrencyAsync(_command.CurrencyId).Returns(false);

            //Act
            Func<Task> act = async () => await _handler.Handle(_command, CancellationToken.None);

            //Assert
            await act.Should().ThrowAsync<AccountCurrencyNotFoundException>()
                .Where(e => e.ErrorType == ServiceAndFeatureExceptionType.BadParams);
        }
    }
}
