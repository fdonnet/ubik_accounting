﻿using static Ubik.Accounting.Api.Features.Accounts.Commands.DeleteAccount;
using Ubik.Accounting.Api.Features;
using Ubik.Accounting.Api.Models;
using Ubik.ApiService.Common.Validators;
using NSubstitute;
using Ubik.Accounting.Api.Features.Accounts.Commands;
using FluentAssertions;
using Ubik.Accounting.Api.Features.Accounts.Exceptions;
using Ubik.ApiService.Common.Exceptions;

namespace Ubik.Accounting.Api.Tests.UnitTests.Features.Accounts.Commands
{
    public class DeleteAccount_Test
    {
        private readonly IServiceManager _serviceManager;
        private readonly DeleteAccountHandler _handler;
        private readonly DeleteAccountCommand _command;
        private readonly Guid _idToDelete;
        private readonly ValidationPipelineBehavior<DeleteAccountCommand, bool> _validationBehavior;

        public DeleteAccount_Test()
        {
            _serviceManager = Substitute.For<IServiceManager>();
            _handler = new DeleteAccountHandler(_serviceManager);
            _idToDelete = Guid.NewGuid();
            _validationBehavior = new ValidationPipelineBehavior<DeleteAccountCommand, bool>(new DeleteAccountValidator());
            _command = new DeleteAccountCommand() { Id=_idToDelete};

        }

        [Fact]
        public async Task Del_Account_Ok()
        {
            //Arrange
            _serviceManager.AccountService.DeleteAsync(_idToDelete).Returns(true);
            _serviceManager.AccountService.GetAsync(_idToDelete).Returns
                (new Account() { Id = _idToDelete, Code="test", Label="test" });

            //Act
            var result = await _handler.Handle(_command, CancellationToken.None);

            //Assert
            result.Should()
                    .BeTrue();
        }

        [Fact]
        public async Task Del_AccountNotFoundException_AccountIdNotFound()
        {
            //Arrange
            _serviceManager.AccountService.DeleteAsync(_idToDelete).Returns(true);
            _serviceManager.AccountService.GetAsync(_idToDelete).Returns(Task.FromResult<Account?>(null));

            //Act
            Func<Task> act = async () => await _handler.Handle(_command, CancellationToken.None);

            //Assert
            await act.Should().ThrowAsync<AccountNotFoundException>()
                .Where(e => e.ErrorType == ServiceAndFeatureExceptionType.NotFound);
        }

        [Fact]
        public async Task Del_CustomValidationException_IdRequired()
        {
            //Arrange
            _serviceManager.AccountService.DeleteAsync(_idToDelete).Returns(true);
            _serviceManager.AccountService.GetAsync(_idToDelete).Returns(new Account() { Id = _idToDelete, Code = "test", Label = "test" });
            _command.Id = default!;

            //Act
            Func<Task> act = async () => await _validationBehavior.Handle(_command, () =>
            {
                return _handler.Handle(_command, CancellationToken.None);
            }, CancellationToken.None);

            //Assert (3 errors because version is not specified)
            await act.Should().ThrowAsync<CustomValidationException>()
                .Where(e => e.ErrorType == ServiceAndFeatureExceptionType.BadParams
                    && e.CustomErrors.Count() == 1);
        }


    }
}