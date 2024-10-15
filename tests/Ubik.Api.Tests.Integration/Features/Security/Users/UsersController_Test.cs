using FluentAssertions;
using LanguageExt.Pipes;
using MassTransit;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Ubik.ApiService.Common.Exceptions;
using Ubik.Security.Contracts.Users.Results;

namespace Ubik.Api.Tests.Integration.Features.Security.Users
{
    public class UsersController_Test : BaseIntegrationTest
    {
        private readonly string _baseUrlForV1;
        private readonly HttpClient _client;

        public UsersController_Test(IntegrationTestProxyFactory factory) : base(factory)
        {
            _baseUrlForV1 = "/usrmgt/api/v1/users";
            _client = Factory.CreateDefaultClient();
        }


        [Fact]
        public async Task Get_User_WithRW_OK()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.RW);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.GetAsync($"{_baseUrlForV1}/c8660000-3c36-7456-580a-08dce562105f");
            var result = await response.Content.ReadFromJsonAsync<UserStandardResult>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Should()
                .NotBeNull()
                .And.BeOfType<UserStandardResult>(); ;
        }

        [Fact]
        public async Task Get_User_NotFound_404()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.RW);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.GetAsync($"{_baseUrlForV1}/{Guid.NewGuid()}");
            var result = await response.Content.ReadFromJsonAsync<CustomProblemDetails>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            result.Should()
                .NotBeNull()
                .And.BeOfType<CustomProblemDetails>()
                .And.Match<CustomProblemDetails>(x => x.Errors.First().Code == "USER_NOT_FOUND");
        }

        [Fact]
        public async Task Get_User_NotInTheTenant_404()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.RW);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.GetAsync($"{_baseUrlForV1}/e84d0000-088f-d0ad-3a6e-08dced19f063");
            var result = await response.Content.ReadFromJsonAsync<CustomProblemDetails>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            result.Should()
                .NotBeNull()
                .And.BeOfType<CustomProblemDetails>()
                .And.Match<CustomProblemDetails>(x => x.Errors.First().Code == "USER_NOT_FOUND");
        }

        [Fact]
        public async Task Get_User_WithRO_OK()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.RO);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.GetAsync($"{_baseUrlForV1}/c8660000-3c36-7456-580a-08dce562105f");
            var result = await response.Content.ReadFromJsonAsync<UserStandardResult>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Should()
                .NotBeNull()
                .And.BeOfType<UserStandardResult>(); ;
        }

        [Fact]
        public async Task Get_User_WithNoRole_403()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.NoRole);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.GetAsync($"{_baseUrlForV1}/c8660000-3c36-7456-580a-08dce562105f");
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Get_User_WithAdminRole_403()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.MegaAdmin);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.GetAsync($"{_baseUrlForV1}/c8660000-3c36-7456-580a-08dce562105f");
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }


        [Fact]
        public async Task Get_User_WithNoAuth_401()
        {
            //Arrange


            //Act
            var response = await _client.GetAsync($"{_baseUrlForV1}/c8660000-3c36-7456-580a-08dce562105f");
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }
    }
}
