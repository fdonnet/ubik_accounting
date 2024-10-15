using FluentAssertions;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net;
using Ubik.Security.Contracts.Users.Results;
using Ubik.Security.Contracts.Tenants.Results;
using Ubik.ApiService.Common.Exceptions;
using Ubik.Security.Contracts.Tenants.Commands;
using MassTransit.SagaStateMachine;

namespace Ubik.Api.Tests.Integration.Features.Security.Users
{
    public class MeController_Test : BaseIntegrationTest
    {
        private readonly string _baseUrlForV1;
        private readonly HttpClient _client;

        public MeController_Test(IntegrationTestProxyFactory factory) : base(factory)
        {
            _baseUrlForV1 = "/usrmgt/api/v1/me";
            _client = Factory.CreateDefaultClient();
        }

        [Fact]
        public async Task Get_CurrentMeUser_OK()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.RW);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.GetAsync($"{_baseUrlForV1}");
            var result = await response.Content.ReadFromJsonAsync<UserStandardResult>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Should()
                .NotBeNull()
                .And.BeOfType<UserStandardResult>(); ;
        }

        [Fact]
        public async Task Get_CurrentMeUser_NoAuth_401()
        {
            //Arrange


            //Act
            var response = await _client.GetAsync($"{_baseUrlForV1}");

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Get_CurrentMeUser_SelectedTenant_OK()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.RW);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.GetAsync($"{_baseUrlForV1}/tenants/selected");
            var result = await response.Content.ReadFromJsonAsync<TenantStandardResult>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Should()
                .NotBeNull()
                .And.BeOfType<TenantStandardResult>(); ;
        }

        [Fact]
        public async Task Get_CurrentMeUser_SelectedTenant_NoAuth_401()
        {
            //Arrange


            //Act
            var response = await _client.GetAsync($"{_baseUrlForV1}/tenants/selected");

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Get_CurrentMeUser_AllTenants_OK()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.RW);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.GetAsync($"{_baseUrlForV1}/tenants");
            var result = await response.Content.ReadFromJsonAsync<List<TenantStandardResult>>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Should()
                .NotBeNull()
                .And.BeOfType<List<TenantStandardResult>>(); ;
        }

        [Fact]
        public async Task Get_CurrentMeUser_AllTenants_NoAuth_401()
        {
            //Arrange


            //Act
            var response = await _client.GetAsync($"{_baseUrlForV1}/tenants");

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Get_CurrentMeUser_Tenant_OK()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.RW);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.GetAsync($"{_baseUrlForV1}/tenants/{new Guid("74a20000-088f-d0ad-7a4e-08dce86b0459")}");
            var result = await response.Content.ReadFromJsonAsync<TenantStandardResult>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Should()
                .NotBeNull()
                .And.BeOfType<TenantStandardResult>(); ;
        }

        [Fact]
        public async Task Get_CurrentMeUser_Tenant_NotFound_404()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.RW);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.GetAsync($"{_baseUrlForV1}/tenants/{Guid.NewGuid()}");
            var result = await response.Content.ReadFromJsonAsync<CustomProblemDetails>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            result.Should()
                .NotBeNull()
                .And.BeOfType<CustomProblemDetails>()
                .And.Match<CustomProblemDetails>(x => x.Errors.First().Code == "USER_SELECTED_TENANT_NOT_FOUND");
        }

        [Fact]
        public async Task Get_CurrentMeUser_Tenant_NoAuth_401()
        {
            //Arrange


            //Act
            var response = await _client.GetAsync($"{_baseUrlForV1}/tenants/{Guid.NewGuid()}");

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        //TODO : to be activated when cleanup is in place
        //[Fact]
        //public async Task Post_CurrentMeUser_Tenant_OK()
        //{
        //    //Arrange
        //    var token = await GetAccessTokenAsync(TokenType.RW);
        //    _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        //    //Act
        //    var response = await _client.PostAsJsonAsync($"{_baseUrlForV1}/tenants",
        //        new AddTenantCommand
        //        {
        //            Code = "TestTenant",
        //            Description = "TestTenant",
        //            Label = "TestTenant",
        //        });

        //    var result = await response.Content.ReadFromJsonAsync<TenantStandardResult>();

        //    //Assert
        //    response.StatusCode.Should().Be(HttpStatusCode.Created);
        //    result.Should()
        //        .NotBeNull()
        //        .And.BeOfType<TenantStandardResult>()
        //        .And.Match<TenantStandardResult>(x => x.Code == "TestTenant - testrw");

        //    //Assert link created with call to get endpoint
        //    var response2 = await _client.GetAsync($"{_baseUrlForV1}/tenants/{result?.Id}");
        //    var result2 = await response2.Content.ReadFromJsonAsync<TenantStandardResult>();
        //    result2.Should()
        //       .NotBeNull()
        //       .And.BeOfType<TenantStandardResult>()
        //       .And.Match<TenantStandardResult>(x => x.Code == "TestTenant - testrw");

        //    //TODO: Cleanup
        //    try
        //    {
        //        await _client.DeleteAsync($"{_baseUrlForV1}/tenants/{result?.Id}");
        //    }
        //    catch
        //    {                 
        //    }
        //}

        //TODO: TEST Role added to the user when he created a tenant

        [Fact]
        public async Task Post_CurrentMeUser_Tenant_NoAuth_401()
        {
            //Arrange

            //Act
            var response = await _client.PostAsJsonAsync($"{_baseUrlForV1}/tenants",
                new AddTenantCommand
                {
                    Code = "TestTenant",
                    Description = "TestTenant",
                    Label = "TestTenant",
                });

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Post_CurrentMeUser_Tenant_AlreadyExists_409()
        {             //Arrange
            var token = await GetAccessTokenAsync(TokenType.RW);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.PostAsJsonAsync($"{_baseUrlForV1}/tenants",
                new AddTenantCommand
                {
                    Code = "ubik_tenant_test_2",
                    Description = "TestTenant",
                    Label = "TestTenant",
                });

            var result = await response.Content.ReadFromJsonAsync<CustomProblemDetails>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Conflict);
            result.Should()
                .NotBeNull()
                .And.BeOfType<CustomProblemDetails>()
                .And.Match<CustomProblemDetails>(x => x.Errors.First().Code == "TENANT_ALREADY_EXISTS");
        }
    }
}
