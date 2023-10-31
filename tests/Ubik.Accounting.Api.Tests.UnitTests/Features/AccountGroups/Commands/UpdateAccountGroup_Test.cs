using Ubik.Accounting.Api.Features;
using Ubik.Accounting.Api.Models;
using NSubstitute;
using Ubik.Accounting.Api.Features.AccountGroups.Mappers;
using FluentAssertions;
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
                AccountGroupClassificationId = Guid.NewGuid(),
                Version = Guid.NewGuid()
            };

            _accountGroup = new AccountGroup() { Code = "1800", Label = "1000" };
            _accountGroup = _command.ToAccountGroup(_accountGroup);

            _serviceManager.AccountGroupService.Update(_accountGroup).Returns(_accountGroup);
            _serviceManager.AccountGroupService
                .IfExistsWithDifferentIdAsync(_command.Code, _command.AccountGroupClassificationId, _command.Id).Returns(false);

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
            _serviceManager.AccountGroupService
                .IfExistsWithDifferentIdAsync(_command.Code, _command.AccountGroupClassificationId, _command.Id).Returns(true);


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
    }
}
