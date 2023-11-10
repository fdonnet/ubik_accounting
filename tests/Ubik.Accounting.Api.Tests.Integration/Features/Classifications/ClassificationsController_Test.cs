using System.Net.Http.Headers;
using System.Net;
using Ubik.Accounting.Api.Data.Init;
using Ubik.Accounting.Api.Tests.Integration.Auth;
using FluentAssertions;
using System.Net.Http.Json;
using Ubik.Accounting.Contracts.Classifications.Results;
using Ubik.ApiService.Common.Exceptions;

namespace Ubik.Accounting.Api.Tests.Integration.Features.Classifications
{
    public class ClassificationsController_Test : BaseIntegrationTest
    {
        private readonly BaseValuesForClassifications _testValuesForClassifications;
        private readonly string _baseUrlForV1;

        public ClassificationsController_Test(IntegrationTestWebAppFactory factory) : base(factory)
        {
            _testValuesForClassifications = new BaseValuesForClassifications();
            _baseUrlForV1 = "/api/v1/Classifications";
        }

        [Fact]
        public async Task CheckAuth_401_NoAuth()
        {
            //Arrange
            var httpClient = Factory.CreateDefaultClient();

            //Act
            var responseGetAll = await httpClient.GetAsync(_baseUrlForV1);
            var responseGet = await httpClient.GetAsync($"{_baseUrlForV1}/{_testValuesForClassifications.ClassificationId1}");
            var responseGetAccounts = await httpClient.GetAsync($"{_baseUrlForV1}/{_testValuesForClassifications.ClassificationId1}/Accounts");


            //Assert
            responseGetAll.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
            responseGet.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
            responseGetAccounts.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
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
            var responseGet = await httpClient.GetAsync($"{_baseUrlForV1}/{_testValuesForClassifications.ClassificationId1}");
            var responseGetAccounts = await httpClient.GetAsync($"{_baseUrlForV1}/{_testValuesForClassifications.ClassificationId1}/Accounts");


            //Assert
            responseGetAll.StatusCode.Should().Be(HttpStatusCode.Forbidden);
            responseGet.StatusCode.Should().Be(HttpStatusCode.Forbidden);
            responseGetAccounts.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task GetAll_Classifications_Ok()
        {
            //Arrange
            var httpClient = Factory.CreateDefaultClient();

            var accessToken = await AuthHelper.GetAccessTokenReadOnly();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            //Act
            var response = await httpClient.GetAsync(_baseUrlForV1);
            var result = await response.Content.ReadFromJsonAsync<IEnumerable<GetAllClassificationsResult>>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Should()
                .NotBeNull()
                .And.AllBeOfType<GetAllClassificationsResult>(); ;
        }

        [Fact]
        public async Task Get_Classification_Ok()
        {
            //Arrange
            var httpClient = Factory.CreateDefaultClient();

            var accessToken = await AuthHelper.GetAccessTokenReadOnly();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            //Act
            var response = await httpClient.GetAsync($"{_baseUrlForV1}/{_testValuesForClassifications.ClassificationId1}");
            var result = await response.Content.ReadFromJsonAsync<GetClassificationResult>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Should()
                .NotBeNull()
                .And.BeOfType<GetClassificationResult>()
                .And.Match<GetClassificationResult>(x => x.Code == _testValuesForClassifications.ClassificationCode1);
        }

        [Fact]
        public async Task Get_ProblemDetails_IdNotFound()
        {
            //Arrange
            var httpClient = Factory.CreateDefaultClient();

            var accessToken = await AuthHelper.GetAccessTokenReadOnly();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            //Act
            var response = await httpClient.GetAsync($"{_baseUrlForV1}/{Guid.NewGuid()}");
            var responseAccounts = await httpClient.GetAsync($"{_baseUrlForV1}/{Guid.NewGuid()}/Accounts");

            var result = await response.Content.ReadFromJsonAsync<CustomProblemDetails>();
            var resultAccounts = await responseAccounts.Content.ReadFromJsonAsync<CustomProblemDetails>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            result.Should()
                .NotBeNull()
                .And.BeOfType<CustomProblemDetails>()
                .And.Match<CustomProblemDetails>(x => x.Errors.First().Code == "CLASSIFICATION_NOT_FOUND");
        }

        [Fact]
        public async Task GetAccounts_Accounts_Ok()
        {
            //Arrange
            var httpClient = Factory.CreateDefaultClient();

            var accessToken = await AuthHelper.GetAccessTokenReadOnly();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            //Act
            var response = await httpClient.GetAsync($"{_baseUrlForV1}/{_testValuesForClassifications.ClassificationId1}/Accounts");
            var result = await response.Content.ReadFromJsonAsync<IEnumerable<GetClassificationAccountsResult>>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Should()
                .NotBeNull()
                .And.AllBeOfType<GetClassificationAccountsResult>(); ;
        }
    }
}
