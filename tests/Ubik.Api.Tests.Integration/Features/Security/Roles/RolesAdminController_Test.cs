using FluentAssertions;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net;
using Ubik.Security.Contracts.Roles.Results;
using Ubik.ApiService.Common.Exceptions;
using MassTransit;
using Ubik.Security.Contracts.Roles.Commands;
using Ubik.Security.Contracts.Authorizations.Commands;

namespace Ubik.Api.Tests.Integration.Features.Security.Roles
{
    public class RolesAdminController_Test : BaseIntegrationTest
    {
        private readonly string _baseUrlForV1;
        private readonly HttpClient _client;
        private readonly static Guid _roleId = new("141a0000-3c36-7456-b223-08dce6346ddc");  //usrmgt_all_rw
        private readonly static Guid _roleIdToDel = new("b8650000-088f-d0ad-2726-08dcedbd2375");
        private readonly static Guid _roleIdNotABaseRole = new("989e0000-088f-d0ad-9cf1-08dcedbf070e");

        public RolesAdminController_Test(IntegrationTestProxyFactory factory) : base(factory)
        {
            _baseUrlForV1 = "/usrmgt/admin/api/v1/roles";
            _client = Factory.CreateDefaultClient();
        }

        [Fact]
        public async Task Get_Roles_All_WithAdminUser_OK()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.MegaAdmin);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.GetAsync(_baseUrlForV1);
            var result = await response.Content.ReadFromJsonAsync<List<RoleStandardResult>>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Should()
                .NotBeNull()
                .And.BeOfType<List<RoleStandardResult>>();
        }

        [Fact]
        public async Task Get_Roles_All_WithNotAdminUser_403()
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
        public async Task Get_Roles_All_WithNoAuth_401()
        {
            // Act
            var response = await _client.GetAsync(_baseUrlForV1);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Get_Role_By_Id_WithAdminUser_OK()
        {
            // Arrange
            var token = await GetAccessTokenAsync(TokenType.MegaAdmin);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Act
            var response = await _client.GetAsync($"{_baseUrlForV1}/{_roleId}");
            var result = await response.Content.ReadFromJsonAsync<RoleStandardResult>();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Should()
                .NotBeNull()
                .And.BeOfType<RoleStandardResult>();
        }

        [Fact]
        public async Task Get_Role_By_Id_WithNotAdminUser_403()
        {
            // Arrange
            var token = await GetAccessTokenAsync(TokenType.RW);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Act
            var response = await _client.GetAsync($"{_baseUrlForV1}/{_roleId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Get_Role_By_Id_WithNoAuth_401()
        {
            // Act
            var response = await _client.GetAsync($"{_baseUrlForV1}/{_roleId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Get_Role_By_Id_WithBadId_404()
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
                .And.Match<CustomProblemDetails>(x => x.Errors.First().Code == "ROLE_NOT_FOUND");
        }

        [Fact]
        public async Task Get_Role_By_Id_WithRoleNotABaseRole_404()
        {
            // Arrange
            var token = await GetAccessTokenAsync(TokenType.MegaAdmin);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Act
            var response = await _client.GetAsync($"{_baseUrlForV1}/{_roleIdNotABaseRole}");
            var result = await response.Content.ReadFromJsonAsync<CustomProblemDetails>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            result.Should()
                .NotBeNull()
                .And.BeOfType<CustomProblemDetails>()
                .And.Match<CustomProblemDetails>(x => x.Errors.First().Code == "ROLE_NOT_FOUND");
        }

        [Fact]
        public async Task Add_Role_WithAdminUser_OK()
        {
            // Arrange
            var token = await GetAccessTokenAsync(TokenType.MegaAdmin);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var command = new AddRoleCommand
            {
                Code = "TestRole",
                Label = "TestRole",
                Description = "TestRole"
            };

            // Act
            var response = await _client.PostAsJsonAsync(_baseUrlForV1, command);
            var result = await response.Content.ReadFromJsonAsync<RoleStandardResult>();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            result.Should()
                .NotBeNull()
                .And.BeOfType<RoleStandardResult>();
        }

        [Fact]
        public async Task Add_Role_WithNotAdminUser_403()
        {
            // Arrange
            var token = await GetAccessTokenAsync(TokenType.RW);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var command = new AddRoleCommand
            {
                Code = "TestRole",
                Label = "TestRole",
                Description = "TestRole"
            };

            // Act
            var response = await _client.PostAsJsonAsync(_baseUrlForV1, command);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Add_Role_WithNoAuth_401()
        {
            // Arrange
            var command = new AddRoleCommand
            {
                Code = "TestRole",
                Label = "TestRole",
                Description = "TestRole"
            };

            // Act
            var response = await _client.PostAsJsonAsync(_baseUrlForV1, command);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Add_Role_WithExistingCode_409()
        {
            // Arrange
            var token = await GetAccessTokenAsync(TokenType.MegaAdmin);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var command = new AddRoleCommand
            {
                Code = "usrmgt_all_rw",
                Label = "TestRole",
                Description = "TestRole"
            };

            // Act
            var response = await _client.PostAsJsonAsync(_baseUrlForV1, command);
            var result = await response.Content.ReadFromJsonAsync<CustomProblemDetails>();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Conflict);
            result.Should()
                .NotBeNull()
                .And.BeOfType<CustomProblemDetails>()
                .And.Match<CustomProblemDetails>(x => x.Errors.First().Code == "ROLE_ALREADY_EXISTS");
        }

        [Fact]
        public async Task Update_Role_WithAdminUser_OK()
        {
            // Arrange
            var token = await GetAccessTokenAsync(TokenType.MegaAdmin);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var command = new UpdateRoleCommand
            {
                Id = _roleId,
                Code = "usrmgt_all_rw",
                Label = "TestRole",
                Description = "TEST",
                Version = new Guid("141a0000-3c36-7456-3676-08dce63488f8")
            };

            // Act
            var response = await _client.PutAsJsonAsync($"{_baseUrlForV1}/{_roleId}", command);
            var result = await response.Content.ReadFromJsonAsync<RoleStandardResult>();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Should()
                .NotBeNull()
                .And.BeOfType<RoleStandardResult>()
                .And.Match<RoleStandardResult>(x => x.Description == "TEST");
        }

        [Fact]
        public async Task Update_Role_WithNotAdminUser_403()
        {
            // Arrange
            var token = await GetAccessTokenAsync(TokenType.RW);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var command = new UpdateRoleCommand
            {
                Id = _roleId,
                Code = "usrmgt_all_rw",
                Label = "TestRole",
                Description = "TEST",
                Version = _roleId
            };

            // Act
            var response = await _client.PutAsJsonAsync($"{_baseUrlForV1}/{_roleId}", command);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Update_Role_WithNoAuth_401()
        {
            // Arrange
            var command = new UpdateRoleCommand
            {
                Id = _roleId,
                Code = "usrmgt_all_rw",
                Label = "TestRole",
                Description = "TEST",
                Version = _roleId
            };

            // Act
            var response = await _client.PutAsJsonAsync($"{_baseUrlForV1}/{_roleId}", command);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Update_Role_WithBadId_404()
        {
            // Arrange
            var token = await GetAccessTokenAsync(TokenType.MegaAdmin);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var id = NewId.NextGuid();
            var command = new UpdateRoleCommand
            {
                Id = id,
                Code = "usrmgt_all_rw",
                Label = "TestRole",
                Description = "TEST",
                Version = _roleId
            };

            // Act
            var response = await _client.PutAsJsonAsync($"{_baseUrlForV1}/{id}", command);
            var result = await response.Content.ReadFromJsonAsync<CustomProblemDetails>();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            result.Should()
                .NotBeNull()
                .And.BeOfType<CustomProblemDetails>()
                .And.Match<CustomProblemDetails>(x => x.Errors.First().Code == "ROLE_NOT_FOUND");
        }

        [Fact]
        public async Task Update_Role_WithRoleNotABaseRole_404()
        {
            // Arrange
            var token = await GetAccessTokenAsync(TokenType.MegaAdmin);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            
            var command = new UpdateRoleCommand
            {
                Id = _roleIdNotABaseRole,
                Code = "usrmgt_all_rw",
                Label = "TestRole",
                Description = "TEST",
                Version = _roleId
            };

            // Act
            var response = await _client.PutAsJsonAsync($"{_baseUrlForV1}/{_roleIdNotABaseRole}", command);
            var result = await response.Content.ReadFromJsonAsync<CustomProblemDetails>();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            result.Should()
                .NotBeNull()
                .And.BeOfType<CustomProblemDetails>()
                .And.Match<CustomProblemDetails>(x => x.Errors.First().Code == "ROLE_NOT_FOUND");
        }

        [Fact]
        public async Task Update_Role_WithExistingCode_409()
        {
            // Arrange
            var token = await GetAccessTokenAsync(TokenType.MegaAdmin);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var command = new UpdateRoleCommand
            {
                Id = _roleId,
                Code = "usrmgt_all_ro",
                Label = "TestRole",
                Description = "TEST",
                Version = _roleId
            };

            // Act
            var response = await _client.PutAsJsonAsync($"{_baseUrlForV1}/{_roleId}", command);
            var result = await response.Content.ReadFromJsonAsync<CustomProblemDetails>();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Conflict);
            result.Should()
                .NotBeNull()
                .And.BeOfType<CustomProblemDetails>()
                .And.Match<CustomProblemDetails>(x => x.Errors.First().Code == "ROLE_ALREADY_EXISTS");
        }

        [Fact]
        public async Task Update_Role_NotMatchId_400()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.MegaAdmin);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var command = new UpdateAuthorizationCommand
            {
                Id = _roleId,
                Code = "test",
                Description = "Test",
                Label = "TestLabel",
                Version = _roleId
            };

            //Act
            var response = await _client.PutAsJsonAsync($"{_baseUrlForV1}/{Guid.NewGuid()}", command);
            var result = await response.Content.ReadFromJsonAsync<CustomProblemDetails>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            result.Should()
                .NotBeNull()
                .And.BeOfType<CustomProblemDetails>()
                .And.Match<CustomProblemDetails>(x => x.Errors.First().Code == "ROLE_UPDATE_IDS_NOT_MATCH");
        }

        [Fact]
        public async Task Update_Role_WithBadVersion_409()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.MegaAdmin);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var command = new UpdateAuthorizationCommand
            {
                Id = _roleId,
                Code = "test",
                Description = "Test",
                Label = "TestLabel",
                Version = NewId.NextGuid()
            };

            //Act
            var response = await _client.PutAsJsonAsync($"{_baseUrlForV1}/{_roleId}", command);
            var result = await response.Content.ReadFromJsonAsync<CustomProblemDetails>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Conflict);
            result.Should()
                .NotBeNull()
                .And.BeOfType<CustomProblemDetails>()
                .And.Match<CustomProblemDetails>(x => x.Errors.First().Code == "ROLE_UPDATE_CONCURRENCY");
        }

        [Fact]
        public async Task Delete_Role_WithAdminUser_OK()
        {
            // Arrange
            var token = await GetAccessTokenAsync(TokenType.MegaAdmin);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Act
            var response = await _client.DeleteAsync($"{_baseUrlForV1}/{_roleIdToDel}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task Delete_Role_WithNotAdminUser_403()
        {
            // Arrange
            var token = await GetAccessTokenAsync(TokenType.RW);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Act
            var response = await _client.DeleteAsync($"{_baseUrlForV1}/{_roleIdToDel}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Delete_Role_WithNoAuth_401()
        {
            // Act
            var response = await _client.DeleteAsync($"{_baseUrlForV1}/{_roleIdToDel}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Delete_Role_WithBadId_404()
        {
            // Arrange
            var token = await GetAccessTokenAsync(TokenType.MegaAdmin);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Act
            var response = await _client.DeleteAsync($"{_baseUrlForV1}/{NewId.NextGuid()}");
            var result = await response.Content.ReadFromJsonAsync<CustomProblemDetails>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            result.Should()
                .NotBeNull()
                .And.BeOfType<CustomProblemDetails>()
                .And.Match<CustomProblemDetails>(x => x.Errors.First().Code == "ROLE_NOT_FOUND");
        }

        [Fact]
        public async Task Delete_Role_WithRoleNotABaseRol_404()
        {
            // Arrange
            var token = await GetAccessTokenAsync(TokenType.MegaAdmin);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Act
            var response = await _client.DeleteAsync($"{_baseUrlForV1}/{_roleIdNotABaseRole}");
            var result = await response.Content.ReadFromJsonAsync<CustomProblemDetails>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            result.Should()
                .NotBeNull()
                .And.BeOfType<CustomProblemDetails>()
                .And.Match<CustomProblemDetails>(x => x.Errors.First().Code == "ROLE_NOT_FOUND");
        }
    }
}
