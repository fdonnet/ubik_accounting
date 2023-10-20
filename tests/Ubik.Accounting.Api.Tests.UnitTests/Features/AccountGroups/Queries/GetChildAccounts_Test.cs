using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ubik.Accounting.Api.Features;
using Ubik.Accounting.Api.Models;
using Ubik.ApiService.Common.Validators;
using static Ubik.Accounting.Api.Features.AccountGroups.Queries.GetChildAccounts;
using NSubstitute;
using Ubik.Accounting.Api.Features.AccountGroups.Queries;
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
        private readonly ValidationPipelineBehavior<GetChildAccountsQuery, IEnumerable<GetChildAccountsResult>> _validationBehavior;

        public GetChildAccounts_Test()
        {
            _serviceManager = Substitute.For<IServiceManager>();
            _handler = new GetChildAccountsHandler(_serviceManager);

            _query = new GetChildAccountsQuery()
            {
                AccountGroupId = Guid.NewGuid()
            };

            _accountGroup = new AccountGroup() { Code = "TEST", Label = "Test" };

            _validationBehavior = new ValidationPipelineBehavior<GetChildAccountsQuery, IEnumerable<GetChildAccountsResult>>(new GetChildAccountsValidator());
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

        [Fact]
        public async Task Get_CustomValidationException_EmptyValuesInFields()
        {
            //Arrange
            _query.AccountGroupId = default;

            //Act
            Func<Task> act = async () => await _validationBehavior.Handle(_query, () =>
            {
                return _handler.Handle(_query, CancellationToken.None);
            }, CancellationToken.None);

            //Assert
            await act.Should().ThrowAsync<CustomValidationException>()
                .Where(e => e.ErrorType == ServiceAndFeatureExceptionType.BadParams
                    && e.CustomErrors.Count() == 1);
        }
    }
}
