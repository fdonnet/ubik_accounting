using Bogus;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Ubik.Accounting.Api.Features;
using Ubik.Accounting.Api.Features.Accounts.Commands;
using Ubik.Accounting.Api.Features.Accounts.Exceptions;
using Ubik.Accounting.Api.Features.Accounts.Mappers;
using Ubik.Accounting.Api.Models;
using Ubik.ApiService.Common.Exceptions;
using Ubik.ApiService.Common.Validators;
using static Ubik.Accounting.Api.Features.Accounts.Commands.AddAccount;

namespace Ubik.Accounting.Api.Tests.UnitTests.Features.Accounts.Commands
{
    public class AddAccount_Test
    {
        private readonly IServiceManager _serviceManager;
        private readonly AddAccountHandler _handler;
        private readonly AddAccountCommand _addAccountCommand;
        private Account _account;
        private readonly ValidationPipelineBehavior<AddAccountCommand, AddAccountResult> _validationBehavior;

        public AddAccount_Test()
        {
            _serviceManager = Substitute.For<IServiceManager>();
            _handler = new AddAccountHandler(_serviceManager);

            _addAccountCommand = new AddAccountCommand()
            {
                Code = "78888",
                Label = "Test",
                Description = "Test",
                AccountGroupId = Guid.NewGuid()
            };

            _account = _addAccountCommand.ToAccount();
            _validationBehavior = new ValidationPipelineBehavior<AddAccountCommand, AddAccountResult>(new AddAccountValidator());
            _serviceManager.AccountService.AddAccountAsync(_account).Returns(_account);
        }

        [Fact]
        public async Task OkAdded_AccountResult()
        {
            //Arrange
            _serviceManager.AccountService.IfExists(_account.Code).Returns(false);

            //Act
            var result = await _handler.Handle(_addAccountCommand, CancellationToken.None);

            //Assert
            result.Should()
                    .NotBeNull()
                    .And.BeOfType<AddAccountResult>();
        }

        [Fact]
        public async Task AccountAlreadyExists_AccountAlreadyExistsException()
        {
            //Arrange
            _serviceManager.AccountService.IfExists(_account.Code).Returns(true);

            //Act
            Func<Task> act = async () => await _handler.Handle(_addAccountCommand, CancellationToken.None);

            //Assert
            await act.Should().ThrowAsync<AccountAlreadyExistsException>();
        }

        [Fact]
        public async Task EmptyValuesInFields_CustomValidationException()
        {
            //Arrange
            _serviceManager.AccountService.IfExists(_account.Code).Returns(false);
            _addAccountCommand.Code = "";
            _addAccountCommand.Label = "";

            //Act
            Func<Task> act = async () => await _validationBehavior.Handle(_addAccountCommand, () =>
            {
                return _handler.Handle(_addAccountCommand, CancellationToken.None);
            }, CancellationToken.None);

            //Assert
            await act.Should().ThrowAsync<CustomValidationException>()
                .Where(e => e.ErrorType == ServiceAndFeatureExceptionType.BadParams
                    && e.CustomErrors.Count() == 2);
        }

        [Fact]
        public async Task TooLongValuesInFields_CustomValidationException()
        {
            //Arrange
            _serviceManager.AccountService.IfExists(_account.Code).Returns(false);

            _addAccountCommand.Code = new string(new Faker("fr_CH").Random.Chars(count: 21));
            _addAccountCommand.Label = new string(new Faker("fr_CH").Random.Chars(count: 101));
            _addAccountCommand.Description = new string(new Faker("fr_CH").Random.Chars(count: 701));

            //Act
            Func<Task> act = async () => await _validationBehavior.Handle(_addAccountCommand, () =>
            {
                return _handler.Handle(_addAccountCommand, CancellationToken.None);
            }, CancellationToken.None);

            //Assert
            await act.Should().ThrowAsync<CustomValidationException>()
                .Where(e => e.ErrorType == ServiceAndFeatureExceptionType.BadParams
                    && e.CustomErrors.Count() == 3);
        }

    }
}
