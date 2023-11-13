using FluentAssertions;
using System.Net.Http.Json;
using System.Text;
using System.Net;
using Ubik.ApiService.Common.Exceptions;
using Bogus;
using System.Text.Json;
using System.Net.Http.Headers;
using Ubik.Accounting.Api.Tests.Integration.Auth;
using Ubik.Accounting.Api.Data.Init;
using Ubik.Accounting.Contracts.Accounts.Results;

namespace Ubik.Accounting.Api.Tests.Integration.Features.Accounts
{
    public class AccountsController_Test : BaseIntegrationTest
    {
        private readonly BaseValuesForAccounts _testValuesForAccounts;
        private readonly BaseValuesForAccountGroups _testValuesForAccountGroups;
        private readonly string _baseUrlForV1;

        public AccountsController_Test(IntegrationTestWebAppFactory factory) : base(factory)
        {
            _testValuesForAccounts = new BaseValuesForAccounts();
            _testValuesForAccountGroups = new BaseValuesForAccountGroups();
            _baseUrlForV1 = "/api/v1/Accounts";
        }

        [Fact]
        public async Task CheckAuth_401_NoAuth()
        {
            //Arrange
            var httpClient = Factory.CreateDefaultClient();

            //Act
            var responseGetAll = await httpClient.GetAsync(_baseUrlForV1);
            var responseGet = await httpClient.GetAsync($"{_baseUrlForV1}/{_testValuesForAccounts.AccountId1}");
            var responsePost = await httpClient.PostAsync
                (_baseUrlForV1, new StringContent("test", Encoding.UTF8, "application/json"));

            var responsePut = await httpClient.PutAsync
                ($"{_baseUrlForV1}/{_testValuesForAccounts.AccountId1}", new StringContent("test", Encoding.UTF8, "application/json"));

            var responseDel = await httpClient.DeleteAsync($"{_baseUrlForV1}/{_testValuesForAccounts.AccountId1}");
            var responseAddToGroup = await httpClient.PutAsync
                ($"{_baseUrlForV1}/{_testValuesForAccounts.AccountId2}/AccountGroups/{_testValuesForAccountGroups.AccountGroupId2}", null);

            var responseDelToGroup = await httpClient.DeleteAsync
                ($"{_baseUrlForV1}/{_testValuesForAccounts.AccountId2}/AccountGroups/{_testValuesForAccountGroups.AccountGroupId2}");

            //Assert
            responseGetAll.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
            responseGet.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
            responsePost.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
            responsePut.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
            responseDel.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
            responseAddToGroup.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
            responseDelToGroup.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
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
            var responseGet = await httpClient.GetAsync($"{_baseUrlForV1}/{_testValuesForAccounts.AccountId1}");
            var responsePost = await httpClient.PostAsync(_baseUrlForV1, new StringContent("test", Encoding.UTF8, "application/json"));
            var responsePut = await httpClient.PutAsync
                ($"{_baseUrlForV1}/{_testValuesForAccounts.AccountId1}", new StringContent("test", Encoding.UTF8, "application/json"));

            var responseDel = await httpClient.DeleteAsync($"{_baseUrlForV1}/{_testValuesForAccounts.AccountId1}");
            var responseAddToGroup = await httpClient.PutAsync
                ($"{_baseUrlForV1}/{_testValuesForAccounts.AccountId2}/AccountGroups/{_testValuesForAccountGroups.AccountGroupId2}", null);

            var responseDelToGroup = await httpClient.DeleteAsync
                ($"{_baseUrlForV1}/{_testValuesForAccounts.AccountId2}/AccountGroups/{_testValuesForAccountGroups.AccountGroupId2}");

            //Assert
            responseGetAll.StatusCode.Should().Be(HttpStatusCode.Forbidden);
            responseGet.StatusCode.Should().Be(HttpStatusCode.Forbidden);
            responsePost.StatusCode.Should().Be(HttpStatusCode.Forbidden);
            responsePut.StatusCode.Should().Be(HttpStatusCode.Forbidden);
            responseDel.StatusCode.Should().Be(HttpStatusCode.Forbidden);
            responseAddToGroup.StatusCode.Should().Be(HttpStatusCode.Forbidden);
            responseDelToGroup.StatusCode.Should().Be(HttpStatusCode.Forbidden);
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
            var responsePut = await httpClient.PutAsync
                ($"{_baseUrlForV1}/{_testValuesForAccounts.AccountId1}", new StringContent("test", Encoding.UTF8, "application/json"));

            var responseDel = await httpClient.DeleteAsync($"{_baseUrlForV1}/{_testValuesForAccounts.AccountId1}");
            var responseAddToGroup = await httpClient.PutAsync
                ($"{_baseUrlForV1}/{_testValuesForAccounts.AccountId2}/AccountGroups/{_testValuesForAccountGroups.AccountGroupId2}", null);

            var responseDelToGroup = await httpClient.DeleteAsync
                ($"{_baseUrlForV1}/{_testValuesForAccounts.AccountId2}/AccountGroups/{_testValuesForAccountGroups.AccountGroupId2}");

            //Assert
            responsePost.StatusCode.Should().Be(HttpStatusCode.Forbidden);
            responsePut.StatusCode.Should().Be(HttpStatusCode.Forbidden);
            responseDel.StatusCode.Should().Be(HttpStatusCode.Forbidden);
            responseAddToGroup.StatusCode.Should().Be(HttpStatusCode.Forbidden);
            responseDelToGroup.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task GetAll_Accounts_Ok()
        {
            //Arrange
            var httpClient = Factory.CreateDefaultClient();

            var accessToken = await AuthHelper.GetAccessTokenReadOnly();
            httpClient.DefaultRequestHeaders.Authorization =new AuthenticationHeaderValue("Bearer", accessToken);

            //Act
            var response = await httpClient.GetAsync(_baseUrlForV1);
            var result = await response.Content.ReadFromJsonAsync<IEnumerable<GetAllAccountsResult>>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Should()
                .NotBeNull()
                .And.AllBeOfType<GetAllAccountsResult>(); ;
        }

        [Fact]
        public async Task Get_Account_Ok()
        {
            //Arrange
            var httpClient = Factory.CreateDefaultClient();

            var accessToken = await AuthHelper.GetAccessTokenReadOnly();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            //Act
            var response = await httpClient.GetAsync($"{_baseUrlForV1}/{_testValuesForAccounts.AccountId1}");
            var result = await response.Content.ReadFromJsonAsync<GetAccountResult>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Should()
                .NotBeNull()
                .And.BeOfType<GetAccountResult>()
                .And.Match<GetAccountResult>(x => x.Code == "1020");
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
                .And.Match<CustomProblemDetails>(x => x.Errors.First().Code == "ACCOUNT_NOT_FOUND");
        }

        [Fact]
        public async Task Post_Account_Ok()
        {
            //Arrange
            var httpClient = Factory.CreateDefaultClient();

            var accessToken = await AuthHelper.GetAccessTokenReadWrite();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            //Act
            var fake = FakeGenerator.GenerateAddAccounts(1).First();
            var postAccountJson = JsonSerializer.Serialize(fake);
            var content = new StringContent(postAccountJson.ToString(), Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync($"{_baseUrlForV1}", content);
            var result = await response.Content.ReadFromJsonAsync<AddAccountResult>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            result.Should()
                .NotBeNull()
                .And.BeOfType<AddAccountResult>()
                .And.Match<AddAccountResult>(x =>
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
            var fake = FakeGenerator.GenerateAddAccounts(1,code: _testValuesForAccounts.AccountCode1).First();

            var postAccountJson = JsonSerializer.Serialize(fake);
            var content = new StringContent(postAccountJson.ToString(), Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync($"{_baseUrlForV1}", content);
            var result = await response.Content.ReadFromJsonAsync<CustomProblemDetails>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Conflict);
            result.Should()
                .NotBeNull()
                .And.BeOfType<CustomProblemDetails>()
                .And.Match<CustomProblemDetails>(x => x.Errors.First().Code == "ACCOUNT_ALREADY_EXISTS");
        }

        [Fact]
        public async Task Post_ProblemDetails_CurrencyIdNotFound()
        {
            //Arrange
            var httpClient = Factory.CreateDefaultClient();

            var accessToken = await AuthHelper.GetAccessTokenReadWrite();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            //Act
            var fake = FakeGenerator.GenerateAddAccounts(1,currencyId:Guid.NewGuid()).First();

            var postAccountJson = JsonSerializer.Serialize(fake);
            var content = new StringContent(postAccountJson.ToString(), Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync($"{_baseUrlForV1}", content);
            var result = await response.Content.ReadFromJsonAsync<CustomProblemDetails>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            result.Should()
                .NotBeNull()
                .And.BeOfType<CustomProblemDetails>()
                .And.Match<CustomProblemDetails>(x => x.Errors.First().Code == "ACCOUNT_CURRENCY_NOT_FOUND");
        }

        [Fact]
        public async Task Post_ProblemDetails_EmptyFields()
        {
            //Arrange
            var httpClient = Factory.CreateDefaultClient();

            var accessToken = await AuthHelper.GetAccessTokenReadWrite();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            //Act
            var fake = FakeGenerator.GenerateAddAccounts(1,code:"",label:"").First();

            var postAccountJson = JsonSerializer.Serialize(fake);
            var content = new StringContent(postAccountJson.ToString(), Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync($"{_baseUrlForV1}", content);
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
            var fake = FakeGenerator.GenerateAddAccounts(1,
                code: new string(new Faker("fr_CH").Random.Chars(count: 21)),
                label: new string(new Faker("fr_CH").Random.Chars(count: 101)),
                description : new string(new Faker("fr_CH").Random.Chars(count: 701))
                ).First();

            var postAccountJson = JsonSerializer.Serialize(fake);
            var content = new StringContent(postAccountJson.ToString(), Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync($"{_baseUrlForV1}", content);
            var result = await response.Content.ReadFromJsonAsync<CustomProblemDetails>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            result.Should()
                .NotBeNull()
                .And.BeOfType<CustomProblemDetails>()
                .And.Match<CustomProblemDetails>(x => x.Errors.First().Code == "VALIDATION_ERROR" && x.Errors.Count() == 3);
        }

        [Fact]
        public async Task Put_Account_Ok()
        {
            //Arrange
            var httpClient = Factory.CreateDefaultClient();

            var accessToken = await AuthHelper.GetAccessTokenReadWrite();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            //Act
            var responseGet = await httpClient.GetAsync($"{_baseUrlForV1}/{_testValuesForAccounts.AccountId1}");
            var resultGet = await responseGet.Content.ReadFromJsonAsync<GetAccountResult>();

            var fake = FakeGenerator.GenerateUpdAccounts(1, resultGet!.Id, _testValuesForAccounts.AccountCode1,version: resultGet!.Version).First();

            var postAccountJson = JsonSerializer.Serialize(fake);
            var content = new StringContent(postAccountJson.ToString(), Encoding.UTF8, "application/json");

            var response = await httpClient.PutAsync($"{_baseUrlForV1}/{_testValuesForAccounts.AccountId1}", content);
            var result = await response.Content.ReadFromJsonAsync<UpdateAccountResult>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Should()
                .NotBeNull()
                .And.BeOfType<UpdateAccountResult>()
                .And.Match<UpdateAccountResult>(x =>
                    x.Code == fake.Code
                    && x.Label == fake.Label
                    && x.Description == fake.Description
                    && x.Version != fake.Version);
        }

        [Fact]
        public async Task PutAddToAccountGroup_AccountAccountGroup_Ok()
        {
            //Arrange
            var httpClient = Factory.CreateDefaultClient();

            var accessToken = await AuthHelper.GetAccessTokenReadWrite();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            //Act
            var response = await httpClient.PutAsync
                ($"{_baseUrlForV1}/{_testValuesForAccounts.AccountId2}/AccountGroups/{_testValuesForAccountGroups.AccountGroupId2}", null);

            var result = await response.Content.ReadFromJsonAsync<AddAccountInAccountGroupResult>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Should()
                .NotBeNull()
                .And.BeOfType<AddAccountInAccountGroupResult>()
                .And.Match<AddAccountInAccountGroupResult>(x =>
                    x.AccountId == _testValuesForAccounts.AccountId2);

            //Clean
            await httpClient.DeleteAsync
                ($"{_baseUrlForV1}/{_testValuesForAccounts.AccountId2}/AccountGroups/{_testValuesForAccountGroups.AccountGroupId2}");
        }

        [Fact]
        public async Task PutAddToAccountGroup_ProblemDetails_AccountNotFound()
        {
            //Arrange
            var httpClient = Factory.CreateDefaultClient();

            var accessToken = await AuthHelper.GetAccessTokenReadWrite();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            //Act
            var response = await httpClient.PutAsync
                ($"{_baseUrlForV1}/{Guid.NewGuid()}/AccountGroups/{_testValuesForAccountGroups.AccountGroupId2}", null);

            var result = await response.Content.ReadFromJsonAsync<CustomProblemDetails>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            result.Should()
                .NotBeNull()
                .And.BeOfType<CustomProblemDetails>()
                .And.Match<CustomProblemDetails>(x => x.Errors.First().Code == "ACCOUNT_NOT_FOUND");
        }


        [Fact]
        public async Task PutAddToAccountGroup_ProblemDetails_AccountGroupNotFound()
        {
            //Arrange
            var httpClient = Factory.CreateDefaultClient();

            var accessToken = await AuthHelper.GetAccessTokenReadWrite();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            //Act
            var response = await httpClient.PutAsync
                ($"{_baseUrlForV1}/{_testValuesForAccounts.AccountId2}/AccountGroups/{Guid.NewGuid()}", null);

            var result = await response.Content.ReadFromJsonAsync<CustomProblemDetails>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            result.Should()
                .NotBeNull()
                .And.BeOfType<CustomProblemDetails>()
                .And.Match<CustomProblemDetails>(x => x.Errors.First().Code == "ACCOUNTGROUP_NOT_FOUND");
        }

        [Fact]
        public async Task PutAddToAccountGroup_ProblemDetails_AccountAlreadyExistsInClassification()
        {
            //Arrange
            var httpClient = Factory.CreateDefaultClient();

            var accessToken = await AuthHelper.GetAccessTokenReadWrite();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            //Act
            var response = await httpClient.PutAsync
                ($"{_baseUrlForV1}/{_testValuesForAccounts.AccountId1}/AccountGroups/{_testValuesForAccountGroups.AccountGroupId1}", null);

            var result = await response.Content.ReadFromJsonAsync<CustomProblemDetails>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Conflict);
            result.Should()
                .NotBeNull()
                .And.BeOfType<CustomProblemDetails>()
                .And.Match<CustomProblemDetails>(x => x.Errors.First().Code == "ACCOUNT_ALREADY_EXISTS_IN_CLASSIFICATION");
        }

        [Fact]
        public async Task Put_ProblemDetails_EmptyFields()
        {
            //Arrange
            var httpClient = Factory.CreateDefaultClient();

            var accessToken = await AuthHelper.GetAccessTokenReadWrite();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            //Act
            var fake = FakeGenerator.GenerateUpdAccounts(1,code:string.Empty,label:string.Empty).First();

            var responseGet = await httpClient.GetAsync($"{_baseUrlForV1}/{_testValuesForAccounts.AccountId1}");
            var resultGet = await responseGet.Content.ReadFromJsonAsync<GetAccountResult>();

            var postAccountJson = JsonSerializer.Serialize(fake);
            var content = new StringContent(postAccountJson.ToString(), Encoding.UTF8, "application/json");

            var response = await httpClient.PutAsync($"{_baseUrlForV1}/{_testValuesForAccounts.AccountId1}", content);
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
            var responseGet = await httpClient.GetAsync($"{_baseUrlForV1}/{_testValuesForAccounts.AccountId1}");
            var resultGet = await responseGet.Content.ReadFromJsonAsync<GetAccountResult>();

            var fake = FakeGenerator.GenerateUpdAccounts(1,
                code: new string(new Faker("fr_CH").Random.Chars(count: 21)),
                label: new string(new Faker("fr_CH").Random.Chars(count: 101)),
                description: new string(new Faker("fr_CH").Random.Chars(count: 701)),
                version: resultGet!.Version).First();

            var postAccountJson = JsonSerializer.Serialize(fake);
            var content = new StringContent(postAccountJson.ToString(), Encoding.UTF8, "application/json");

            var response = await httpClient.PutAsync($"{_baseUrlForV1}/{_testValuesForAccounts.AccountId1}", content);
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
            var responseGet = await httpClient.GetAsync($"{_baseUrlForV1}/{_testValuesForAccounts.AccountId2}");
            var resultGet = await responseGet.Content.ReadFromJsonAsync<GetAccountResult>();
            
            var fake = FakeGenerator.GenerateUpdAccounts(1, id: resultGet!.Id, code:"1020",version: resultGet!.Version).First();

            var postAccountJson = JsonSerializer.Serialize(fake);
            var content = new StringContent(postAccountJson.ToString(), Encoding.UTF8, "application/json");

            var response = await httpClient.PutAsync($"{_baseUrlForV1}/{_testValuesForAccounts.AccountId2}", content);
            var result = await response.Content.ReadFromJsonAsync<CustomProblemDetails>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Conflict);
            result.Should()
                .NotBeNull()
                .And.BeOfType<CustomProblemDetails>()
                .And.Match<CustomProblemDetails>(x => x.Errors.First().Code == "ACCOUNT_ALREADY_EXISTS");
        }

        [Fact]
        public async Task Put_ProblemDetails_IdNotFound()
        {
            //Arrange
            var httpClient = Factory.CreateDefaultClient();

            var accessToken = await AuthHelper.GetAccessTokenReadWrite();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            //Act
            var responseGet = await httpClient.GetAsync($"{_baseUrlForV1}/{_testValuesForAccounts.AccountId2}");
            var resultGet = await responseGet.Content.ReadFromJsonAsync<GetAccountResult>();

            var fake = FakeGenerator.GenerateUpdAccounts(1, id:Guid.NewGuid(), version: resultGet!.Version).First();

            var postAccountJson = JsonSerializer.Serialize(fake);
            var content = new StringContent(postAccountJson.ToString(), Encoding.UTF8, "application/json");

            var response = await httpClient.PutAsync($"{_baseUrlForV1}/{fake.Id}", content);
            var result = await response.Content.ReadFromJsonAsync<CustomProblemDetails>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            result.Should()
                .NotBeNull()
                .And.BeOfType<CustomProblemDetails>()
                .And.Match<CustomProblemDetails>(x => x.Errors.First().Code == "ACCOUNT_NOT_FOUND");
        }

        [Fact]
        public async Task Put_ProblemDetails_CurrencyIdNotFound()
        {
            //Arrange
            var httpClient = Factory.CreateDefaultClient();

            var accessToken = await AuthHelper.GetAccessTokenReadWrite();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            //Act
            var responseGet = await httpClient.GetAsync($"{_baseUrlForV1}/{_testValuesForAccounts.AccountId1}");
            var resultGet = await responseGet.Content.ReadFromJsonAsync<GetAccountResult>();

            var fake = FakeGenerator.GenerateUpdAccounts(1, id: _testValuesForAccounts.AccountId1, currencyId:Guid.NewGuid()).First();

            var postAccountJson = JsonSerializer.Serialize(fake);
            var content = new StringContent(postAccountJson.ToString(), Encoding.UTF8, "application/json");

            var response = await httpClient.PutAsync($"{_baseUrlForV1}/{_testValuesForAccounts.AccountId1}", content);
            var result = await response.Content.ReadFromJsonAsync<CustomProblemDetails>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            result.Should()
                .NotBeNull()
                .And.BeOfType<CustomProblemDetails>()
                .And.Match<CustomProblemDetails>(x => x.Errors.First().Code == "ACCOUNT_CURRENCY_NOT_FOUND");
        }

        [Fact]
        public async Task Put_ProblemDetails_QueryIdAndCommandIdNotMatch()
        {
            //Arrange
            var httpClient = Factory.CreateDefaultClient();

            var accessToken = await AuthHelper.GetAccessTokenReadWrite();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            //Act
            var responseGet = await httpClient.GetAsync($"{_baseUrlForV1}/{_testValuesForAccounts.AccountId1}");
            var resultGet = await responseGet.Content.ReadFromJsonAsync<GetAccountResult>();

            var fake = FakeGenerator.GenerateUpdAccounts(1, id: Guid.NewGuid(), currencyId: Guid.NewGuid()).First();

            var postAccountJson = JsonSerializer.Serialize(fake);
            var content = new StringContent(postAccountJson.ToString(), Encoding.UTF8, "application/json");

            var response = await httpClient.PutAsync($"{_baseUrlForV1}/{_testValuesForAccounts.AccountId1}", content);
            var result = await response.Content.ReadFromJsonAsync<CustomProblemDetails>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            result.Should()
                .NotBeNull()
                .And.BeOfType<CustomProblemDetails>()
                .And.Match<CustomProblemDetails>(x => x.Errors.First().Code == "ACCOUNT_UPDATE_IDS_NOT_MATCH");
        }

        [Fact]
        public async Task Put_ProblemDetails_ModifiedByAnotherProcess()
        {
            //Arrange
            var httpClient = Factory.CreateDefaultClient();

            var accessToken = await AuthHelper.GetAccessTokenReadWrite();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            //Act
            var responseGet = await httpClient.GetAsync($"{_baseUrlForV1}/{_testValuesForAccounts.AccountId1}");
            var resultGet = await responseGet.Content.ReadFromJsonAsync<GetAccountResult>();

            var fake = FakeGenerator.GenerateUpdAccounts(1,
                version: resultGet!.Version,
                id: resultGet!.Id,
                code: _testValuesForAccounts.AccountCode1).First();

            var postAccountJson = JsonSerializer.Serialize(fake);
            var content = new StringContent(postAccountJson.ToString(), Encoding.UTF8, "application/json");

            var response = await httpClient.PutAsync($"{_baseUrlForV1}/{_testValuesForAccounts.AccountId1}", content);
            var response2 = await httpClient.PutAsync($"{_baseUrlForV1}/{_testValuesForAccounts.AccountId1}", content);
            var result = await response2.Content.ReadFromJsonAsync<CustomProblemDetails>();

            //Assert
            response2.StatusCode.Should().Be(HttpStatusCode.Conflict);
            result.Should()
                .NotBeNull()
                .And.BeOfType<CustomProblemDetails>()
                .And.Match<CustomProblemDetails>(x => x.Errors.First().Code == "ACCOUNT_UPDATE_CONCURRENCY");
        }

        [Fact]
        public async Task Del_NoContent_Ok()
        {
            //Arrange
            var httpClient = Factory.CreateDefaultClient();

            var accessToken = await AuthHelper.GetAccessTokenReadWrite();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            //Act
            var response = await httpClient.DeleteAsync($"{_baseUrlForV1}/{_testValuesForAccounts.AccountIdForDel}");

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
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
                .And.Match<CustomProblemDetails>(x => x.Errors.First().Code == "ACCOUNT_NOT_FOUND");
        }

        [Fact]
        public async Task DelFromAccountGroup_NoContent_Ok()
        {
            //Arrange
            var httpClient = Factory.CreateDefaultClient();

            var accessToken = await AuthHelper.GetAccessTokenReadWrite();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            //Act
            var response = await httpClient.DeleteAsync($"{_baseUrlForV1}/{_testValuesForAccounts.AccountId1}/AccountGroups/{_testValuesForAccountGroups.AccountGroupId1}");

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);

            //Clean
            await httpClient.PutAsync($"{_baseUrlForV1}/{_testValuesForAccounts.AccountId1}/AccountGroups/{_testValuesForAccountGroups.AccountGroupId1}",null);
        }

        [Fact]
        public async Task DelFromAccountGroup_ProblemDetails_AccountAccountGroupNotFound()
        {
            //Arrange
            var httpClient = Factory.CreateDefaultClient();

            var accessToken = await AuthHelper.GetAccessTokenReadWrite();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            //Act
            var response = await httpClient.DeleteAsync($"{_baseUrlForV1}/{Guid.NewGuid()}/AccountGroups/{Guid.NewGuid()}");
            var result = await response.Content.ReadFromJsonAsync<CustomProblemDetails>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            result.Should()
                .NotBeNull()
                .And.BeOfType<CustomProblemDetails>()
                .And.Match<CustomProblemDetails>(x => x.Errors.First().Code == "ACCOUNT_NOT_EXISTS_IN_ACCOUNTGROUP");
        }
    }
}
