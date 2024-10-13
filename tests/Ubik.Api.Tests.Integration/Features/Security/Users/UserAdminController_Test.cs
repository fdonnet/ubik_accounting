using Docker.DotNet;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Ubik.Accounting.Api.Data.Init;
using Ubik.Accounting.Contracts.Classifications.Results;
using Ubik.Security.Contracts.Users.Results;

namespace Ubik.Api.Tests.Integration.Features.Security.Users
{
    public class UserAdminController_Test : BaseIntegrationTest
    {
        private readonly string _baseUrlForV1;
        private readonly HttpClient _client;

        public UserAdminController_Test(IntegrationTestProxyFactory factory) : base(factory)
        {
            _baseUrlForV1 = "/usrmgt/admin/api/v1/users";
            _client = Factory.CreateDefaultClient();
        }

        [Fact]
        public async Task Get_TestRwUser_ByEmail_OK()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.MegaAdmin);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.GetAsync($"{_baseUrlForV1}?email=testrw@test.com");
            var result = await response.Content.ReadFromJsonAsync<UserAdminResult>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Should()
                .NotBeNull()
                .And.BeOfType<UserAdminResult>(); ;
        }
    }
}

