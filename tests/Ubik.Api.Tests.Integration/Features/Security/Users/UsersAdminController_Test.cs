using FluentAssertions;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Ubik.ApiService.Common.Exceptions;
using Ubik.Security.Contracts.Users.Results;

namespace Ubik.Api.Tests.Integration.Features.Security.Users
{
    public class UsersAdminController_Test : BaseIntegrationTest
    {
        private readonly string _baseUrlForV1;
        private readonly HttpClient _client;

        public UsersAdminController_Test(IntegrationTestProxyFactory factory) : base(factory)
        {
            _baseUrlForV1 = "/usrmgt/admin/api/v1/users";
            _client = Factory.CreateDefaultClient();
        }

        [Fact]
        public async Task Get_User_ByEmail_WithAdminUser_OK()
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

        [Fact]
        public async Task Get_User_ByEmail_BadEmail_404()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.MegaAdmin);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.GetAsync($"{_baseUrlForV1}?email=xxxxzzz@test.com");
            var result = await response.Content.ReadFromJsonAsync<CustomProblemDetails>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            result.Should()
                .NotBeNull()
                .And.BeOfType<CustomProblemDetails>();
        }

        [Fact]
        public async Task Get_User_ByEmail_WithNotAdminUser_403()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.RW);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.GetAsync($"{_baseUrlForV1}?email=testrw@test.com");

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
            
        }

        [Fact]
        public async Task Get_User_ByEmail_WithNoAuth_401()
        {
            //Arrange

            //Act
            var response = await _client.GetAsync($"{_baseUrlForV1}?email=testrw@test.com");

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

        }

    }
}

