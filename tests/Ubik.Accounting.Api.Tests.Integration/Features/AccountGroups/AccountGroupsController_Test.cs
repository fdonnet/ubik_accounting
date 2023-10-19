using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Ubik.Accounting.Api.Data.Init;
using static Ubik.Accounting.Api.Features.Accounts.Queries.GetAllAccounts;
using Ubik.Accounting.Api.Tests.Integration.Auth;
using static Ubik.Accounting.Api.Features.AccountGroups.Queries.GetAllAccountGroups;

namespace Ubik.Accounting.Api.Tests.Integration.Features.AccountGroups
{
    public class AccountGroupsController_Test : BaseIntegrationTest
    {
        private readonly BaseValuesForAccounts _testValuesForAccounts;
        private readonly BaseValuesForAccountGroups _testValuesForAccountGroups;

        public AccountGroupsController_Test(IntegrationTestWebAppFactory factory) : base(factory)
        {
            _testValuesForAccounts = new BaseValuesForAccounts();
            _testValuesForAccountGroups = new BaseValuesForAccountGroups();
        }

        [Fact]
        public async Task GetAll_AccountGroups_Ok()
        {
            //Arrange
            var httpClient = Factory.CreateDefaultClient();

            var accessToken = await AuthHelper.GetAccessTokenReadOnly();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            //Act
            var response = await httpClient.GetAsync("/AccountGroups");
            var result = await response.Content.ReadFromJsonAsync<IEnumerable<GetAllAccountGroupsResult>>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Should()
                .NotBeNull()
                .And.AllBeOfType<GetAllAccountGroupsResult>(); ;
        }
    }
}
