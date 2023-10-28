using FluentAssertions;
using NSubstitute;
using Ubik.Accounting.Api.Features;
using Ubik.Accounting.Api.Features.AccountGroups.Exceptions;
using Ubik.Accounting.Api.Models;
using Ubik.ApiService.Common.Exceptions;
using static Ubik.Accounting.Api.Features.AccountGroups.Commands.DeleteAccountGroup;

namespace Ubik.Accounting.Api.Tests.UnitTests.Features.AccountGroups.Commands
{
    //TODO: test child account and child account group cannot delete ;
    public class DeleteAccountGroup_Test
    {
        private readonly IServiceManager _serviceManager;
        private readonly DeleteAccountGroupHandler _handler;
        private readonly DeleteAccountGroupCommand _command;
        private readonly Guid _idToDelete;

        public DeleteAccountGroup_Test()
        {
            _serviceManager = Substitute.For<IServiceManager>();
            _handler = new DeleteAccountGroupHandler(_serviceManager);
            _idToDelete = Guid.NewGuid();
            _command = new DeleteAccountGroupCommand() { Id = _idToDelete };

            _serviceManager.AccountGroupService.ExecuteDeleteAsync(_idToDelete).Returns(true);
            _serviceManager.AccountGroupService.GetAsync(_idToDelete).Returns
                (new AccountGroup() { Id = _idToDelete, Code = "test", Label = "test" });
            _serviceManager.AccountGroupService.HasAnyChildAccountGroups(_idToDelete).Returns(false);
            //_serviceManager.AccountGroupService.HasAnyChildAccounts(_idToDelete).Returns(false);
        }

        [Fact]
        public async Task Del_AccountGroup_Ok()
        {
            //Arrange

            //Act
            var result = await _handler.Handle(_command, CancellationToken.None);

            //Assert
            result.Should()
                    .BeTrue();
        }

        [Fact]
        public async Task Del_AccountGroupNotFoundException_AccountGroupIdNotFound()
        {
            //Arrange
            _serviceManager.AccountGroupService.GetAsync(_idToDelete).Returns(Task.FromResult<AccountGroup?>(null));

            //Act
            Func<Task> act = async () => await _handler.Handle(_command, CancellationToken.None);

            //Assert
            await act.Should().ThrowAsync<AccountGroupNotFoundException>()
                .Where(e => e.ErrorType == ServiceAndFeatureExceptionType.NotFound);
        }

        [Fact]
        public async Task Del_AccountGroupHasChildAccountGroupsException_AccountGroupIdIsLinkedToChildAccountGroups()
        {
            //Arrange
            _serviceManager.AccountGroupService.HasAnyChildAccountGroups(_idToDelete).Returns(true);

            //Act
            Func<Task> act = async () => await _handler.Handle(_command, CancellationToken.None);

            //Assert
            await act.Should().ThrowAsync<AccountGroupHasChildAccountGroupsException>()
                .Where(e => e.ErrorType == ServiceAndFeatureExceptionType.Conflict);
        }

        //TODO: see if we need to adapt
        //[Fact]
        //public async Task Del_AccountGroupHasChildAccountsException_AccountGroupIdIsLinkedToChildAccounts()
        //{
        //    //Arrange
        //    _serviceManager.AccountGroupService.HasAnyChildAccounts(_idToDelete).Returns(true);

        //    //Act
        //    Func<Task> act = async () => await _handler.Handle(_command, CancellationToken.None);

        //    //Assert
        //    await act.Should().ThrowAsync<AccountGroupHasChildAccountsException>()
        //        .Where(e => e.ErrorType == ServiceAndFeatureExceptionType.Conflict);
        //}
    }
}
