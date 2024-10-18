using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Ubik.Security.Contracts.Roles.Results;
using Ubik.Accounting.Contracts.AccountGroups.Results;

namespace Ubik.Api.Tests.Integration.Features.Accounting.AccountGroups
{
    public class AccountGroupsController_Test : BaseIntegrationTestAccounting
    {
        private readonly string _baseUrlForV1;
        private readonly HttpClient _client;
        private readonly static Guid _accountGroupId = new("141a0000-3c36-7456-b223-08dce6346ddc");  //usrmgt_all_rw
        private readonly static Guid _accountGroupToDel = new("b8650000-088f-d0ad-2726-08dcedbd2375");

        public AccountGroupsController_Test(IntegrationTestProxyFactory factory) : base(factory)
        {
            _baseUrlForV1 = "/accounting/api/v1/accountgroups";
            _client = Factory.CreateDefaultClient();
        }

        [Fact]
        public async Task Get_AccountGroups_All_WithRW_OK()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.RW);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.GetAsync(_baseUrlForV1);
            var result = await response.Content.ReadFromJsonAsync<List<AccountGroupStandardResult>>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Should()
                .NotBeNull()
                .And.BeOfType<List<AccountGroupStandardResult>>();
        }

        [Fact]
        public async Task Get_AccountGroups_All_WithOtherTenant_OK()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.OtherTenant);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.GetAsync(_baseUrlForV1);
            var result = await response.Content.ReadFromJsonAsync<List<AccountGroupStandardResult>>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Should()
                .NotBeNull()
                .And.BeOfType<List<AccountGroupStandardResult>>()
                .And.Match<List<AccountGroupStandardResult>>(x => x.Count == 0);
        }

        [Fact]
        public async Task Get_AccountGroups_All_WithRO_OK()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.RO);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.GetAsync(_baseUrlForV1);
            var result = await response.Content.ReadFromJsonAsync<List<AccountGroupStandardResult>>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Should()
                .NotBeNull()
                .And.BeOfType<List<AccountGroupStandardResult>>();
        }

        [Fact]
        public async Task Get_AccountGroups_All_WithNoAuth_401()
        {
            //Act
            var response = await _client.GetAsync(_baseUrlForV1);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Get_AccountGroups_All_WithNoRole_403()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.NoRole);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.GetAsync(_baseUrlForV1);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }
    }
}
