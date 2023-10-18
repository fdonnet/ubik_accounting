using FluentAssertions;
using NSubstitute;
using Ubik.Accounting.Api.Features;
using Ubik.Accounting.Api.Features.AccountGroups.Commands;
using Ubik.Accounting.Api.Features.AccountGroups.Exceptions;
using Ubik.Accounting.Api.Models;
using Ubik.ApiService.Common.Exceptions;
using Ubik.ApiService.Common.Validators;
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
        private readonly ValidationPipelineBehavior<DeleteAccountGroupCommand, bool> _validationBehavior;

        public DeleteAccountGroup_Test()
        {
            _serviceManager = Substitute.For<IServiceManager>();
            _handler = new DeleteAccountGroupHandler(_serviceManager);
            _idToDelete = Guid.NewGuid();
            _validationBehavior = new ValidationPipelineBehavior<DeleteAccountGroupCommand, bool>(new DeleteAccountGroupValidator());
            _command = new DeleteAccountGroupCommand() { Id = _idToDelete };
        }

        [Fact]
        public async Task Del_AccountGroup_Ok()
        {
            //Arrange
            _serviceManager.AccountGroupService.DeleteAsync(_idToDelete).Returns(true);
            _serviceManager.AccountGroupService.GetAsync(_idToDelete).Returns
                (new AccountGroup() { Id = _idToDelete, Code = "test", Label = "test" });

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
            _serviceManager.AccountGroupService.DeleteAsync(_idToDelete).Returns(true);
            _serviceManager.AccountGroupService.GetAsync(_idToDelete).Returns(Task.FromResult<AccountGroup?>(null));

            //Act
            Func<Task> act = async () => await _handler.Handle(_command, CancellationToken.None);

            //Assert
            await act.Should().ThrowAsync<AccountGroupNotFoundException>()
                .Where(e => e.ErrorType == ServiceAndFeatureExceptionType.NotFound);
        }

        [Fact]
        public async Task Del_CustomValidationException_IdRequired()
        {
            //Arrange
            _serviceManager.AccountGroupService.DeleteAsync(_idToDelete).Returns(true);
            _serviceManager.AccountGroupService.GetAsync(_idToDelete).Returns(new AccountGroup() { Id = _idToDelete, Code = "test", Label = "test" });
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
