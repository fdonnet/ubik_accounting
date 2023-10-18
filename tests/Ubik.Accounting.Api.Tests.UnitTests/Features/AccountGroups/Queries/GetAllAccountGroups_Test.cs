using FluentAssertions;
using NSubstitute;
using Ubik.Accounting.Api.Features;
using Ubik.Accounting.Api.Features.AccountGroups.Mappers;
using Ubik.Accounting.Api.Features.AccountGroups.Queries;
using Ubik.Accounting.Api.Models;
using Ubik.ApiService.Common.Validators;
using static Ubik.Accounting.Api.Features.AccountGroups.Queries.GetAllAccountGroups;

namespace Ubik.Accounting.Api.Tests.UnitTests.Features.AccountGroups.Queries
{

    public class GetAllAccountGroups_Test
    {
        private readonly IServiceManager _serviceManager;
        private readonly GetAllAccountGroupsHandler _handler;
        private readonly GetAllAccountGroupsQuery _query;
        private IEnumerable<AccountGroup> _accountGroups;
        private readonly ValidationPipelineBehavior<GetAllAccountGroupsQuery, IEnumerable<GetAllAccountGroupsResult>> _validationBehavior;

        public GetAllAccountGroups_Test()
        {
            _serviceManager = Substitute.For<IServiceManager>();
            _handler = new GetAllAccountGroupsHandler(_serviceManager);

            _query = new GetAllAccountGroupsQuery();

            _accountGroups = new AccountGroup[] { new AccountGroup() { Code = "TEST", Label = "Test" } };

            _validationBehavior = new ValidationPipelineBehavior<GetAllAccountGroupsQuery,
                IEnumerable<GetAllAccountGroupsResult>>(new GetAllAccountGroupsValidator());
        }

        [Fact]
        public async Task GetAll_AccountGroups_Ok()
        {
            //Arrange
            _serviceManager.AccountGroupService.GetAllAsync().Returns(_accountGroups);

            //Act
            var result = await _handler.Handle(_query, CancellationToken.None);

            //Assert
            result.Should()
                    .NotBeNull()
                    .And.BeEquivalentTo(_accountGroups.ToGetAllAccountGroupsResult())
                    .And.HaveCount(1);
        }

    }
}
