using FluentAssertions;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net;
using System.Text;
using Ubik.Accounting.Api.Data.Init;
using Ubik.Accounting.Api.Tests.Integration.Auth;
using Bogus;
using Ubik.ApiService.Common.Exceptions;
using System.Text.Json;
using Ubik.Accounting.Contracts.Accounts.Results;
using MassTransit;
using Ubik.Accounting.Contracts.AccountGroups.Results;

namespace Ubik.Accounting.Api.Tests.Integration.Features.AccountGroups
{
    public class AccountGroupsController_Test : BaseIntegrationTest
    {
        private readonly BaseValuesForAccountGroups _testValuesForAccountGroups;
        private readonly BaseValuesForClassifications _testValuesForAccountGroupClassifications;
        private readonly string _baseUrlForV1;
        
        public AccountGroupsController_Test(IntegrationTestWebAppFactory factory) : base(factory)
        {
            _testValuesForAccountGroups = new BaseValuesForAccountGroups();
            _testValuesForAccountGroupClassifications = new BaseValuesForClassifications();
            _baseUrlForV1 = "/api/v1/AccountGroups";
        }

        [Fact]
        public async Task CheckAuth_401_NoAuth()
        {
            //Arrange
            var httpClient = Factory.CreateDefaultClient();

            //Act
            var responseGetAll = await httpClient.GetAsync(_baseUrlForV1);
            var responseGet = await httpClient.GetAsync($"{_baseUrlForV1}/{_testValuesForAccountGroups.AccountGroupId1}");
            var responseGetChildAccounts = await httpClient.GetAsync($"{_baseUrlForV1}/{_testValuesForAccountGroups.AccountGroupId1}/Accounts");
            var responsePost = await httpClient.PostAsync(_baseUrlForV1, 
                new StringContent("test", Encoding.UTF8, "application/json"));

            var responsePut = await httpClient.PutAsync($"{_baseUrlForV1}/{_testValuesForAccountGroups.AccountGroupId1}", 
                new StringContent("test", Encoding.UTF8, "application/json"));

            var responseDel = await httpClient.DeleteAsync($"{_baseUrlForV1}/{_testValuesForAccountGroups.AccountGroupId1}");

            //Assert
            responseGetAll.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
            responseGetChildAccounts.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
            responseGet.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
            responsePost.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
            responsePut.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
            responseDel.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
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
            var responseGet = await httpClient.GetAsync($"{_baseUrlForV1}/{_testValuesForAccountGroups.AccountGroupId1}");
            var responseGetChildAccounts = await httpClient.GetAsync($"{_baseUrlForV1}/{_testValuesForAccountGroups.AccountGroupId1}/Accounts");
            var responsePost = await httpClient.PostAsync(_baseUrlForV1, 
                new StringContent("test", Encoding.UTF8, "application/json"));

            var responsePut = await httpClient.PutAsync($"{_baseUrlForV1}/{_testValuesForAccountGroups.AccountGroupId1}",
                new StringContent("test", Encoding.UTF8, "application/json"));

            var responseDel = await httpClient.DeleteAsync($"{_baseUrlForV1}/{_testValuesForAccountGroups.AccountGroupId1}");

            //Assert
            responseGetAll.StatusCode.Should().Be(HttpStatusCode.Forbidden);
            responseGetChildAccounts.StatusCode.Should().Be(HttpStatusCode.Forbidden);
            responseGet.StatusCode.Should().Be(HttpStatusCode.Forbidden);
            responsePost.StatusCode.Should().Be(HttpStatusCode.Forbidden);
            responsePut.StatusCode.Should().Be(HttpStatusCode.Forbidden);
            responseDel.StatusCode.Should().Be(HttpStatusCode.Forbidden);
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
            var responsePut = await httpClient.PutAsync($"{_baseUrlForV1}/{_testValuesForAccountGroups.AccountGroupId1}", new StringContent("test", Encoding.UTF8, "application/json"));
            var responseDel = await httpClient.DeleteAsync($"{_baseUrlForV1}/{_testValuesForAccountGroups.AccountGroupId1}");

            //Assert
            responsePost.StatusCode.Should().Be(HttpStatusCode.Forbidden);
            responsePut.StatusCode.Should().Be(HttpStatusCode.Forbidden);
            responseDel.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task GetAll_AccountGroups_Ok()
        {
            //Arrange
           var httpClient = Factory.CreateDefaultClient();

            var accessToken = await AuthHelper.GetAccessTokenReadOnly();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            //Act
            var response = await httpClient.GetAsync(_baseUrlForV1);
            var result = await response.Content.ReadFromJsonAsync<IEnumerable<GetAllAccountGroupsResult>>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Should()
                .NotBeNull()
                .And.AllBeOfType<GetAllAccountGroupsResult>(); ;
        }

        [Fact]
        public async Task Get_AccountGroup_Ok()
        {
            //Arrange
            var httpClient = Factory.CreateDefaultClient();

            var accessToken = await AuthHelper.GetAccessTokenReadOnly();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            //Act
            var response = await httpClient.GetAsync($"{_baseUrlForV1}/{_testValuesForAccountGroups.AccountGroupId1}");
            var result = await response.Content.ReadFromJsonAsync<GetAccountGroupResult>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Should()
                .NotBeNull()
                .And.BeOfType<GetAccountGroupResult>()
                .And.Match<GetAccountGroupResult>(x => x.Code == "100");
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
            var result = await response.Content.ReadFromJsonAsync<CustomProblemDetails>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            result.Should()
                .NotBeNull()
                .And.BeOfType<CustomProblemDetails>()
                .And.Match<CustomProblemDetails>(x => x.Errors.First().Code == "ACCOUNTGROUP_NOT_FOUND");
        }

        [Fact]
        public async Task GetChildAccounts_Accounts_Ok()
        {
            //Arrange
            var httpClient = Factory.CreateDefaultClient();

            var accessToken = await AuthHelper.GetAccessTokenReadOnly();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            //Act
            var response = await httpClient.GetAsync($"{_baseUrlForV1}/{_testValuesForAccountGroups.AccountGroupId1}/Accounts");
            var result = await response.Content.ReadFromJsonAsync<IEnumerable<GetChildAccountsResult>>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Should()
                .NotBeNull()
                .And.AllBeOfType<GetChildAccountsResult>();
        }

        [Fact]
        public async Task GetChildAccounts_ProblemDetails_AccountGroupNotFound()
        {
            //Arrange
            var httpClient = Factory.CreateDefaultClient();

            var accessToken = await AuthHelper.GetAccessTokenReadOnly();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            //Act
            var response = await httpClient.GetAsync($"{_baseUrlForV1}/{Guid.NewGuid()}/Accounts");
            var result = await response.Content.ReadFromJsonAsync<CustomProblemDetails>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            result.Should()
                .NotBeNull()
                .And.BeOfType<CustomProblemDetails>()
                .And.Match<CustomProblemDetails>(x => x.Errors.First().Code == "ACCOUNTGROUP_NOT_FOUND");
        }

        [Fact]
        public async Task Post_AccountGroup_Ok()
        {
            //Arrange
            var httpClient = Factory.CreateDefaultClient();

            var accessToken = await AuthHelper.GetAccessTokenReadWrite();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            //Act
            var fake = FakeGenerator.GenerateAddAccountGroups(1).First();
            var postAccountJson = JsonSerializer.Serialize(fake);
            var content = new StringContent(postAccountJson.ToString(), Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync(_baseUrlForV1, content);
            var result = await response.Content.ReadFromJsonAsync<AddAccountGroupResult>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            result.Should()
                .NotBeNull()
                .And.BeOfType<AddAccountGroupResult>()
                .And.Match<AddAccountGroupResult>(x =>
                    x.Code == fake.Code
                    && x.Label == fake.Label
                    && x.Description == fake.Description
                    && x.Version != default!);
        }

        [Fact]
        public async Task Post_AccountGroup_OkCodeExistButInOtherClassification()
        {
            //Arrange
            var httpClient = Factory.CreateDefaultClient();

            var accessToken = await AuthHelper.GetAccessTokenReadWrite();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            //Act
            var fake = FakeGenerator.GenerateAddAccountGroups(1,
                code: _testValuesForAccountGroups.AccountGroupCode1,
                accountGroupClassificationId: _testValuesForAccountGroupClassifications.ClassificationId2).First();

            var postAccountJson = JsonSerializer.Serialize(fake);
            var content = new StringContent(postAccountJson.ToString(), Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync(_baseUrlForV1, content);
            var result = await response.Content.ReadFromJsonAsync<AddAccountGroupResult>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            result.Should()
                .NotBeNull()
                .And.BeOfType<AddAccountGroupResult>()
                .And.Match<AddAccountGroupResult>(x =>
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
            var fake = FakeGenerator.GenerateAddAccountGroups(1, code: _testValuesForAccountGroups.AccountGroupCode1).First();

            var postAccountJson = JsonSerializer.Serialize(fake);
            var content = new StringContent(postAccountJson.ToString(), Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync(_baseUrlForV1, content);
            var result = await response.Content.ReadFromJsonAsync<CustomProblemDetails>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Conflict);
            result.Should()
                .NotBeNull()
                .And.BeOfType<CustomProblemDetails>()
                .And.Match<CustomProblemDetails>(x => x.Errors.First().Code == "ACCOUNTGROUP_IN_CLASSIFICATION_ALREADY_EXISTS");
        }

        [Fact]
        public async Task Post_ProblemDetails_ClassificationNotFound()
        {
            //Arrange
            var httpClient = Factory.CreateDefaultClient();

            var accessToken = await AuthHelper.GetAccessTokenReadWrite();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            //Act
            var fake = FakeGenerator.GenerateAddAccountGroups(1, accountGroupClassificationId: Guid.NewGuid()).First();

            var postAccountJson = JsonSerializer.Serialize(fake);
            var content = new StringContent(postAccountJson.ToString(), Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync(_baseUrlForV1, content);
            var result = await response.Content.ReadFromJsonAsync<CustomProblemDetails>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            result.Should()
                .NotBeNull()
                .And.BeOfType<CustomProblemDetails>()
                .And.Match<CustomProblemDetails>(x => x.Errors.First().Code == "CLASSIFICATION_NOT_FOUND");
        }

        [Fact]
        public async Task Post_ProblemDetails_EmptyFields()
        {
            //Arrange
            var httpClient = Factory.CreateDefaultClient();

            var accessToken = await AuthHelper.GetAccessTokenReadWrite();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            //Act
            var fake = FakeGenerator.GenerateAddAccountGroups(1, code:string.Empty,label:string.Empty).First();

            var postAccountJson = JsonSerializer.Serialize(fake);
            var content = new StringContent(postAccountJson.ToString(), Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync(_baseUrlForV1, content);
            var result = await response.Content.ReadFromJsonAsync<CustomProblemDetails>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            result.Should()
                .NotBeNull()
                .And.BeOfType<CustomProblemDetails>()
                .And.Match<CustomProblemDetails>(x => x.Errors.First().Code == "VALIDATION_ERROR" && x.Errors.Count() == 2);
        }

        [Fact]
        public async Task Post_ProblemDetails_TooLongFields()
        {
            //Arrange
            var httpClient = Factory.CreateDefaultClient();

            var accessToken = await AuthHelper.GetAccessTokenReadWrite();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            //Act
            var fake = FakeGenerator.GenerateAddAccountGroups(1,
                code: new string(new Faker("fr_CH").Random.Chars(count: 21)),
                label: new string(new Faker("fr_CH").Random.Chars(count: 101)),
                description: new string(new Faker("fr_CH").Random.Chars(count: 701))).First();

            var postAccountJson = JsonSerializer.Serialize(fake);
            var content = new StringContent(postAccountJson.ToString(), Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync(_baseUrlForV1, content);
            var result = await response.Content.ReadFromJsonAsync<CustomProblemDetails>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            result.Should()
                .NotBeNull()
                .And.BeOfType<CustomProblemDetails>()
                .And.Match<CustomProblemDetails>(x => x.Errors.First().Code == "VALIDATION_ERROR" && x.Errors.Count() == 3);
        }

        [Fact]
        public async Task Put_AccountGroup_Ok()
        {
            //Arrange
            var httpClient = Factory.CreateDefaultClient();

            var accessToken = await AuthHelper.GetAccessTokenReadWrite();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            //Act
            var responseGet = await httpClient.GetAsync($"{_baseUrlForV1}/{_testValuesForAccountGroups.AccountGroupId1}");
            var resultGet = await responseGet.Content.ReadFromJsonAsync<GetAccountGroupResult>();

            var fake = FakeGenerator.GenerateUpdAccountGroups(1,
                version: resultGet!.Version,
                id: resultGet.Id,
                code: _testValuesForAccountGroups.AccountGroupCode1).First();

            var postAccountJson = JsonSerializer.Serialize(fake);
            var content = new StringContent(postAccountJson.ToString(), Encoding.UTF8, "application/json");

            var response = await httpClient.PutAsync($"{_baseUrlForV1}/{_testValuesForAccountGroups.AccountGroupId1}", content);
            var result = await response.Content.ReadFromJsonAsync<UpdateAccountGroupResult>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Should()
                .NotBeNull()
                .And.BeOfType<UpdateAccountGroupResult>()
                .And.Match<UpdateAccountGroupResult>(x =>
                    x.Code == fake.Code
                    && x.Label == fake.Label
                    && x.Description == fake.Description
                    && x.Version != fake.Version);
        }

        [Fact]
        public async Task Put_ProblemDetails_ClassificationNotFound()
        {
            //Arrange
            var httpClient = Factory.CreateDefaultClient();

            var accessToken = await AuthHelper.GetAccessTokenReadWrite();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            //Act
            var responseGet = await httpClient.GetAsync($"{_baseUrlForV1}/{_testValuesForAccountGroups.AccountGroupId1}");
            var resultGet = await responseGet.Content.ReadFromJsonAsync<GetAccountGroupResult>();

            var fake = FakeGenerator.GenerateUpdAccountGroups(1,
                version: resultGet!.Version,
                id: resultGet.Id,
                code: resultGet.Code,
                accountGroupClassificationId: Guid.NewGuid()).First();

            var postAccountJson = JsonSerializer.Serialize(fake);
            var content = new StringContent(postAccountJson.ToString(), Encoding.UTF8, "application/json");

            var response = await httpClient.PutAsync($"{_baseUrlForV1}/{_testValuesForAccountGroups.AccountGroupId1}", content);
            var result = await response.Content.ReadFromJsonAsync<CustomProblemDetails>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            result.Should()
                .NotBeNull()
                .And.BeOfType<CustomProblemDetails>()
                .And.Match<CustomProblemDetails>(x => x.Errors.First().Code == "CLASSIFICATION_NOT_FOUND");
        }

        [Fact]
        public async Task Put_ProblemDetails_QueryIdAndCommandIdNotMatch()
        {
            //Arrange
            var httpClient = Factory.CreateDefaultClient();

            var accessToken = await AuthHelper.GetAccessTokenReadWrite();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            //Act
            var responseGet = await httpClient.GetAsync($"{_baseUrlForV1}/{_testValuesForAccountGroups.AccountGroupId1}");
            var resultGet = await responseGet.Content.ReadFromJsonAsync<GetAccountGroupResult>();

            var fake = FakeGenerator.GenerateUpdAccountGroups(1,
                version: resultGet!.Version,
                id: Guid.NewGuid(),
                code: resultGet.Code,
                accountGroupClassificationId: Guid.NewGuid()).First();

            var postAccountJson = JsonSerializer.Serialize(fake);
            var content = new StringContent(postAccountJson.ToString(), Encoding.UTF8, "application/json");

            var response = await httpClient.PutAsync($"{_baseUrlForV1}/{_testValuesForAccountGroups.AccountGroupId1}", content);
            var result = await response.Content.ReadFromJsonAsync<CustomProblemDetails>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            result.Should()
                .NotBeNull()
                .And.BeOfType<CustomProblemDetails>()
                .And.Match<CustomProblemDetails>(x => x.Errors.First().Code == "ACCOUNTGROUP_UPDATE_IDS_NOT_MATCH");
        }

        [Fact]
        public async Task Put_ProblemDetails_EmptyFields()
        {
            //Arrange
            var httpClient = Factory.CreateDefaultClient();

            var accessToken = await AuthHelper.GetAccessTokenReadWrite();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            //Act
            var fake = FakeGenerator.GenerateUpdAccountGroups(1, code: string.Empty, label:string.Empty).First();

            var responseGet = await httpClient.GetAsync($"{_baseUrlForV1}/{_testValuesForAccountGroups.AccountGroupId1}");
            var resultGet = await responseGet.Content.ReadFromJsonAsync<GetAccountResult>();

            var postAccountJson = JsonSerializer.Serialize(fake);
            var content = new StringContent(postAccountJson.ToString(), Encoding.UTF8, "application/json");

            var response = await httpClient.PutAsync($"{_baseUrlForV1}/{_testValuesForAccountGroups.AccountGroupId1}", content);
            var result = await response.Content.ReadFromJsonAsync<CustomProblemDetails>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            result.Should()
                .NotBeNull()
                .And.BeOfType<CustomProblemDetails>()
                .And.Match<CustomProblemDetails>(x => x.Errors.First().Code == "VALIDATION_ERROR" && x.Errors.Count() == 2);
        }

        [Fact]
        public async Task Put_ProblemDetails_TooLongFields()
        {
            //Arrange
            var httpClient = Factory.CreateDefaultClient();

            var accessToken = await AuthHelper.GetAccessTokenReadWrite();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            //Act
            var responseGet = await httpClient.GetAsync($"{_baseUrlForV1}/{_testValuesForAccountGroups.AccountGroupId1}");
            var resultGet = await responseGet.Content.ReadFromJsonAsync<GetAccountResult>();

            var fake = FakeGenerator.GenerateUpdAccountGroups(1,
                code: new string(new Faker("fr_CH").Random.Chars(count: 21)),
                label: new string(new Faker("fr_CH").Random.Chars(count: 101)),
                description: new string(new Faker("fr_CH").Random.Chars(count: 701)),
                version: resultGet!.Version).First();

            var postAccountJson = JsonSerializer.Serialize(fake);
            var content = new StringContent(postAccountJson.ToString(), Encoding.UTF8, "application/json");

            var response = await httpClient.PutAsync($"{_baseUrlForV1}/{_testValuesForAccountGroups.AccountGroupId1}", content);
            var result = await response.Content.ReadFromJsonAsync<CustomProblemDetails>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            result.Should()
                .NotBeNull()
                .And.BeOfType<CustomProblemDetails>()
                .And.Match<CustomProblemDetails>(x => x.Errors.First().Code == "VALIDATION_ERROR" && x.Errors.Count() == 3);
        }

        [Fact]
        public async Task Put_ProblemDetails_CodeExistsWithDifferentId()
        {
            //Arrange
            var httpClient = Factory.CreateDefaultClient();

            var accessToken = await AuthHelper.GetAccessTokenReadWrite();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            //Act
            var responseGet = await httpClient.GetAsync($"{_baseUrlForV1}/{_testValuesForAccountGroups.AccountGroupId2}");
            var resultGet = await responseGet.Content.ReadFromJsonAsync<GetAccountResult>();

            var fake = FakeGenerator.GenerateUpdAccountGroups(1,
                id: resultGet!.Id,
                code: "10", 
                version: resultGet!.Version).First();
           
            var postAccountJson = JsonSerializer.Serialize(fake);
            var content = new StringContent(postAccountJson.ToString(), Encoding.UTF8, "application/json");

            var response = await httpClient.PutAsync($"{_baseUrlForV1}/{_testValuesForAccountGroups.AccountGroupId2}", content);
            var result = await response.Content.ReadFromJsonAsync<CustomProblemDetails>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Conflict);
            result.Should()
                .NotBeNull()
                .And.BeOfType<CustomProblemDetails>()
                .And.Match<CustomProblemDetails>(x => x.Errors.First().Code == "ACCOUNTGROUP_IN_CLASSIFICATION_ALREADY_EXISTS");
        }

        [Fact]
        public async Task Put_ProblemDetails_IdNotFound()
        {
            //Arrange
            var httpClient = Factory.CreateDefaultClient();

            var accessToken = await AuthHelper.GetAccessTokenReadWrite();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            //Act
            var responseGet = await httpClient.GetAsync($"{_baseUrlForV1}/{_testValuesForAccountGroups.AccountGroupId2}");
            var resultGet = await responseGet.Content.ReadFromJsonAsync<GetAccountResult>();

            var fake = FakeGenerator.GenerateUpdAccountGroups(1,
                id: NewId.NextGuid(),
                version: resultGet!.Version).First();

            var postAccountJson = JsonSerializer.Serialize(fake);
            var content = new StringContent(postAccountJson.ToString(), Encoding.UTF8, "application/json");

            var response = await httpClient.PutAsync($"{_baseUrlForV1}/{fake.Id}", content);
            var result = await response.Content.ReadFromJsonAsync<CustomProblemDetails>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            result.Should()
                .NotBeNull()
                .And.BeOfType<CustomProblemDetails>()
                .And.Match<CustomProblemDetails>(x => x.Errors.First().Code == "ACCOUNTGROUP_NOT_FOUND");
        }

        [Fact]
        public async Task Put_ProblemDetails_ModifiedByAnotherProcess()
        {
            //Arrange
            var httpClient = Factory.CreateDefaultClient();

            var accessToken = await AuthHelper.GetAccessTokenReadWrite();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            //Act
            var responseGet = await httpClient.GetAsync($"{_baseUrlForV1}/{_testValuesForAccountGroups.AccountGroupId1}");
            var resultGet = await responseGet.Content.ReadFromJsonAsync<GetAccountResult>();

            var fake = FakeGenerator.GenerateUpdAccountGroups(1,
                version: resultGet!.Version,
                id: resultGet!.Id,
                code: _testValuesForAccountGroups.AccountGroupCode1).First();

            var postAccountJson = JsonSerializer.Serialize(fake);
            var content = new StringContent(postAccountJson.ToString(), Encoding.UTF8, "application/json");

            var response = await httpClient.PutAsync($"{_baseUrlForV1}/{_testValuesForAccountGroups.AccountGroupId1}", content);
            var response2 = await httpClient.PutAsync($"{_baseUrlForV1}/{_testValuesForAccountGroups.AccountGroupId1}", content);
            var result = await response2.Content.ReadFromJsonAsync<CustomProblemDetails>();

            //Assert
            response2.StatusCode.Should().Be(HttpStatusCode.Conflict);
            result.Should()
                .NotBeNull()
                .And.BeOfType<CustomProblemDetails>()
                .And.Match<CustomProblemDetails>(x => x.Errors.First().Code == "ACCOUNTGROUP_UPDATE_CONCURRENCY");
        }

        [Fact]
        public async Task Del_AccoundGroupResult_Ok()
        {
            //Arrange
            var httpClient = Factory.CreateDefaultClient();

            var accessToken = await AuthHelper.GetAccessTokenReadWrite();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            //Act
            var response = await httpClient.DeleteAsync($"{_baseUrlForV1}/{_testValuesForAccountGroups.AccountGroupIdForDel2}");
            var result = await response.Content.ReadFromJsonAsync<IEnumerable<DeleteAccountGroupResult>>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Should()
                .NotBeNull()
                .And.AllBeOfType<DeleteAccountGroupResult>(); ;

        }

        [Fact]
        public async Task Del_ProblemDetails_IdNotFound()
        {
            //Arrange
            var httpClient = Factory.CreateDefaultClient();

            var accessToken = await AuthHelper.GetAccessTokenReadWrite();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            //Act
            var response = await httpClient.DeleteAsync($"{_baseUrlForV1}/{Guid.NewGuid()}");
            var result = await response.Content.ReadFromJsonAsync<CustomProblemDetails>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            result.Should()
                .NotBeNull()
                .And.BeOfType<CustomProblemDetails>()
                .And.Match<CustomProblemDetails>(x => x.Errors.First().Code == "ACCOUNTGROUP_NOT_FOUND");
        }    
    }
}
