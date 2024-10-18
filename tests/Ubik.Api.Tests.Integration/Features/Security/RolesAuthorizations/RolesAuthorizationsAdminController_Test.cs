using FluentAssertions;
using MassTransit;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Ubik.ApiService.Common.Exceptions;
using Ubik.Security.Contracts.RoleAuthorizations.Commands;
using Ubik.Security.Contracts.RoleAuthorizations.Events;

namespace Ubik.Api.Tests.Integration.Features.Security.RolesAuthorizations
{
    public class RolesAuthorizationsAdminController_Test : BaseIntegrationTestSecurity
    {
        private readonly string _baseUrlForV1;
        private readonly HttpClient _client;
        private readonly static Guid _roleAuthorizationId = new("3c730000-088f-d0ad-962c-08dce873f4c3");
        private readonly static Guid _roleAuthorizationIdToDel = new("04280000-088f-d0ad-15f9-08dcedd377eb");
        private readonly static Guid _roleAuthorizationIdNotABaseRole = new("04280000-088f-d0ad-048a-08dcedd38d77");
        private readonly static Guid _roleId = new("74410000-088f-d0ad-f4d5-08dcedd9c6a3");
        private readonly static Guid _authorizationId = new("d05a0000-3c36-7456-8576-08dce616ad08");

        public RolesAuthorizationsAdminController_Test(IntegrationTestProxyFactory factory) : base(factory)
        {
            _baseUrlForV1 = "/usrmgt/admin/api/v1/rolesauthorizations";
            _client = Factory.CreateDefaultClient();
        }

        [Fact]
        public async Task Get_RolesAuthorizations_All_WithAdminUser_OK()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.MegaAdmin);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Act
            var response = await _client.GetAsync(_baseUrlForV1);
            var result = await response.Content.ReadFromJsonAsync<List<RoleAuthorizationStandardResult>>();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Should()
                .NotBeNull()
                .And.BeOfType<List<RoleAuthorizationStandardResult>>();
        }

        [Fact]
        public async Task Get_RolesAuthorizations_All_WithNotAdminUser_403()
        {
            // Arrange
            var token = await GetAccessTokenAsync(TokenType.RW);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Act
            var response = await _client.GetAsync(_baseUrlForV1);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Get_RolesAuthorizations_All_WithNoAuth_401()
        {
            // Act
            var response = await _client.GetAsync(_baseUrlForV1);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Get_RoleAuthorization_By_Id_WithAdminUser_OK()
        {
            // Arrange
            var token = await GetAccessTokenAsync(TokenType.MegaAdmin);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Act
            var response = await _client.GetAsync($"{_baseUrlForV1}/{_roleAuthorizationId}");
            var result = await response.Content.ReadFromJsonAsync<RoleAuthorizationStandardResult>();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Should()
                .NotBeNull()
                .And.BeOfType<RoleAuthorizationStandardResult>();
        }

        [Fact]
        public async Task Get_RoleAuthorization_By_Id_WithNotAdminUser_403()
        {
            // Arrange
            var token = await GetAccessTokenAsync(TokenType.RW);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Act
            var response = await _client.GetAsync($"{_baseUrlForV1}/{_roleAuthorizationId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Get_RoleAuthorization_By_Id_WithNoAuth_401()
        {
            // Act
            var response = await _client.GetAsync($"{_baseUrlForV1}/{_roleAuthorizationId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Get_RoleAuthorization_By_Id_WithBadId_404()
        {
            // Arrange
            var token = await GetAccessTokenAsync(TokenType.MegaAdmin);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Act
            var response = await _client.GetAsync($"{_baseUrlForV1}/{NewId.NextGuid()}");
            var result = await response.Content.ReadFromJsonAsync<CustomProblemDetails>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            result.Should()
                .NotBeNull()
                .And.BeOfType<CustomProblemDetails>()
                .And.Match<CustomProblemDetails>(x => x.Errors.First().Code == "ROLEAUTHORIZATION_NOT_FOUND");
        }

        [Fact]
        public async Task Get_RoleAuthorization_By_Id_WithRoleNotABaseRole_404()
        {
            // Arrange
            var token = await GetAccessTokenAsync(TokenType.MegaAdmin);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Act
            var response = await _client.GetAsync($"{_baseUrlForV1}/{_roleAuthorizationIdNotABaseRole}");
            var result = await response.Content.ReadFromJsonAsync<CustomProblemDetails>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            result.Should()
                .NotBeNull()
                .And.BeOfType<CustomProblemDetails>()
                .And.Match<CustomProblemDetails>(x => x.Errors.First().Code == "ROLEAUTHORIZATION_NOT_FOUND");
        }

        [Fact]
        public async Task Add_RoleAuthorization_WithAdminUser_OK()
        {
            // Arrange
            var token = await GetAccessTokenAsync(TokenType.MegaAdmin);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var command = new AddRoleAuthorizationCommand
            {
                RoleId = _roleId,
                AuthorizationId = _authorizationId
            };

            // Act
            var response = await _client.PostAsJsonAsync(_baseUrlForV1, command);
            var result = await response.Content.ReadFromJsonAsync<RoleAuthorizationStandardResult>();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            result.Should()
                .NotBeNull()
                .And.BeOfType<RoleAuthorizationStandardResult>();
        }

        [Fact]
        public async Task Add_RoleAuthorization_WithNotAdminUser_403()
        {
            // Arrange
            var token = await GetAccessTokenAsync(TokenType.RW);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var command = new AddRoleAuthorizationCommand
            {
                RoleId = _roleId,
                AuthorizationId = _authorizationId
            };

            // Act
            var response = await _client.PostAsJsonAsync(_baseUrlForV1, command);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Add_RoleAuthorization_WithNoAuth_401()
        {
            // Arrange
            var command = new AddRoleAuthorizationCommand
            {
                RoleId = _roleId,
                AuthorizationId = _authorizationId
            };

            // Act
            var response = await _client.PostAsJsonAsync(_baseUrlForV1, command);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Add_RoleAuthorization_AlreadyExists_409()
        {
            // Arrange
            var token = await GetAccessTokenAsync(TokenType.MegaAdmin);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var command = new AddRoleAuthorizationCommand
            {
                RoleId = new Guid("141a0000-3c36-7456-b223-08dce6346ddc"),
                AuthorizationId = new Guid("60260000-3c36-7456-5cf4-08dce60fd766")
            };

            // Act
            var response = await _client.PostAsJsonAsync(_baseUrlForV1, command);
            var result = await response.Content.ReadFromJsonAsync<CustomProblemDetails>();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Conflict);
            result.Should()
                .NotBeNull()
                .And.BeOfType<CustomProblemDetails>()
                .And.Match<CustomProblemDetails>(x => x.Errors.First().Code == "ROLEAUTHORIZATION_ALREADY_EXISTS");
        }

        [Fact]
        public async Task Add_RoleAuthorization_WithNotABaseRole_400()
        {
            // Arrange
            var token = await GetAccessTokenAsync(TokenType.MegaAdmin);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var command = new AddRoleAuthorizationCommand
            {
                RoleId = new Guid("989e0000-088f-d0ad-9cf1-08dcedbf070e"),
                AuthorizationId = new Guid("d05a0000-3c36-7456-8576-08dce616ad08")
            };

            // Act
            var response = await _client.PostAsJsonAsync(_baseUrlForV1, command);
            var result = await response.Content.ReadFromJsonAsync<CustomProblemDetails>();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            result.Should()
                .NotBeNull()
                .And.BeOfType<CustomProblemDetails>()
                .And.Match<CustomProblemDetails>(x => x.Errors.First().Code == "ROLE_AUTHORIZATION_NOT_A_BASE_ROLE_OR_NOT_EXISTS");
        }

        [Fact]
        public async Task Add_RoleAuthorization_BadAuthorizationId_400()
        {
            // Arrange
            var token = await GetAccessTokenAsync(TokenType.MegaAdmin);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var command = new AddRoleAuthorizationCommand
            {
                RoleId = new Guid("74410000-088f-d0ad-f4d5-08dcedd9c6a3"),
                AuthorizationId = NewId.NextGuid()
            };

            // Act
            var response = await _client.PostAsJsonAsync(_baseUrlForV1, command);
            var result = await response.Content.ReadFromJsonAsync<CustomProblemDetails>();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            result.Should()
                .NotBeNull()
                .And.BeOfType<CustomProblemDetails>()
                .And.Match<CustomProblemDetails>(x => x.Errors.First().Code == "ROLEAUTHORIZATION_AUTHORIZATION_NOT_FOUND");
        }

        [Fact]
        public async Task Delete_RoleAuthorization_WithAdminUser_OK()
        {
            // Arrange
            var token = await GetAccessTokenAsync(TokenType.MegaAdmin);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Act
            var response = await _client.DeleteAsync($"{_baseUrlForV1}/{_roleAuthorizationIdToDel}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task Delete_RoleAuthorization_WithNotAdminUser_403()
        {
            // Arrange
            var token = await GetAccessTokenAsync(TokenType.RW);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Act
            var response = await _client.DeleteAsync($"{_baseUrlForV1}/{_roleAuthorizationIdToDel}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Delete_RoleAuthorization_WithNoAuth_401()
        {
            // Act
            var response = await _client.DeleteAsync($"{_baseUrlForV1}/{_roleAuthorizationIdToDel}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Delete_RoleAuthorization_WithBadId_404()
        {
            // Arrange
            var token = await GetAccessTokenAsync(TokenType.MegaAdmin);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Act
            var response = await _client.DeleteAsync($"{_baseUrlForV1}/{NewId.NextGuid()}");
            var result = await response.Content.ReadFromJsonAsync<CustomProblemDetails>();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            result.Should()
                .NotBeNull()
                .And.BeOfType<CustomProblemDetails>()
                .And.Match<CustomProblemDetails>(x => x.Errors.First().Code == "ROLEAUTHORIZATION_NOT_FOUND");
        }

        [Fact]
        public async Task Delete_RoleAuthorization_WithRoleNotABaseRole_400()
        {
            // Arrange
            var token = await GetAccessTokenAsync(TokenType.MegaAdmin);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Act
            var response = await _client.DeleteAsync($"{_baseUrlForV1}/{_roleAuthorizationIdNotABaseRole}");
            var result = await response.Content.ReadFromJsonAsync<CustomProblemDetails>();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            result.Should()
                .NotBeNull()
                .And.BeOfType<CustomProblemDetails>()
                .And.Match<CustomProblemDetails>(x => x.Errors.First().Code == "ROLE_AUTHORIZATION_NOT_A_BASE_ROLE_OR_NOT_EXISTS");
        }

    }
}
