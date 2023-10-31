using Ubik.Accounting.Api.Features;
using Ubik.Accounting.Api.Models;
using NSubstitute;
using FluentAssertions;
using Ubik.Accounting.Api.Features.AccountGroups.Exceptions;
using Ubik.ApiService.Common.Exceptions;

namespace Ubik.Accounting.Api.Tests.UnitTests.Features.AccountGroups.Queries
{
    public class GetChildAccounts_Test
    {
        private readonly IServiceManager _serviceManager;
        private readonly GetChildAccountsHandler _handler;
        private readonly GetChildAccountsQuery _query;
        private readonly AccountGroup _accountGroup;

        public GetChildAccounts_Test()
        {
            _serviceManager = Substitute.For<IServiceManager>();
            _handler = new GetChildAccountsHandler(_serviceManager);

            _query = new GetChildAccountsQuery()
            {
                AccountGroupId = Guid.NewGuid()
            };

            _accountGroup = new AccountGroup() { Code = "TEST", Label = "Test" };

            _serviceManager.AccountGroupService.GetWithChildAccountsAsync(_query.AccountGroupId).Returns(_accountGroup);
        }

        [Fact]
        public async Task GetAll_Accounts_Ok()
        {
            //Arrange

            //Act
            var result = await _handler.Handle(_query, CancellationToken.None);

            //Assert
            result.Should()
                    .NotBeNull()
                    .And.AllBeOfType<Account>();
        }

        [Fact]
        public async Task GetAll_AccountGroupNotFoundException_AccountGroupIdNotFound()
        {
            //Arrange
            _serviceManager.AccountGroupService.GetWithChildAccountsAsync(_query.AccountGroupId).Returns(Task.FromResult<AccountGroup?>(null));

            //Act
            Func<Task> act = async () => await _handler.Handle(_query, CancellationToken.None);

            //Assert
            await act.Should().ThrowAsync<AccountGroupNotFoundException>()
                .Where(e => e.ErrorType == ServiceAndFeatureExceptionType.NotFound);
        }
    }
}
