using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Ubik.ApiService.Common.Exceptions;
using Ubik.Security.Contracts.Authorizations.Commands;
using Ubik.Security.Contracts.Authorizations.Results;
using Ubik.Security.Contracts.Users.Results;

namespace Ubik.Api.Tests.Integration.Features.Security.Authorizations
{
    public class AuthorizationsAdminController_Test : BaseIntegrationTest
    {
        private readonly string _baseUrlForV1;
        private readonly HttpClient _client;
        private readonly static Guid authorizationId = new("60260000-3c36-7456-5cf4-08dce60fd766");  

        public AuthorizationsAdminController_Test(IntegrationTestProxyFactory factory) : base(factory)
        {
            _baseUrlForV1 = "/usrmgt/admin/api/v1/authorizations";
            _client = Factory.CreateDefaultClient();
        }

        //Get all auhtorization test
        [Fact]
        public async Task Get_Auhtorizations_All_WithAdminUser_OK()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.MegaAdmin);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.GetAsync(_baseUrlForV1);
            var result = await response.Content.ReadFromJsonAsync<List<AuthorizationStandardResult>>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Should()
                .NotBeNull()
                .And.BeOfType<List<AuthorizationStandardResult>>(); ;
        }

        [Fact]
        public async Task Get_Auhtorizations_All_WithNotAdminUser_403()
        {
            var token = await GetAccessTokenAsync(TokenType.RW);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.GetAsync(_baseUrlForV1);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Get_Auhtorizations_All_WithNoAuth_401()
        {
            //Act
            var response = await _client.GetAsync(_baseUrlForV1);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        //Get auhtorization by id test
        [Fact]
        public async Task Get_Auhtorization_By_Id_WithAdminUser_OK()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.MegaAdmin);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.GetAsync($"{_baseUrlForV1}/{authorizationId}");
            var result = await response.Content.ReadFromJsonAsync<AuthorizationStandardResult>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Should()
                .NotBeNull()
                .And.BeOfType<AuthorizationStandardResult>(); ;
        }

        [Fact]
        public async Task Get_Auhtorization_By_Id_BadId_404()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.MegaAdmin);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.GetAsync($"{_baseUrlForV1}/{Guid.NewGuid()}");
            var result = await response.Content.ReadFromJsonAsync<CustomProblemDetails>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            result.Should()
                .NotBeNull()
                .And.BeOfType<CustomProblemDetails>()
                .And.Match<CustomProblemDetails>(x => x.Errors.First().Code == "AUTHORIZATION_NOT_FOUND");
        }

        [Fact]
        public async Task Get_Auhtorization_By_Id_WithNotAdminUser_403()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.RW);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.GetAsync($"{_baseUrlForV1}/{authorizationId}");

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Get_Auhtorization_By_Id_WithNoAuth_401()
        {
            //Act
            var response = await _client.GetAsync($"{_baseUrlForV1}/{authorizationId}");

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Post_Auhtorization_WithAdminUser_OK()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.MegaAdmin);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var command = new AddAuthorizationCommand
            {
                Code = "TestCode",
                Description = "TestDescription",
                Label= "TestLabel"
            };

            //Act
            var response = await _client.PostAsJsonAsync(_baseUrlForV1, command);
            var result = await response.Content.ReadFromJsonAsync<AuthorizationStandardResult>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            result.Should()
                .NotBeNull()
                .And.BeOfType<AuthorizationStandardResult>();

        }

        [Fact]
        public async Task Post_Auhtorization_WithNotAdminUser_403()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.RW);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var command = new AddAuthorizationCommand
            {
                Code = "TestCode",
                Description = "TestDescription",
                Label = "TestLabel"
            };

            //Act
            var response = await _client.PostAsJsonAsync(_baseUrlForV1, command);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Post_Auhtorization_WithNoAuth_401()
        {
            var command = new AddAuthorizationCommand
            {
                Code = "TestCode",
                Description = "TestDescription",
                Label = "TestLabel"
            };

            //Act
            var response = await _client.PostAsJsonAsync(_baseUrlForV1, command);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Post_Auhtorization_AlreadyExists_409()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.MegaAdmin);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var command = new AddAuthorizationCommand
            {
                Code = "security_user_read",
                Description = "TestDescription",
                Label = "TestLabel"
            };

            //Act
            var response = await _client.PostAsJsonAsync(_baseUrlForV1, command);
            var result = await response.Content.ReadFromJsonAsync<CustomProblemDetails>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Conflict);
            result.Should()
                .NotBeNull()
                .And.BeOfType<CustomProblemDetails>()
                .And.Match<CustomProblemDetails>(x => x.Errors.First().Code == "AUTHORIZATION_ALREADY_EXISTS");
        }
    }
}
