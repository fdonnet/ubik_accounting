using FluentAssertions;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Ubik.Accounting.Api.Data.Init;
using Ubik.Api.Tests.Integration.Auth;
using Ubik.Accounting.Contracts.Currencies.Results;

namespace Ubik.Api.Tests.Integration.Features.Accounting.Currencies
{
    public class CurrenciesController_Test : BaseIntegrationTest
    {
        private readonly BaseValuesForCurrencies _testValuesForCurrencies;
        private readonly string _baseUrlForV1;

        public CurrenciesController_Test(IntegrationTestAccoutingFactory factory) : base(factory)
        {
            _testValuesForCurrencies = new BaseValuesForCurrencies();
            _baseUrlForV1 = "/api/v1/Currencies";
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
        [Fact]
        public async Task CheckHasRoleForAccount_403_NoRole()
        {
            //Arrange
            var httpClient = Factory.CreateDefaultClient();

            var accessToken = await AuthHelper.GetAccessTokenNoRole();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            //Act
            var responseGetAll = await httpClient.GetAsync(_baseUrlForV1);

            //Assert
            responseGetAll.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task GetAll_Currencies_Ok()
        {
            //Arrange
            var httpClient = Factory.CreateDefaultClient();

            var accessToken = await AuthHelper.GetAccessTokenReadOnly();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            //Act
            var response = await httpClient.GetAsync(_baseUrlForV1);
            var result = await response.Content.ReadFromJsonAsync<IEnumerable<GetAllCurrenciesResult>>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Should()
                .NotBeNull()
                .And.AllBeOfType<GetAllCurrenciesResult>(); ;
        }

    }
}
