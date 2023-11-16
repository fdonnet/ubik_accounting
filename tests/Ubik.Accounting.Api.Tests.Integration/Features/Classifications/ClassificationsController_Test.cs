using System.Net.Http.Headers;
using System.Net;
using Ubik.Accounting.Api.Data.Init;
using Ubik.Accounting.Api.Tests.Integration.Auth;
using FluentAssertions;
using System.Net.Http.Json;
using Ubik.Accounting.Contracts.Classifications.Results;
using Ubik.ApiService.Common.Exceptions;
using System.Text;
using System.Text.Json;
using Ubik.Accounting.Contracts.Classifications.Commands;

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
            var responsePost = await httpClient.PostAsync(_baseUrlForV1,
                new StringContent("test", Encoding.UTF8, "application/json"));

            var responseGet = await httpClient.GetAsync($"{_baseUrlForV1}/{_testValuesForClassifications.ClassificationId1}");
            var responseGetAccounts = await httpClient.GetAsync($"{_baseUrlForV1}/{_testValuesForClassifications.ClassificationId1}/Accounts");
            var responseGetMissingAccounts = await httpClient.GetAsync($"{_baseUrlForV1}/{_testValuesForClassifications.ClassificationId1}/MissingAccounts");
            var responseGetStatus = await httpClient.GetAsync($"{_baseUrlForV1}/{_testValuesForClassifications.ClassificationId1}/Status");


            //Assert
            responseGetAll.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
            responsePost.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
            responseGet.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
            responseGetAccounts.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
            responseGetMissingAccounts.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
            responseGetStatus.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
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
            var responsePost = await httpClient.PostAsync(_baseUrlForV1,
                new StringContent("test", Encoding.UTF8, "application/json"));

            var responseGet = await httpClient.GetAsync($"{_baseUrlForV1}/{_testValuesForClassifications.ClassificationId1}");
            var responseGetAccounts = await httpClient.GetAsync($"{_baseUrlForV1}/{_testValuesForClassifications.ClassificationId1}/Accounts");
            var responseGetMissingAccounts = await httpClient.GetAsync($"{_baseUrlForV1}/{_testValuesForClassifications.ClassificationId1}/MissingAccounts");
            var responseGetStatus = await httpClient.GetAsync($"{_baseUrlForV1}/{_testValuesForClassifications.ClassificationId1}/Status");


            //Assert
            responseGetAll.StatusCode.Should().Be(HttpStatusCode.Forbidden);
            responsePost.StatusCode.Should().Be(HttpStatusCode.Forbidden);
            responseGet.StatusCode.Should().Be(HttpStatusCode.Forbidden);
            responseGetAccounts.StatusCode.Should().Be(HttpStatusCode.Forbidden);
            responseGetMissingAccounts.StatusCode.Should().Be(HttpStatusCode.Forbidden);
            responseGetStatus.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task CheckHasWriteRoleForAccount_403_NoWriteRole()
        {
            //Arrange
            var httpClient = Factory.CreateDefaultClient();

            var accessToken = await AuthHelper.GetAccessTokenReadOnly();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            //Act
            var responsePost = await httpClient.PostAsync(_baseUrlForV1, new StringContent("test", Encoding.UTF8, "application/json"));
            //var responsePut = await httpClient.PutAsync($"{_baseUrlForV1}/{_testValuesForAccountGroups.AccountGroupId1}", new StringContent("test", Encoding.UTF8, "application/json"));
            //var responseDel = await httpClient.DeleteAsync($"{_baseUrlForV1}/{_testValuesForAccountGroups.AccountGroupId1}");

            //Assert
            responsePost.StatusCode.Should().Be(HttpStatusCode.Forbidden);
            //responsePut.StatusCode.Should().Be(HttpStatusCode.Forbidden);
            //responseDel.StatusCode.Should().Be(HttpStatusCode.Forbidden);
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
            var responseMissingAccounts = await httpClient.GetAsync($"{_baseUrlForV1}/{Guid.NewGuid()}/MissingAccounts");
            var responseStatus = await httpClient.GetAsync($"{_baseUrlForV1}/{Guid.NewGuid()}/Status");

            var result = await response.Content.ReadFromJsonAsync<CustomProblemDetails>();
            var resultAccounts = await responseAccounts.Content.ReadFromJsonAsync<CustomProblemDetails>();
            var resultMissingAccounts = await responseMissingAccounts.Content.ReadFromJsonAsync<CustomProblemDetails>();
            var resultStatus = await responseStatus.Content.ReadFromJsonAsync<CustomProblemDetails>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            responseAccounts.StatusCode.Should().Be(HttpStatusCode.NotFound);
            responseMissingAccounts.StatusCode.Should().Be(HttpStatusCode.NotFound);
            responseStatus.StatusCode.Should().Be(HttpStatusCode.NotFound);

            result.Should()
                .NotBeNull()
                .And.BeOfType<CustomProblemDetails>()
                .And.Match<CustomProblemDetails>(x => x.Errors.First().Code == "CLASSIFICATION_NOT_FOUND");

            resultAccounts.Should()
                .NotBeNull()
                .And.BeOfType<CustomProblemDetails>()
                .And.Match<CustomProblemDetails>(x => x.Errors.First().Code == "CLASSIFICATION_NOT_FOUND");

            resultMissingAccounts.Should()
                .NotBeNull()
                .And.BeOfType<CustomProblemDetails>()
                .And.Match<CustomProblemDetails>(x => x.Errors.First().Code == "CLASSIFICATION_NOT_FOUND");

            resultStatus.Should()
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

        [Fact]
        public async Task GetMissingAccounts_Accounts_Ok()
        {
            //Arrange
            var httpClient = Factory.CreateDefaultClient();

            var accessToken = await AuthHelper.GetAccessTokenReadOnly();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            //Act
            var response = await httpClient.GetAsync($"{_baseUrlForV1}/{_testValuesForClassifications.ClassificationId1}/MissingAccounts");
            var result = await response.Content.ReadFromJsonAsync<IEnumerable<GetClassificationAccountsMissingResult>>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Should()
                .NotBeNull()
                .And.AllBeOfType<GetClassificationAccountsMissingResult>(); ;
        }

        [Fact]
        public async Task GetStatus_ClassificationStatus_Ok()
        {
            //Arrange
            var httpClient = Factory.CreateDefaultClient();

            var accessToken = await AuthHelper.GetAccessTokenReadOnly();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            //Act
            var response = await httpClient.GetAsync($"{_baseUrlForV1}/{_testValuesForClassifications.ClassificationId1}/Status");
            var result = await response.Content.ReadFromJsonAsync<GetClassificationStatusResult>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Should()
                .NotBeNull()
                .And.BeOfType<GetClassificationStatusResult>()
                .And.Match<GetClassificationStatusResult>(x => x.IsReady == false);
        }

        [Fact]
        public async Task Post_Classification_Ok()
        {
            //Arrange
            var httpClient = Factory.CreateDefaultClient();

            var accessToken = await AuthHelper.GetAccessTokenReadWrite();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            //Act
            var fake = new AddClassificationCommand { Code = "TEST", Label = "TEST" };
            var postAccountJson = JsonSerializer.Serialize(fake);
            var content = new StringContent(postAccountJson.ToString(), Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync(_baseUrlForV1, content);
            var result = await response.Content.ReadFromJsonAsync<AddClassificationResult>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            result.Should()
                .NotBeNull()
                .And.BeOfType<AddClassificationResult>()
                .And.Match<AddClassificationResult>(x =>
                    x.Code == fake.Code
                    && x.Label == fake.Label
                    && x.Description == fake.Description
                    && x.Version != default!);
        }

        [Fact]
        public async Task Post_ProblemDetails_CodeExist()
        {
            //Arrange
            var httpClient = Factory.CreateDefaultClient();

            var accessToken = await AuthHelper.GetAccessTokenReadWrite();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            //Act
            var fake = new AddClassificationCommand { Code = "SWISSPLAN", Label = "TEST" };

            var postAccountJson = JsonSerializer.Serialize(fake);
            var content = new StringContent(postAccountJson.ToString(), Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync(_baseUrlForV1, content);
            var result = await response.Content.ReadFromJsonAsync<CustomProblemDetails>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Conflict);
            result.Should()
                .NotBeNull()
                .And.BeOfType<CustomProblemDetails>()
                .And.Match<CustomProblemDetails>(x => x.Errors.First().Code == "CLASSIFICATION_ALREADY_EXISTS");
        }
    }
}
