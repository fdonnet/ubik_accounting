﻿using NSubstitute;
using Ubik.Accounting.Api.Features.Accounts.Commands;
using Ubik.Accounting.Api.Features;
using Ubik.Accounting.Api.Models;
using Ubik.ApiService.Common.Validators;
using static Ubik.Accounting.Api.Features.Accounts.Commands.UpdateAccount;
using Ubik.Accounting.Api.Features.Accounts.Mappers;
using FluentAssertions;
using static Ubik.Accounting.Api.Features.Accounts.Commands.AddAccount;
using Ubik.Accounting.Api.Features.Accounts.Exceptions;
using Bogus;
using Ubik.ApiService.Common.Exceptions;

namespace Ubik.Accounting.Api.Tests.UnitTests.Features.Accounts.Commands
{
    public class UpdateAccount_Test
    {
        private readonly IServiceManager _serviceManager;
        private readonly UpdateAccountHandler _handler;
        private readonly UpdateAccountCommand _updAccountCommand;
        private Account _account;
        private readonly ValidationPipelineBehavior<UpdateAccountCommand, UpdateAccountResult> _validationBehavior;

        public UpdateAccount_Test()
        {
            _serviceManager = Substitute.For<IServiceManager>();
            _handler = new UpdateAccountHandler(_serviceManager);

            _updAccountCommand = new UpdateAccountCommand()
            {
                Id = Guid.NewGuid(),
                Code = "78888",
                Label = "Test",
                Description = "Test",
                AccountGroupId = Guid.NewGuid(),
                Version = Guid.NewGuid()
            };

            _account = new Account() { Code = "1800", Label = "1000" };
            _account = _updAccountCommand.ToAccount(_account);
            _validationBehavior = new ValidationPipelineBehavior<UpdateAccountCommand, UpdateAccountResult>(new UpdateAccountValidator());
            _serviceManager.AccountService.UpdateAccountAsync(_account).Returns(_account);
        }

        [Fact]
        public async Task Upd_Account_Ok()
        {
            //Arrange
            _serviceManager.AccountService.IfExistsWithDifferentId(_account.Code,_account.Id).Returns(false);
            _serviceManager.AccountService.GetAccountAsync(_account.Id).Returns(_account);

            //Act
            var result = await _handler.Handle(_updAccountCommand, CancellationToken.None);

            //Assert
            result.Should()
                    .NotBeNull()
                    .And.BeOfType<UpdateAccountResult>();
        }

        [Fact]
        public async Task Upd_AccountAlreadyExistsException_AccountCodeAlreadyExistsWithDifferentId()
        {
            //Arrange
            _serviceManager.AccountService.IfExistsWithDifferentId(_account.Code, _account.Id).Returns(true);
            _serviceManager.AccountService.GetAccountAsync(_account.Id).Returns(_account);

            //Act
            Func<Task> act = async () => await _handler.Handle(_updAccountCommand, CancellationToken.None);

            //Assert
            await act.Should().ThrowAsync<AccountAlreadyExistsException>();
        }

        [Fact]
        public async Task Upd_AccountNotFoundException_AccountIdNotFound()
        {
            //Arrange
            _serviceManager.AccountService.IfExistsWithDifferentId(_account.Code, _account.Id).Returns(false);
            _serviceManager.AccountService.GetAccountAsync(_account.Id).Returns(Task.FromResult<Account?>(null));

            //Act
            Func<Task> act = async () => await _handler.Handle(_updAccountCommand, CancellationToken.None);

            //Assert
            await act.Should().ThrowAsync<AccountNotFoundException>();
        }

        [Fact]
        public async Task Upd_CustomValidationException_EmptyValuesInFields()
        {
            //Arrange
            _serviceManager.AccountService.IfExistsWithDifferentId(_account.Code, _account.Id).Returns(false);
            _serviceManager.AccountService.GetAccountAsync(_account.Id).Returns(_account);
            _updAccountCommand.Code = "";
            _updAccountCommand.Label = "";

            //Act
            Func<Task> act = async () => await _validationBehavior.Handle(_updAccountCommand, () =>
            {
                return _handler.Handle(_updAccountCommand, CancellationToken.None);
            }, CancellationToken.None);

            //Assert (3 errors because version is not specified)
            await act.Should().ThrowAsync<CustomValidationException>()
                .Where(e => e.ErrorType == ServiceAndFeatureExceptionType.BadParams
                    && e.CustomErrors.Count() == 2);
        }

        [Fact]
        public async Task Upd_CustomValidationException_TooLongValuesInFields()
        {
            //Arrange
            _serviceManager.AccountService.IfExistsWithDifferentId(_account.Code, _account.Id).Returns(false);
            _serviceManager.AccountService.GetAccountAsync(_account.Id).Returns(_account);

            _updAccountCommand.Code = new string(new Faker("fr_CH").Random.Chars(count: 21));
            _updAccountCommand.Label = new string(new Faker("fr_CH").Random.Chars(count: 101));
            _updAccountCommand.Description = new string(new Faker("fr_CH").Random.Chars(count: 701));

            //Act
            Func<Task> act = async () => await _validationBehavior.Handle(_updAccountCommand, () =>
            {
                return _handler.Handle(_updAccountCommand, CancellationToken.None);
            }, CancellationToken.None);

            //Assert
            await act.Should().ThrowAsync<CustomValidationException>()
                .Where(e => e.ErrorType == ServiceAndFeatureExceptionType.BadParams
                    && e.CustomErrors.Count() == 3);
        }


        [Fact]
        public async Task Upd_CustomValidationException_VersionIdNotProvided()
        {
            //Arrange
            _serviceManager.AccountService.IfExistsWithDifferentId(_account.Code, _account.Id).Returns(false);
            _serviceManager.AccountService.GetAccountAsync(_account.Id).Returns(_account);

            _updAccountCommand.Version = default!;

            //Act
            Func<Task> act = async () => await _validationBehavior.Handle(_updAccountCommand, () =>
            {
                return _handler.Handle(_updAccountCommand, CancellationToken.None);
            }, CancellationToken.None);

            //Assert
            await act.Should().ThrowAsync<CustomValidationException>()
                .Where(e => e.ErrorType == ServiceAndFeatureExceptionType.BadParams
                    && e.CustomErrors.Count() == 1);
        }


    }
}
