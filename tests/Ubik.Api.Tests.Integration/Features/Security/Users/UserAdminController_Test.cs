using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Ubik.Accounting.Api.Data.Init;

namespace Ubik.Api.Tests.Integration.Features.Security.Users
{
    public class UserAdminController_Test : BaseIntegrationTest
    {
        private readonly string _baseUrlForV1;

        public UserAdminController_Test(IntegrationTestProxyFactory factory) : base(factory)
        {
            _baseUrlForV1 = "/usrmgt/admin/api/v1/users";
        }

        [Fact]
        public async Task CheckAuth_401_NoAuth()
        {
            //Arrange
            var httpClient = Factory.CreateDefaultClient();

            //Act
            var responseGetAll = await httpClient.GetAsync(_baseUrlForV1);
            

            //Assert
            responseGetAll.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

    }
}
