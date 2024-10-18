using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Ubik.Security.Contracts.Authorizations.Results;
using Ubik.Security.Contracts.Tenants.Results;
using Ubik.ApiService.Common.Exceptions;
using Ubik.Security.Contracts.Tenants.Commands;
using Ubik.Security.Contracts.Authorizations.Commands;
using MassTransit;

namespace Ubik.Api.Tests.Integration.Features.Security.Tenants
{
    public class TenantAdminController_Test : BaseIntegrationTestSecurity
    {
        private readonly string _baseUrlForV1;
        private readonly HttpClient _client;
        private readonly static Guid _tenantId = new("74a20000-088f-d0ad-7a4e-08dce86b0459");  //ubik_tenant_test_2 - testrw
        private readonly static Guid _tenantIdToDel = new("f0130000-088f-d0ad-5682-08dcede59555");

        public TenantAdminController_Test(IntegrationTestProxyFactory factory) : base(factory)
        {
            _baseUrlForV1 = "/usrmgt/admin/api/v1/tenants";
            _client = Factory.CreateDefaultClient();
        }

        //Get all Authorization test
        [Fact]
        public async Task Get_Tenants_All_WithAdminUser_OK()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.MegaAdmin);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.GetAsync(_baseUrlForV1);
            var result = await response.Content.ReadFromJsonAsync<List<TenantStandardResult>>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Should()
                .NotBeNull()
                .And.BeOfType<List<TenantStandardResult>>(); ;
        }

        [Fact]
        public async Task Get_Tenants_All_WithNotAdminUser_403()
        {
            var token = await GetAccessTokenAsync(TokenType.RW);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.GetAsync(_baseUrlForV1);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Get_Tenants_All_WithNoAuth_401()
        {
            //Act
            var response = await _client.GetAsync(_baseUrlForV1);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Get_Tenant_By_Id_WithAdminUser_OK()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.MegaAdmin);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.GetAsync($"{_baseUrlForV1}/{_tenantId}");
            var result = await response.Content.ReadFromJsonAsync<TenantStandardResult>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Should()
                .NotBeNull()
                .And.BeOfType<TenantStandardResult>();
        }

        [Fact]
        public async Task Get_Tenant_By_Id_WithNotAdminUser_403()
        {
            var token = await GetAccessTokenAsync(TokenType.RW);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.GetAsync($"{_baseUrlForV1}/{_tenantId}");

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Get_Tenant_By_Id_WithNoAuth_401()
        {
            //Act
            var response = await _client.GetAsync($"{_baseUrlForV1}/{_tenantId}");

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Get_Tenant_By_Id_BadId_404()
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
                .And.Match<CustomProblemDetails>(x => x.Errors.First().Code == "TENANT_NOT_FOUND");
        }

        [Fact]
        public async Task Add_Tenant_WithAdminUser_OK()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.MegaAdmin);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var command = new AddTenantCommand
            {
                Code = "test_tenant",
                Label = "Test Tenant",
                Description = "Test Tenant Description",
            };

            //Act
            var response = await _client.PostAsJsonAsync(_baseUrlForV1, command);
            var result = await response.Content.ReadFromJsonAsync<TenantStandardResult>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            result.Should()
                .NotBeNull()
                .And.BeOfType<TenantStandardResult>();
        }

        [Fact]
        public async Task Add_Tenant_WithNotAdminUser_403()
        {
            var token = await GetAccessTokenAsync(TokenType.RW);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var command = new AddTenantCommand
            {
                Code = "test_tenant",
                Label = "Test Tenant",
                Description = "Test Tenant Description",
            };

            //Act
            var response = await _client.PostAsJsonAsync(_baseUrlForV1, command);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Add_Tenant_WithNoAuth_401()
        {
            var command = new AddTenantCommand
            {
                Code = "test_tenant",
                Label = "Test Tenant",
                Description = "Test Tenant Description",
            };

            //Act
            var response = await _client.PostAsJsonAsync(_baseUrlForV1, command);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Add_Tenant_WithExistingCode_409()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.MegaAdmin);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var command = new AddTenantCommand
            {
                Code = "test_tenant_2times",
                Label = "Test Tenant",
                Description = "Test Tenant Description",
            };

            //Act
            await _client.PostAsJsonAsync(_baseUrlForV1, command);
            var response = await _client.PostAsJsonAsync(_baseUrlForV1, command);
            var result = await response.Content.ReadFromJsonAsync<CustomProblemDetails>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Conflict);
            result.Should()
                .NotBeNull()
                .And.BeOfType<CustomProblemDetails>()
                .And.Match<CustomProblemDetails>(x => x.Errors.First().Code == "TENANT_ALREADY_EXISTS");
        }

        [Fact]
        public async Task Update_Tenant_WithAdminUser_OK()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.MegaAdmin);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var command = new UpdateAuthorizationCommand
            {
                Id = _tenantId,
                Code = "ubik_tenant_test_2 - testrw",
                Description = "Test",
                Label = "TestLabel",
                Version = new Guid("74a20000-088f-d0ad-89e8-08dce86b0459")
            };

            //Act
            var response = await _client.PutAsJsonAsync($"{_baseUrlForV1}/{_tenantId}", command);
            var result = await response.Content.ReadFromJsonAsync<AuthorizationStandardResult>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Should()
                .NotBeNull()
                .And.BeOfType<AuthorizationStandardResult>()
                .And.Match<AuthorizationStandardResult>(x => x.Description == "Test");
        }

        [Fact]
        public async Task Update_Tenant_WithNotAdminUser_403()
        {
            var token = await GetAccessTokenAsync(TokenType.RW);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var command = new UpdateAuthorizationCommand
            {
                Id = _tenantId,
                Code = "ubik_tenant_test_2 - testrw",
                Description = "Test",
                Label = "TestLabel",
                Version = new Guid("74a20000-088f-d0ad-89e8-08dce86b0459")
            };

            //Act
            var response = await _client.PutAsJsonAsync($"{_baseUrlForV1}/{_tenantId}", command);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Update_Tenant_WithNoAuth_401()
        {
            var command = new UpdateAuthorizationCommand
            {
                Id = _tenantId,
                Code = "ubik_tenant_test_2 - testrw",
                Description = "Test",
                Label = "TestLabel",
                Version = new Guid("74a20000-088f-d0ad-89e8-08dce86b0459")
            };

            //Act
            var response = await _client.PutAsJsonAsync($"{_baseUrlForV1}/{_tenantId}", command);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Update_Tenant_WithBadId_400()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.MegaAdmin);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var id = Guid.NewGuid();
            var command = new UpdateAuthorizationCommand
            {
                Id = id,
                Code = "ubik_tenant_test_x - testrw",
                Description = "Test",
                Label = "TestLabel",
                Version = new Guid("74a20000-088f-d0ad-89e8-08dce86b0459")
            };

            //Act
            var response = await _client.PutAsJsonAsync($"{_baseUrlForV1}/{id}", command);
            var result = await response.Content.ReadFromJsonAsync<CustomProblemDetails>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            result.Should()
                .NotBeNull()
                .And.BeOfType<CustomProblemDetails>()
                .And.Match<CustomProblemDetails>(x => x.Errors.First().Code == "TENANT_NOT_FOUND");
        }

        [Fact]
        public async Task Update_Tenant_NotMatchId_400()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.MegaAdmin);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var command = new UpdateAuthorizationCommand
            {
                Id = Guid.NewGuid(),
                Code = "ubik_tenant_test_x - testrw",
                Description = "Test",
                Label = "TestLabel",
                Version = new Guid("74a20000-088f-d0ad-89e8-08dce86b0459")
            };

            //Act
            var response = await _client.PutAsJsonAsync($"{_baseUrlForV1}/{Guid.NewGuid()}", command);
            var result = await response.Content.ReadFromJsonAsync<CustomProblemDetails>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            result.Should()
                .NotBeNull()
                .And.BeOfType<CustomProblemDetails>()
                .And.Match<CustomProblemDetails>(x => x.Errors.First().Code == "TENANT_UPDATE_IDS_NOT_MATCH");
        }

        [Fact]
        public async Task Update_Tenant_AlreadyExistsWithOtherId_409()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.MegaAdmin);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var command = new UpdateAuthorizationCommand
            {
                Id = _tenantId,
                Code = "ubik_tenant_test_1 - testrw",
                Description = "Test",
                Label = "TestLabel",
                Version = new Guid("74a20000-088f-d0ad-89e8-08dce86b0459")
            };

            //Act
            var response = await _client.PutAsJsonAsync($"{_baseUrlForV1}/{_tenantId}", command);
            var result = await response.Content.ReadFromJsonAsync<CustomProblemDetails>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Conflict);
            result.Should()
                .NotBeNull()
                .And.BeOfType<CustomProblemDetails>()
                .And.Match<CustomProblemDetails>(x => x.Errors.First().Code == "TENANT_ALREADY_EXISTS");
        }

        [Fact]
        public async Task Update_Tenant_WithBadVersion_409()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.MegaAdmin);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var command = new UpdateAuthorizationCommand
            {
                Id = _tenantId,
                Code = "ubik_tenant_test_z - testrw",
                Description = "Test",
                Label = "TestLabel",
                Version = NewId.NextGuid()
            };

            //Act
            var response = await _client.PutAsJsonAsync($"{_baseUrlForV1}/{_tenantId}", command);
            var result = await response.Content.ReadFromJsonAsync<CustomProblemDetails>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Conflict);
            result.Should()
                .NotBeNull()
                .And.BeOfType<CustomProblemDetails>()
                .And.Match<CustomProblemDetails>(x => x.Errors.First().Code == "TENANT_UPDATE_CONCURRENCY");
        }

        [Fact]
        public async Task Delete_Tenant_WithAdminUser_OK()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.MegaAdmin);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.DeleteAsync($"{_baseUrlForV1}/{_tenantIdToDel}");

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task Delete_Tenant_WithNotAdminUser_403()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.RW);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.DeleteAsync($"{_baseUrlForV1}/{_tenantIdToDel}");

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Delete_Tenant_WithNoAuth_401()
        {
            //Act
            var response = await _client.DeleteAsync($"{_baseUrlForV1}/{_tenantIdToDel}");

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }
    }
}
