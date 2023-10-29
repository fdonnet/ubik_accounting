using FluentAssertions;
using System.Net.Http.Json;
using System.Text;
using static Ubik.Accounting.Api.Features.Accounts.Queries.GetAccount;
using System.Net;
using Ubik.ApiService.Common.Exceptions;
using Bogus;
using System.Text.Json;
using static Ubik.Accounting.Api.Features.Accounts.Commands.UpdateAccount;
using System.Net.Http.Headers;
using Ubik.Accounting.Api.Tests.Integration.Auth;
using Ubik.Accounting.Api.Data.Init;
using Ubik.Accounting.Contracts.Accounts.Results;

namespace Ubik.Accounting.Api.Tests.Integration.Features.Accounts
{
    public class AccountsController_Test : BaseIntegrationTest
    {
        private readonly BaseValuesForAccounts _testValuesForAccounts;
        private readonly string _baseUrlForV1;

        public AccountsController_Test(IntegrationTestWebAppFactory factory) : base(factory)
        {
            _testValuesForAccounts = new BaseValuesForAccounts();
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
            var responsePost = await httpClient.PostAsync(_baseUrlForV1, new StringContent("test", Encoding.UTF8, "application/json"));
            var responsePut = await httpClient.PutAsync($"{_baseUrlForV1}/{_testValuesForAccounts.AccountId1}", new StringContent("test", Encoding.UTF8, "application/json"));
            var responseDel = await httpClient.DeleteAsync($"{_baseUrlForV1}/{_testValuesForAccounts.AccountId1}");

            //Assert
            responseGetAll.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
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
            var responseGet = await httpClient.GetAsync($"{_baseUrlForV1}/{_testValuesForAccounts.AccountId1}");
            var responsePost = await httpClient.PostAsync(_baseUrlForV1, new StringContent("test", Encoding.UTF8, "application/json"));
            var responsePut = await httpClient.PutAsync($"{_baseUrlForV1}/{_testValuesForAccounts.AccountId1}", new StringContent("test", Encoding.UTF8, "application/json"));
            var responseDel = await httpClient.DeleteAsync($"{_baseUrlForV1}/{_testValuesForAccounts.AccountId1}");

            //Assert
            responseGetAll.StatusCode.Should().Be(HttpStatusCode.Forbidden);
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
            var responsePut = await httpClient.PutAsync($"{_baseUrlForV1}/{_testValuesForAccounts.AccountId1}", new StringContent("test", Encoding.UTF8, "application/json"));
            var responseDel = await httpClient.DeleteAsync($"{_baseUrlForV1}/{_testValuesForAccounts.AccountId1}");

            //Assert
            responsePost.StatusCode.Should().Be(HttpStatusCode.Forbidden);
            responsePut.StatusCode.Should().Be(HttpStatusCode.Forbidden);
            responseDel.StatusCode.Should().Be(HttpStatusCode.Forbidden);
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
            var fake = FakeGenerator.GenerateAddAccounts(1).First();
            fake.Code = _testValuesForAccounts.AccountCode1;

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
            var fake = FakeGenerator.GenerateAddAccounts(1).First();
            fake.CurrencyId = Guid.NewGuid();

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
            var fake = FakeGenerator.GenerateAddAccounts(1).First();

            fake.Code = "";
            fake.Label = "";

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
            var fake = FakeGenerator.GenerateAddAccounts(1).First();

            fake.Code = new string(new Faker("fr_CH").Random.Chars(count: 21));
            fake.Label = new string(new Faker("fr_CH").Random.Chars(count: 101));
            fake.Description = new string(new Faker("fr_CH").Random.Chars(count: 701));

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
            var fake = FakeGenerator.GenerateUpdAccounts(1).First();

            var responseGet = await httpClient.GetAsync($"{_baseUrlForV1}/{_testValuesForAccounts.AccountId1}");
            var resultGet = await responseGet.Content.ReadFromJsonAsync<GetAccountResult>();

            fake.Version = resultGet!.Version;
            fake.Id = resultGet!.Id;
            fake.Code = _testValuesForAccounts.AccountCode1;

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
        public async Task Put_ProblemDetails_EmptyFields()
        {
            //Arrange
            var httpClient = Factory.CreateDefaultClient();

            var accessToken = await AuthHelper.GetAccessTokenReadWrite();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            //Act
            var fake = FakeGenerator.GenerateUpdAccounts(1).First();

            var responseGet = await httpClient.GetAsync($"{_baseUrlForV1}/{_testValuesForAccounts.AccountId1}");
            var resultGet = await responseGet.Content.ReadFromJsonAsync<GetAccountResult>();

            fake.Code = string.Empty;
            fake.Label = string.Empty;

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
            var fake = FakeGenerator.GenerateUpdAccounts(1).First();

            var responseGet = await httpClient.GetAsync($"{_baseUrlForV1}/{_testValuesForAccounts.AccountId1}");
            var resultGet = await responseGet.Content.ReadFromJsonAsync<GetAccountResult>();

            fake.Code = new string(new Faker("fr_CH").Random.Chars(count: 21));
            fake.Label = new string(new Faker("fr_CH").Random.Chars(count: 101));
            fake.Description = new string(new Faker("fr_CH").Random.Chars(count: 701));
            fake.Version = resultGet!.Version;

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
            var fake = FakeGenerator.GenerateUpdAccounts(1).First();

            var responseGet = await httpClient.GetAsync($"{_baseUrlForV1}/{_testValuesForAccounts.AccountId2}");
            var resultGet = await responseGet.Content.ReadFromJsonAsync<GetAccountResult>();

            fake.Id = resultGet!.Id;
            fake.Code = "1020";
            fake.Version = resultGet!.Version;

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
            var fake = FakeGenerator.GenerateUpdAccounts(1).First();

            var responseGet = await httpClient.GetAsync($"{_baseUrlForV1}/{_testValuesForAccounts.AccountId2}");
            var resultGet = await responseGet.Content.ReadFromJsonAsync<GetAccountResult>();

            fake.Id = Guid.NewGuid();
            fake.Version = resultGet!.Version;

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
            var fake = FakeGenerator.GenerateUpdAccounts(1).First();

            var responseGet = await httpClient.GetAsync($"{_baseUrlForV1}/{_testValuesForAccounts.AccountId1}");
            var resultGet = await responseGet.Content.ReadFromJsonAsync<GetAccountResult>();

            fake.CurrencyId = Guid.NewGuid();

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
        public async Task Put_ProblemDetails_ModifiedByAnotherProcess()
        {
            //Arrange
            var httpClient = Factory.CreateDefaultClient();

            var accessToken = await AuthHelper.GetAccessTokenReadWrite();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            //Act
            var fake = FakeGenerator.GenerateUpdAccounts(1).First();

            var responseGet = await httpClient.GetAsync($"{_baseUrlForV1}/{_testValuesForAccounts.AccountId1}");
            var resultGet = await responseGet.Content.ReadFromJsonAsync<GetAccountResult>();

            fake.Version = resultGet!.Version;
            fake.Id = resultGet!.Id;
            fake.Code = _testValuesForAccounts.AccountCode1;

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
                .And.Match<CustomProblemDetails>(x => x.Errors.First().Code == "DB_CONCURRENCY_CONFLICT");
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
    }
}
