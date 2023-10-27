using Bogus;
using FluentAssertions;
using NSubstitute;
using System.Security.Principal;
using Ubik.Accounting.Api.Features;
using Ubik.Accounting.Api.Features.AccountGroups.Commands;
using Ubik.Accounting.Api.Features.AccountGroups.Exceptions;
using Ubik.Accounting.Api.Features.AccountGroups.Mappers;
using Ubik.Accounting.Api.Features.Accounts.Exceptions;
using Ubik.Accounting.Api.Models;
using Ubik.ApiService.Common.Exceptions;
using Ubik.ApiService.Common.Validators;
using static Ubik.Accounting.Api.Features.AccountGroups.Commands.AddAccountGroup;
using static Ubik.Accounting.Api.Features.Accounts.Commands.AddAccount;

namespace Ubik.Accounting.Api.Tests.UnitTests.Features.AccountGroups.Commands
{
    public class AddAccountGroup_Test
    {
        private readonly IServiceManager _serviceManager;
        private readonly AddAccountGroupHandler _handler;
        private readonly AddAccountGroupCommand _command;
        private readonly AccountGroup _accountGroup;

        public AddAccountGroup_Test()
        {
            _serviceManager = Substitute.For<IServiceManager>();
            _handler = new AddAccountGroupHandler(_serviceManager);

            _command = new AddAccountGroupCommand()
            {
                Code = "78888",
                Label = "Test",
                Description = "Test",
                ParentAccountGroupId = null,
                AccountGroupClassificationId = Guid.NewGuid(),
            };

            _accountGroup = _command.ToAccountGroup();
            _serviceManager.AccountGroupService.AddAsync(_accountGroup).Returns(_accountGroup);
            _serviceManager.AccountGroupService.IfExistsAsync(_command.Code,_command.AccountGroupClassificationId).Returns(false);
        }

        [Fact]
        public async Task Add_AccountGroup_Ok()
        {
            //Arrange

            //Act
            var result = await _handler.Handle(_command, CancellationToken.None);

            //Assert
            result.Should()
            .NotBeNull()
                    .And.BeOfType<AddAccountGroupResult>();
        }

        [Fact]
        public async Task Add_AccountGroupAlreadyExistsException_AccountGroupCodeAlreadyExists()
        {
            //Arrange
            _serviceManager.AccountGroupService
                .IfExistsAsync(_command.Code,_command.AccountGroupClassificationId).Returns(true);

            //Act
            Func<Task> act = async () => await _handler.Handle(_command, CancellationToken.None);

            //Assert
            await act.Should().ThrowAsync<AccountGroupAlreadyExistsException>()
                .Where(e => e.ErrorType == ServiceAndFeatureExceptionType.Conflict);
        }

        [Fact]
        public async Task Add_AccountGroup_OkWithParentIdSpecified()
        {
            //Arrange
            var parentGuid = Guid.NewGuid();
            _serviceManager.AccountGroupService.IfExistsAsync(parentGuid).Returns(true);
            _command.ParentAccountGroupId = parentGuid;

            //Act
            var result = await _handler.Handle(_command, CancellationToken.None);

            //Assert
            result.Should()
            .NotBeNull()
                    .And.BeOfType<AddAccountGroupResult>();
        }

        [Fact]
        public async Task Add_AccountGroupParentNotFoundException_ParentAccountGroupIdNotFound()
        {
            //Arrange
            var parentGuid = Guid.NewGuid();
            _serviceManager.AccountGroupService.IfExistsAsync(parentGuid).Returns(false);
            _command.ParentAccountGroupId = parentGuid;

            //Act
            Func<Task> act = async () => await _handler.Handle(_command, CancellationToken.None);

            //Assert
            await act.Should().ThrowAsync<AccountGroupParentNotFoundException>()
                .Where(e => e.ErrorType == ServiceAndFeatureExceptionType.BadParams);
        }
    }
}
