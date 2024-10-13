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

        //TODO:
        //- Test with Token "noRole" and "RO" and MegaAdmin
        //- Test with no auth
        //- Test with user not found
        //- Test with user ok but not in the tenant


        [Fact]
        public async Task Get_User_OK()
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
    }
}
