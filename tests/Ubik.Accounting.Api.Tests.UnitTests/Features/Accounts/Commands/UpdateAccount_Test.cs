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

namespace Ubik.Accounting.Api.Tests.UnitTests.Features.Accounts.Commands
{
    public class UpdateAccount_Test
    {
        private readonly IServiceManager _serviceManager;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly UpdateAccountHandler _handler;
        private readonly UpdateAccountCommand _command;
        private readonly Account _account;
        private readonly ValidationPipelineBehavior<UpdateAccountCommand, UpdateAccountResult> _validationBehavior;

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
                Version = Guid.NewGuid()
            };

            _account = new Account() { Code = "1800", Label = "1000" };
            _account = _command.ToAccount(_account);
            _validationBehavior = new ValidationPipelineBehavior<UpdateAccountCommand, UpdateAccountResult>(new UpdateAccountValidator());

            _serviceManager.AccountService.Update(_account).Returns(_account);
            _serviceManager.AccountService.IfExistsWithDifferentIdAsync(_command.Code, _command.Id).Returns(false);
            //_serviceManager.AccountService.IfExistsAccountGroupAsync((Guid)_command.AccountGroupId!).Returns(true);
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
        public async Task Upd_CustomValidationException_EmptyValuesInFields()
        {
            //Arrange
            _command.Code = "";
            _command.Label = "";

            //Act
            Func<Task> act = async () => await _validationBehavior.Handle(_command, () =>
            {
                return _handler.Handle(_command, CancellationToken.None);
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
            _command.Code = new string(new Faker("fr_CH").Random.Chars(count: 21));
            _command.Label = new string(new Faker("fr_CH").Random.Chars(count: 101));
            _command.Description = new string(new Faker("fr_CH").Random.Chars(count: 701));

            //Act
            Func<Task> act = async () => await _validationBehavior.Handle(_command, () =>
            {
                return _handler.Handle(_command, CancellationToken.None);
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
            _command.Version = default!;

            //Act
            Func<Task> act = async () => await _validationBehavior.Handle(_command, () =>
            {
                return _handler.Handle(_command, CancellationToken.None);
            }, CancellationToken.None);

            //Assert
            await act.Should().ThrowAsync<CustomValidationException>()
                .Where(e => e.ErrorType == ServiceAndFeatureExceptionType.BadParams
                    && e.CustomErrors.Count() == 1);
        }


    }
}
