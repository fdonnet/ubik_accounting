using System;
using System.Collections.Generic;

using Ubik.Accounting.Api.Features;
using Ubik.Accounting.Api.Models;
using Ubik.ApiService.Common.Validators;
using NSubstitute;
using static Ubik.Accounting.Api.Features.AccountGroups.Commands.UpdateAccountGroup;
using Ubik.Accounting.Api.Features.AccountGroups.Mappers;
using Ubik.Accounting.Api.Features.AccountGroups.Commands;
using Bogus;
using FluentAssertions;
using static Ubik.Accounting.Api.Features.Accounts.Commands.UpdateAccount;
using System.Security.Principal;
using Ubik.Accounting.Api.Features.Accounts.Exceptions;
using Ubik.ApiService.Common.Exceptions;
using Ubik.Accounting.Api.Features.AccountGroups.Exceptions;

namespace Ubik.Accounting.Api.Tests.UnitTests.Features.AccountGroups.Commands
{
    public class UpdateAccountGroup_Test
    {
        private readonly IServiceManager _serviceManager;
        private readonly UpdateAccountGroupHandler _handler;
        private readonly UpdateAccountGroupCommand _command;
        private readonly AccountGroup _accountGroup;
        private readonly ValidationPipelineBehavior<UpdateAccountGroupCommand, UpdateAccountGroupResult> _validationBehavior;


        public UpdateAccountGroup_Test()
        {
            _serviceManager = Substitute.For<IServiceManager>();
            _handler = new UpdateAccountGroupHandler(_serviceManager);

            _command = new UpdateAccountGroupCommand()
            {
                Id = Guid.NewGuid(),
                Code = "78888",
                Label = "Test",
                Description = "Test",
                ParentAccountGroupId = Guid.NewGuid(),
                Version = Guid.NewGuid()
            };

            _accountGroup = new AccountGroup() { Code = "1800", Label = "1000" };
            _accountGroup = _command.ToAccountGroup(_accountGroup);
            _validationBehavior = new ValidationPipelineBehavior<UpdateAccountGroupCommand, UpdateAccountGroupResult>(new UpdateAccountGroupValidator());

            _serviceManager.AccountGroupService.UpdateAsync(_accountGroup).Returns(_accountGroup);
            _serviceManager.AccountGroupService.IfExistsWithDifferentIdAsync(_command.Code, _command.Id).Returns(false);
            _serviceManager.AccountGroupService.GetAsync(_command.Id).Returns(_accountGroup);
            _serviceManager.AccountGroupService.IfExistsAsync((Guid)_command.ParentAccountGroupId).Returns(true);
        }

        [Fact]
        public async Task Upd_AccountGroup_Ok()
        {
            //Arrange

            //Act
            var result = await _handler.Handle(_command, CancellationToken.None);

            //Assert
            result.Should()
                    .NotBeNull()
                    .And.BeOfType<UpdateAccountGroupResult>();
        }

        [Fact]
        public async Task Upd_AccountGroupAlreadyExistsException_AccountGroupCodeAlreadyExistsWithDifferentId()
        {
            //Arrange
            _serviceManager.AccountGroupService.IfExistsWithDifferentIdAsync(_command.Code, _command.Id).Returns(true);


            //Act
            Func<Task> act = async () => await _handler.Handle(_command, CancellationToken.None);

            //Assert
            await act.Should().ThrowAsync<AccountGroupAlreadyExistsException>()
                .Where(e => e.ErrorType == ServiceAndFeatureExceptionType.Conflict);
        }

        [Fact]
        public async Task Upd_AccountGroupNotFoundException_AccountGroupIdNotFound()
        {
            //Arrange
            _serviceManager.AccountGroupService.GetAsync(_command.Id).Returns(Task.FromResult<AccountGroup?>(null));

            //Act
            Func<Task> act = async () => await _handler.Handle(_command, CancellationToken.None);

            //Assert
            await act.Should().ThrowAsync<AccountGroupNotFoundException>()
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
