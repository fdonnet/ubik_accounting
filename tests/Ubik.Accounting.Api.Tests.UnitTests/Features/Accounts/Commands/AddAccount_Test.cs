using Bogus;
using FluentAssertions;
using NSubstitute;
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
        private readonly AddAccountCommand command;
        private readonly Account _account;
        private readonly ValidationPipelineBehavior<AddAccountCommand, AddAccountResult> _validationBehavior;

        public AddAccount_Test()
        {
            _serviceManager = Substitute.For<IServiceManager>();
            _handler = new AddAccountHandler(_serviceManager);

            command = new AddAccountCommand()
            {
                Code = "78888",
                Label = "Test",
                Description = "Test",
                AccountGroupId = Guid.NewGuid()
            };

            _account = command.ToAccount();
            _validationBehavior = new ValidationPipelineBehavior<AddAccountCommand, AddAccountResult>(new AddAccountValidator());
            _serviceManager.AccountService.AddAsync(_account).Returns(_account);
        }

        [Fact]
        public async Task Add_Account_Ok()
        {
            //Arrange
            _serviceManager.AccountService.IfExistsAsync(_account.Code).Returns(false);

            //Act
            var result = await _handler.Handle(command, CancellationToken.None);

            //Assert
            result.Should()
                    .NotBeNull()
                    .And.BeOfType<AddAccountResult>();
        }

        [Fact]
        public async Task Add_AccountAlreadyExistsException_AccountCodeAlreadyExists()
        {
            //Arrange
            _serviceManager.AccountService.IfExistsAsync(_account.Code).Returns(true);

            //Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            //Assert
            await act.Should().ThrowAsync<AccountAlreadyExistsException>()
                .Where(e => e.ErrorType == ServiceAndFeatureExceptionType.Conflict);
        }

        [Fact]
        public async Task Add_CustomValidationException_EmptyValuesInFields()
        {
            //Arrange
            _serviceManager.AccountService.IfExistsAsync(_account.Code).Returns(false);
            command.AccountGroupId = default!;
            command.Code = "";
            command.Label = "";

            //Act
            Func<Task> act = async () => await _validationBehavior.Handle(command, () =>
            {
                return _handler.Handle(command, CancellationToken.None);
            }, CancellationToken.None);

            //Assert
            await act.Should().ThrowAsync<CustomValidationException>()
                .Where(e => e.ErrorType == ServiceAndFeatureExceptionType.BadParams
                    && e.CustomErrors.Count() == 3);
        }

        [Fact]
        public async Task Add_CustomValidationException_TooLongValuesInFields()
        {
            //Arrange
            _serviceManager.AccountService.IfExistsAsync(_account.Code).Returns(false);

            command.Code = new string(new Faker("fr_CH").Random.Chars(count: 21));
            command.Label = new string(new Faker("fr_CH").Random.Chars(count: 101));
            command.Description = new string(new Faker("fr_CH").Random.Chars(count: 701));

            //Act
            Func<Task> act = async () => await _validationBehavior.Handle(command, () =>
            {
                return _handler.Handle(command, CancellationToken.None);
            }, CancellationToken.None);

            //Assert
            await act.Should().ThrowAsync<CustomValidationException>()
                .Where(e => e.ErrorType == ServiceAndFeatureExceptionType.BadParams
                    && e.CustomErrors.Count() == 3);
        }

    }
}
