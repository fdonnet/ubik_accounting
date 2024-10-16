using FluentAssertions;
using MassTransit;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Ubik.ApiService.Common.Exceptions;
using Ubik.Security.Contracts.Authorizations.Commands;
using Ubik.Security.Contracts.Authorizations.Results;

namespace Ubik.Api.Tests.Integration.Features.Security.Authorizations
{
    public class AuthorizationsAdminController_Test : BaseIntegrationTest
    {
        private readonly string _baseUrlForV1;
        private readonly HttpClient _client;
        private readonly static Guid authorizationId = new("60260000-3c36-7456-5cf4-08dce60fd766");  //security_user_write
        private readonly static Guid authorizationIdToDel = new("f4350000-088f-d0ad-d8b0-08dcedb170e1");

        public AuthorizationsAdminController_Test(IntegrationTestProxyFactory factory) : base(factory)
        {
            _baseUrlForV1 = "/usrmgt/admin/api/v1/authorizations";
            _client = Factory.CreateDefaultClient();
        }

        //Get all Authorization test
        [Fact]
        public async Task Get_Authorizations_All_WithAdminUser_OK()
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
        public async Task Get_Authorizations_All_WithNotAdminUser_403()
        {
            var token = await GetAccessTokenAsync(TokenType.RW);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.GetAsync(_baseUrlForV1);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Get_Authorizations_All_WithNoAuth_401()
        {
            //Act
            var response = await _client.GetAsync(_baseUrlForV1);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        //Get Authorization by id test
        [Fact]
        public async Task Get_Authorization_By_Id_WithAdminUser_OK()
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
        public async Task Get_Authorization_By_Id_BadId_404()
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
        public async Task Get_Authorization_By_Id_WithNotAdminUser_403()
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
        public async Task Get_Authorization_By_Id_WithNoAuth_401()
        {
            //Act
            var response = await _client.GetAsync($"{_baseUrlForV1}/{authorizationId}");

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Add_Authorization_WithAdminUser_OK()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.MegaAdmin);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var command = new AddAuthorizationCommand
            {
                Code = "TestCode",
                Description = "TestDescription",
                Label = "TestLabel"
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
        public async Task Add_Authorization_WithNotAdminUser_403()
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
        public async Task Add_Authorization_WithNoAuth_401()
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
        public async Task Add_Authorization_AlreadyExists_409()
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

        [Fact]
        public async Task Update_Authorization_WithAdminUser_OK()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.MegaAdmin);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var command = new UpdateAuthorizationCommand
            {
                Id = authorizationId,
                Code = "security_user_write",
                Description = "Test",
                Label = "TestLabel",
                Version = new Guid("60260000-3c36-7456-1ee5-08dce60fd76d")
            };

            //Act
            var response = await _client.PutAsJsonAsync($"{_baseUrlForV1}/{authorizationId}", command);
            var result = await response.Content.ReadFromJsonAsync<AuthorizationStandardResult>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Should()
                .NotBeNull()
                .And.BeOfType<AuthorizationStandardResult>()
                .And.Match<AuthorizationStandardResult>(x => x.Description == "Test");
        }

        [Fact]
        public async Task Update_Authorization_WithNotAdminUser_403()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.RW);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var command = new UpdateAuthorizationCommand
            {
                Id = authorizationId,
                Code = "security_user_read",
                Description = "Test",
                Label = "TestLabel",
                Version = authorizationId
            };

            //Act
            var response = await _client.PutAsJsonAsync($"{_baseUrlForV1}/{authorizationId}", command);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Update_Authorization_WithNoAuth_401()
        {
            var command = new UpdateAuthorizationCommand
            {
                Id = authorizationId,
                Code = "security_user_read",
                Description = "Test",
                Label = "TestLabel",
                Version = authorizationId
            };

            //Act
            var response = await _client.PutAsJsonAsync($"{_baseUrlForV1}/{authorizationId}", command);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Update_Authorization_BadId_404()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.MegaAdmin);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var id = Guid.NewGuid();
            var command = new UpdateAuthorizationCommand
            {
                Id = id,
                Code = "test",
                Description = "Test",
                Label = "TestLabel",
                Version = authorizationId
            };

            //Act
            var response = await _client.PutAsJsonAsync($"{_baseUrlForV1}/{id}", command);
            var result = await response.Content.ReadFromJsonAsync<CustomProblemDetails>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            result.Should()
                .NotBeNull()
                .And.BeOfType<CustomProblemDetails>()
                .And.Match<CustomProblemDetails>(x => x.Errors.First().Code == "AUTHORIZATION_NOT_FOUND");
        }

        [Fact]
        public async Task Update_Authorization_NotMatchId_400()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.MegaAdmin);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var command = new UpdateAuthorizationCommand
            {
                Id = authorizationId,
                Code = "test",
                Description = "Test",
                Label = "TestLabel",
                Version = authorizationId
            };

            //Act
            var response = await _client.PutAsJsonAsync($"{_baseUrlForV1}/{Guid.NewGuid()}", command);
            var result = await response.Content.ReadFromJsonAsync<CustomProblemDetails>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            result.Should()
                .NotBeNull()
                .And.BeOfType<CustomProblemDetails>()
                .And.Match<CustomProblemDetails>(x => x.Errors.First().Code == "AUTHORIZATION_UPDATE_IDS_NOT_MATCH");
        }

        [Fact]
        public async Task Update_Authorization_AlreadyExistsWithOtherId_409()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.MegaAdmin);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var command = new UpdateAuthorizationCommand
            {
                Id = authorizationId,
                Code = "security_user_read",
                Description = "Test",
                Label = "TestLabel",
                Version = authorizationId
            };

            //Act
            var response = await _client.PutAsJsonAsync($"{_baseUrlForV1}/{authorizationId}", command);
            var result = await response.Content.ReadFromJsonAsync<CustomProblemDetails>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Conflict);
            result.Should()
                .NotBeNull()
                .And.BeOfType<CustomProblemDetails>()
                .And.Match<CustomProblemDetails>(x => x.Errors.First().Code == "AUTHORIZATION_ALREADY_EXISTS");
        }

        [Fact]
        public async Task Update_Authorization_WithBadVersion_409()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.MegaAdmin);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var command = new UpdateAuthorizationCommand
            {
                Id = authorizationId,
                Code = "security_user_write",
                Description = "Test",
                Label = "TestLabel",
                Version = NewId.NextGuid()
            };

            //Act
            var response = await _client.PutAsJsonAsync($"{_baseUrlForV1}/{authorizationId}", command);
            var result = await response.Content.ReadFromJsonAsync<CustomProblemDetails>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Conflict);
            result.Should()
                .NotBeNull()
                .And.BeOfType<CustomProblemDetails>()
                .And.Match<CustomProblemDetails>(x => x.Errors.First().Code == "AUTHORIZATION_UPDATE_CONCURRENCY");
        }

        [Fact]
        public async Task Delete_Authorization_WithAdminUser_OK()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.MegaAdmin);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.DeleteAsync($"{_baseUrlForV1}/{authorizationIdToDel}");

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task Delete_Authorization_WithNotAdminUser_403()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.RW);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.DeleteAsync($"{_baseUrlForV1}/{authorizationIdToDel}");

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Delete_Authorization_WithNoAuth_401()
        {
            //Act
            var response = await _client.DeleteAsync($"{_baseUrlForV1}/{authorizationIdToDel}");

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Delete_Authorization_BadId_404()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.MegaAdmin);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.DeleteAsync($"{_baseUrlForV1}/{Guid.NewGuid()}");
            var result = await response.Content.ReadFromJsonAsync<CustomProblemDetails>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            result.Should()
                .NotBeNull()
                .And.BeOfType<CustomProblemDetails>()
                .And.Match<CustomProblemDetails>(x => x.Errors.First().Code == "AUTHORIZATION_NOT_FOUND");

        }
    }
}
