using FluentAssertions;
using MassTransit;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Ubik.ApiService.Common.Exceptions;
using Ubik.Security.Contracts.Roles.Results;
using Ubik.Security.Contracts.Users.Results;

namespace Ubik.Api.Tests.Integration.Features.Security.Users
{
    public class UsersController_Test : BaseIntegrationTestSecurity
    {
        private readonly string _baseUrlForV1;
        private readonly HttpClient _client;
        private readonly static Guid _userId = new Guid("c8660000-3c36-7456-580a-08dce562105f");
        private readonly static Guid _roleId = new Guid("141a0000-3c36-7456-b223-08dce6346ddc");

        public UsersController_Test(IntegrationTestProxyFactory factory) : base(factory)
        {
            _baseUrlForV1 = "/usrmgt/api/v1/users";
            _client = Factory.CreateDefaultClient();
        }


        [Theory]
        [InlineData("c8660000-3c36-7456-580a-08dce562105f")]
        [InlineData("d48c0000-088f-d0ad-1adf-08dced1562aa")]
        [InlineData("d48c0000-088f-d0ad-1d00-08dced1562aa")]
        public async Task Get_User_WithRW_OK(string userId)
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.RW);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.GetAsync($"{_baseUrlForV1}/{userId}");
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

        [Theory]
        [InlineData("c8660000-3c36-7456-580a-08dce562105f")]
        [InlineData("d48c0000-088f-d0ad-1adf-08dced1562aa")]
        [InlineData("d48c0000-088f-d0ad-1d00-08dced1562aa")]
        public async Task Get_User_WithRO_OK(string userId)
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.RO);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.GetAsync($"{_baseUrlForV1}/{userId}");
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
            var response = await _client.GetAsync($"{_baseUrlForV1}/{_userId}");
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Get_User_WithAdminRole_403()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.MegaAdmin);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.GetAsync($"{_baseUrlForV1}/{_userId}");
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }


        [Fact]
        public async Task Get_User_WithNoAuth_401()
        {
            //Arrange


            //Act
            var response = await _client.GetAsync($"{_baseUrlForV1}/{_userId}");
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Theory]
        [InlineData("c8660000-3c36-7456-580a-08dce562105f")]
        [InlineData("d48c0000-088f-d0ad-1adf-08dced1562aa")]
        [InlineData("d48c0000-088f-d0ad-1d00-08dced1562aa")]
        public async Task Get_User_Roles_WithRW_OK(string userId)
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.RW);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.GetAsync($"{_baseUrlForV1}/{userId}/roles");
            var result = await response.Content.ReadFromJsonAsync<List<RoleStandardResult>>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Should()
                .NotBeNull()
                .And.BeOfType<List<RoleStandardResult>>(); ;
        }

        [Theory]
        [InlineData("c8660000-3c36-7456-580a-08dce562105f")]
        [InlineData("d48c0000-088f-d0ad-1adf-08dced1562aa")]
        [InlineData("d48c0000-088f-d0ad-1d00-08dced1562aa")]
        public async Task Get_User_Roles_WithRO_OK(string userId)
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.RO);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.GetAsync($"{_baseUrlForV1}/{userId}/roles");
            var result = await response.Content.ReadFromJsonAsync<List<RoleStandardResult>>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Should()
                .NotBeNull()
                .And.BeOfType<List<RoleStandardResult>>(); ;
        }

        [Fact]
        public async Task Get_User_Roles_WithAdminUser_403()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.MegaAdmin);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.GetAsync($"{_baseUrlForV1}/{_userId}/roles");

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Get_User_Roles_WithNoRole_403()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.NoRole);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.GetAsync($"{_baseUrlForV1}/{_userId}/roles");

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Get_User_Roles_WithNoAuth_401()
        {
            //Arrange

            //Act
            var response = await _client.GetAsync($"{_baseUrlForV1}/{_userId}/roles");

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Get_User_Roles_BadUserId_404()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.RW);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.GetAsync($"{_baseUrlForV1}/{NewId.NextGuid()}/roles");
            var result = await response.Content.ReadFromJsonAsync<CustomProblemDetails>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            result.Should()
                .NotBeNull()
                .And.BeOfType<CustomProblemDetails>()
                .And.Match<CustomProblemDetails>(x => x.Errors.First().Code == "USER_NOT_FOUND");
        }

        [Theory]
        [InlineData("e84d0000-088f-d0ad-3a6e-08dced19f063")]
        [InlineData("5c5e0000-3c36-7456-b9da-08dcdf9832e2")]
        public async Task Get_User_Roles_UserNotInTenant_404(string userId)
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.RW);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.GetAsync($"{_baseUrlForV1}/{userId}/roles");
            var result = await response.Content.ReadFromJsonAsync<CustomProblemDetails>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            result.Should()
                .NotBeNull()
                .And.BeOfType<CustomProblemDetails>()
                .And.Match<CustomProblemDetails>(x => x.Errors.First().Code == "USER_NOT_FOUND");
        }

        [Fact]
        public async Task Get_User_Role_WithRW_OK()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.RW);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.GetAsync($"{_baseUrlForV1}/{_userId}/roles/{_roleId}");
            var result = await response.Content.ReadFromJsonAsync<RoleStandardResult>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Should()
                .NotBeNull()
                .And.BeOfType<RoleStandardResult>(); ;
        }

        [Fact]
        public async Task Get_User_Role_WithRO_OK()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.RO);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.GetAsync($"{_baseUrlForV1}/{_userId}/roles/{_roleId}");
            var result = await response.Content.ReadFromJsonAsync<RoleStandardResult>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Should()
                .NotBeNull()
                .And.BeOfType<RoleStandardResult>(); ;
        }

        [Fact]
        public async Task Get_User_Role_WithAdminUser_403()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.MegaAdmin);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.GetAsync($"{_baseUrlForV1}/{_userId}/roles/{_roleId}");

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Get_User_Role_WithNoRole_403()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.NoRole);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.GetAsync($"{_baseUrlForV1}/{_userId}/roles/{_roleId}");

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Get_User_Role_WithNoAuth_401()
        {
            //Arrange

            //Act
            var response = await _client.GetAsync($"{_baseUrlForV1}/{_userId}/roles/{_roleId}");

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Theory]
        [InlineData("e84d0000-088f-d0ad-3a6e-08dced19f063")]
        [InlineData("5c5e0000-3c36-7456-b9da-08dcdf9832e2")]
        public async Task Get_User_Role_UserNotInTenant_404(string userId)
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.RW);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.GetAsync($"{_baseUrlForV1}/{userId}/roles/{_roleId}");
            var result = await response.Content.ReadFromJsonAsync<CustomProblemDetails>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            result.Should()
                .NotBeNull()
                .And.BeOfType<CustomProblemDetails>()
                .And.Match<CustomProblemDetails>(x => x.Errors.First().Code == "USER_NOT_FOUND");
        }

        [Fact]
        public async Task Get_User_Role_BadUserId_404()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.RW);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.GetAsync($"{_baseUrlForV1}/{NewId.NextGuid()}/roles/{_roleId}");
            var result = await response.Content.ReadFromJsonAsync<CustomProblemDetails>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            result.Should()
                .NotBeNull()
                .And.BeOfType<CustomProblemDetails>()
                .And.Match<CustomProblemDetails>(x => x.Errors.First().Code == "USER_NOT_FOUND");
        }

        [Fact]
        public async Task Get_User_Role_RoleNotInTenant_404()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.RW);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.GetAsync($"{_baseUrlForV1}/{_userId}/roles/989e0000-088f-d0ad-9cf1-08dcedbf070e");
            var result = await response.Content.ReadFromJsonAsync<CustomProblemDetails>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            result.Should()
                .NotBeNull()
                .And.BeOfType<CustomProblemDetails>()
                .And.Match<CustomProblemDetails>(x => x.Errors.First().Code == "ROLE_NOT_FOUND");
        }

        [Fact]
        public async Task Get_User_Role_BadRoleId_404()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.RW);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.GetAsync($"{_baseUrlForV1}/{_userId}/roles/{NewId.NextGuid()}");
            var result = await response.Content.ReadFromJsonAsync<CustomProblemDetails>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            result.Should()
                .NotBeNull()
                .And.BeOfType<CustomProblemDetails>()
                .And.Match<CustomProblemDetails>(x => x.Errors.First().Code == "ROLE_NOT_FOUND");
        }

        [Fact]
        public async Task Add_User_Role_WithRW_OK()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.RW);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.PostAsync($"{_baseUrlForV1}/d4520000-3c36-7456-cac1-08dcef793b1a/roles/f47b0000-088f-d0ad-c1b9-08dced163f7e", null);
            var result = await response.Content.ReadFromJsonAsync<RoleStandardResult>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            result.Should()
                .NotBeNull()
                .And.BeOfType<RoleStandardResult>();
        }

        [Fact]
        public async Task Add_User_Role_WithRO_403()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.RO);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.PostAsync($"{_baseUrlForV1}/d4520000-3c36-7456-cac1-08dcef793b1a/roles/f47b0000-088f-d0ad-c1b9-08dced163f7e", null);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Add_User_Role_WithAdminUser_403()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.MegaAdmin);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.PostAsync($"{_baseUrlForV1}/d4520000-3c36-7456-cac1-08dcef793b1a/roles/f47b0000-088f-d0ad-c1b9-08dced163f7e", null);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Add_User_Role_WithOtherTenantUser_404()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.OtherTenant);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.PostAsync($"{_baseUrlForV1}/d4520000-3c36-7456-cac1-08dcef793b1a/roles/f47b0000-088f-d0ad-c1b9-08dced163f7e", null);
            var result = await response.Content.ReadFromJsonAsync<CustomProblemDetails>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            result.Should()
                .NotBeNull()
                .And.BeOfType<CustomProblemDetails>()
                .And.Match<CustomProblemDetails>(x => x.Errors.First().Code == "USER_NOT_FOUND");
        }

        [Fact]
        public async Task Add_User_Role_WithNoRole_403()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.NoRole);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.PostAsync($"{_baseUrlForV1}/d4520000-3c36-7456-cac1-08dcef793b1a/roles/f47b0000-088f-d0ad-c1b9-08dced163f7e", null);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Add_User_Role_WithNoAuth_401()
        {
            //Arrange

            //Act
            var response = await _client.PostAsync($"{_baseUrlForV1}/d4520000-3c36-7456-cac1-08dcef793b1a/roles/f47b0000-088f-d0ad-c1b9-08dced163f7e", null);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Add_User_Role_BadUserId_404()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.RW);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.PostAsync($"{_baseUrlForV1}/{NewId.NextGuid()}/roles/f47b0000-088f-d0ad-c1b9-08dced163f7e", null);
            var result = await response.Content.ReadFromJsonAsync<CustomProblemDetails>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            result.Should()
                .NotBeNull()
                .And.BeOfType<CustomProblemDetails>()
                .And.Match<CustomProblemDetails>(x => x.Errors.First().Code == "USER_NOT_FOUND");
        }

        [Fact]
        public async Task Add_User_Role_UserNotInTenant_404()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.RW);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.PostAsync($"{_baseUrlForV1}/e84d0000-088f-d0ad-3a6e-08dced19f063/roles/f47b0000-088f-d0ad-c1b9-08dced163f7e", null);
            var result = await response.Content.ReadFromJsonAsync<CustomProblemDetails>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            result.Should()
                .NotBeNull()
                .And.BeOfType<CustomProblemDetails>()
                .And.Match<CustomProblemDetails>(x => x.Errors.First().Code == "USER_NOT_FOUND");
        }

        [Fact]
        public async Task Add_User_Role_BadRoleId_404()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.RW);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.PostAsync($"{_baseUrlForV1}/{_userId}/roles/{NewId.NextGuid()}", null);
            var result = await response.Content.ReadFromJsonAsync<CustomProblemDetails>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            result.Should()
                .NotBeNull()
                .And.BeOfType<CustomProblemDetails>()
                .And.Match<CustomProblemDetails>(x => x.Errors.First().Code == "ROLE_NOT_FOUND");
        }

        [Fact]
        public async Task Add_User_Role_RoleNotInTenant_404()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.RW);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.PostAsync($"{_baseUrlForV1}/{_userId}/roles/989e0000-088f-d0ad-9cf1-08dcedbf070e", null);
            var result = await response.Content.ReadFromJsonAsync<CustomProblemDetails>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            result.Should()
                .NotBeNull()
                .And.BeOfType<CustomProblemDetails>()
                .And.Match<CustomProblemDetails>(x => x.Errors.First().Code == "ROLE_NOT_FOUND");
        }

        [Fact]
        public async Task Add_User_Role_UserAlreadyHasRoleInTenant_409()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.RW);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.PostAsync($"{_baseUrlForV1}/c8660000-3c36-7456-580a-08dce562105f/roles/141a0000-3c36-7456-b223-08dce6346ddc", null);
            var result = await response.Content.ReadFromJsonAsync<CustomProblemDetails>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Conflict);
            result.Should()
                .NotBeNull()
                .And.BeOfType<CustomProblemDetails>()
                .And.Match<CustomProblemDetails>(x => x.Errors.First().Code == "USERROLEBYTENANT_ALREADY_EXISTS");
        }

    }
}
