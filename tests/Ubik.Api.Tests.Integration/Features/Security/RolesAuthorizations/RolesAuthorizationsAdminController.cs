using FluentAssertions;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Ubik.Security.Contracts.RoleAuthorizations.Events;

namespace Ubik.Api.Tests.Integration.Features.Security.RolesAuthorizations
{
    public class RolesAuthorizationsAdminController : BaseIntegrationTest
    {
        private readonly string _baseUrlForV1;
        private readonly HttpClient _client;
        private readonly static Guid _roleAuthorizationId = new("3c730000-088f-d0ad-962c-08dce873f4c3");
        private readonly static Guid _roleAuthorizationIdToDel = new("04280000-088f-d0ad-15f9-08dcedd377eb");
        private readonly static Guid _roleAuthorizationIdNotABaseRole = new("04280000-088f-d0ad-048a-08dcedd38d77");

        public RolesAuthorizationsAdminController(IntegrationTestProxyFactory factory) : base(factory)
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


    }
}
