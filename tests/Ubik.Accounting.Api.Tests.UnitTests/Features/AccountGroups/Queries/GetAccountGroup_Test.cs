using Ubik.Accounting.Api.Features;
using Ubik.Accounting.Api.Models;
using Ubik.ApiService.Common.Validators;
using static Ubik.Accounting.Api.Features.AccountGroups.Queries.GetAccountGroup;
using NSubstitute;
using Ubik.Accounting.Api.Features.AccountGroups.Queries;
using FluentAssertions;
using Ubik.ApiService.Common.Exceptions;
using Ubik.Accounting.Api.Features.AccountGroups.Exceptions;

namespace Ubik.Accounting.Api.Tests.UnitTests.Features.AccountGroups.Queries
{
    public class GetAccountGroup_Test
    {
        private readonly IServiceManager _serviceManager;
        private readonly GetAccountGroupHandler _handler;
        private readonly GetAccountGroupQuery _query;
        private readonly AccountGroup _accountGroup;
        private readonly ValidationPipelineBehavior<GetAccountGroupQuery, GetAccountGroupResult> _validationBehavior;

        public GetAccountGroup_Test()
        {
            _serviceManager = Substitute.For<IServiceManager>();
            _handler = new GetAccountGroupHandler(_serviceManager);

            _query = new GetAccountGroupQuery()
            {
                Id = Guid.NewGuid()
            };

            _accountGroup = new AccountGroup() { Code = "TEST", Label = "Test" };

            _validationBehavior = new ValidationPipelineBehavior<GetAccountGroupQuery, GetAccountGroupResult>(new GetAccountGroupValidator());
        }

        [Fact]
        public async Task Get_AccountGroup_Ok()
        {
            //Arrange
            _serviceManager.AccountGroupService.GetAsync(_query.Id).Returns(_accountGroup);

            //Act
            var result = await _handler.Handle(_query, CancellationToken.None);

            //Assert
            result.Should()
                    .NotBeNull()
                    .And.BeOfType<GetAccountGroupResult>();
        }

        [Fact]
        public async Task Get_AccountGroupNotFoundException_AccountGroupIdNotFound()
        {
            //Arrange
            _serviceManager.AccountGroupService.GetAsync(_query.Id).Returns(Task.FromResult<AccountGroup?>(null));

            //Act
            Func<Task> act = async () => await _handler.Handle(_query, CancellationToken.None);

            //Assert
            await act.Should().ThrowAsync<AccountGroupNotFoundException>()
                .Where(e => e.ErrorType == ServiceAndFeatureExceptionType.NotFound);
        }
    }
}
