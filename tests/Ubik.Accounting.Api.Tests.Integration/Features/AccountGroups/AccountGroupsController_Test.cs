using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Ubik.Accounting.Api.Data.Init;
using static Ubik.Accounting.Api.Features.Accounts.Queries.GetAllAccounts;
using Ubik.Accounting.Api.Tests.Integration.Auth;
using static Ubik.Accounting.Api.Features.AccountGroups.Queries.GetAllAccountGroups;
using Bogus;
using static Ubik.Accounting.Api.Features.Accounts.Commands.AddAccount;
using static Ubik.Accounting.Api.Features.Accounts.Commands.UpdateAccount;
using static Ubik.Accounting.Api.Features.Accounts.Queries.GetAccount;
using Ubik.ApiService.Common.Exceptions;
using static Ubik.Accounting.Api.Features.AccountGroups.Queries.GetAccountGroup;
using Ubik.Accounting.Api.Features.AccountGroups.Mappers;
using System.Text.Json;
using static Ubik.Accounting.Api.Features.AccountGroups.Commands.AddAccountGroup;
using static Ubik.Accounting.Api.Features.AccountGroups.Commands.UpdateAccountGroup;
using Ubik.Accounting.Api.Features.AccountGroups.Queries;
using static Ubik.Accounting.Api.Features.AccountGroups.Queries.GetChildAccounts;

namespace Ubik.Accounting.Api.Tests.Integration.Features.AccountGroups
{
    public class AccountGroupsController_Test : BaseIntegrationTest
    {
        private readonly BaseValuesForAccountGroups _testValuesForAccountGroups;
        private readonly BaseValuesForAccountGroupClassifications _testValuesForAccountGroupClassifications;

        public AccountGroupsController_Test(IntegrationTestWebAppFactory factory) : base(factory)
        {
            _testValuesForAccountGroups = new BaseValuesForAccountGroups();
            _testValuesForAccountGroupClassifications = new BaseValuesForAccountGroupClassifications();
        }

        [Fact]
        public async Task CheckAuth_401_NoAuth()
        {
            //Arrange
            var httpClient = Factory.CreateDefaultClient();

            //Act
            var responseGetAll = await httpClient.GetAsync("/AccountGroups");
            var responseGet = await httpClient.GetAsync($"/AccountGroups/{_testValuesForAccountGroups.AccountGroupId1}");
            var responseGetChildAccounts = await httpClient.GetAsync($"/AccountGroups/{_testValuesForAccountGroups.AccountGroupId1}/Accounts");
            var responsePost = await httpClient.PostAsync("/AccountGroups", new StringContent("test", Encoding.UTF8, "application/json"));
            var responsePut = await httpClient.PutAsync($"/AccountGroups/{_testValuesForAccountGroups.AccountGroupId1}", new StringContent("test", Encoding.UTF8, "application/json"));
            var responseDel = await httpClient.DeleteAsync($"/AccountGroups/{_testValuesForAccountGroups.AccountGroupId1}");

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
            var responseGetAll = await httpClient.GetAsync("/AccountGroups");
            var responseGet = await httpClient.GetAsync($"/AccountGroups/{_testValuesForAccountGroups.AccountGroupId1}");
            var responseGetChildAccounts = await httpClient.GetAsync($"/AccountGroups/{_testValuesForAccountGroups.AccountGroupId1}/Accounts");
            var responsePost = await httpClient.PostAsync("/AccountGroups", new StringContent("test", Encoding.UTF8, "application/json"));
            var responsePut = await httpClient.PutAsync($"/AccountGroups/{_testValuesForAccountGroups.AccountGroupId1}", new StringContent("test", Encoding.UTF8, "application/json"));
            var responseDel = await httpClient.DeleteAsync($"/AccountGroups/{_testValuesForAccountGroups.AccountGroupId1}");

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
            var responsePost = await httpClient.PostAsync("/AccountGroups", new StringContent("test", Encoding.UTF8, "application/json"));
            var responsePut = await httpClient.PutAsync($"/AccountGroups/{_testValuesForAccountGroups.AccountGroupId1}", new StringContent("test", Encoding.UTF8, "application/json"));
            var responseDel = await httpClient.DeleteAsync($"/AccountGroups/{_testValuesForAccountGroups.AccountGroupId1}");

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
            var response = await httpClient.GetAsync("/AccountGroups");
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
            var response = await httpClient.GetAsync($"/AccountGroups/{_testValuesForAccountGroups.AccountGroupId1}");
            var result = await response.Content.ReadFromJsonAsync<GetAccountGroupResult>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Should()
                .NotBeNull()
                .And.BeOfType<GetAccountGroupResult>()
                .And.Match<GetAccountGroupResult>(x => x.Code == "102");
        }

        [Fact]
        public async Task Get_ProblemDetails_IdNotFound()
        {
            //Arrange
            var httpClient = Factory.CreateDefaultClient();

            var accessToken = await AuthHelper.GetAccessTokenReadOnly();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            //Act
            var response = await httpClient.GetAsync($"/AccountGroups/{Guid.NewGuid()}");
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
            var response = await httpClient.GetAsync($"/AccountGroups/{_testValuesForAccountGroups.AccountGroupId1}/Accounts");
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
            var response = await httpClient.GetAsync($"/AccountGroups/{Guid.NewGuid()}/Accounts");
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

            var response = await httpClient.PostAsync($"/AccountGroups", content);
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
            var fake = FakeGenerator.GenerateAddAccountGroups(1).First();
            fake.Code = _testValuesForAccountGroups.AccountGroupCode1;
            fake.AccountGroupClassificationId = _testValuesForAccountGroupClassifications.AccountGroupClassificationId2;

            var postAccountJson = JsonSerializer.Serialize(fake);
            var content = new StringContent(postAccountJson.ToString(), Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync($"/AccountGroups", content);
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
            var fake = FakeGenerator.GenerateAddAccountGroups(1).First();
            fake.Code = _testValuesForAccountGroups.AccountGroupCode1;

            var postAccountJson = JsonSerializer.Serialize(fake);
            var content = new StringContent(postAccountJson.ToString(), Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync($"/AccountGroups", content);
            var result = await response.Content.ReadFromJsonAsync<CustomProblemDetails>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Conflict);
            result.Should()
                .NotBeNull()
                .And.BeOfType<CustomProblemDetails>()
                .And.Match<CustomProblemDetails>(x => x.Errors.First().Code == "ACCOUNTGROUP_ALREADY_EXISTS");
        }

        [Fact]
        public async Task Post_ProblemDetails_EmptyFields()
        {
            //Arrange
            var httpClient = Factory.CreateDefaultClient();

            var accessToken = await AuthHelper.GetAccessTokenReadWrite();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            //Act
            var fake = FakeGenerator.GenerateAddAccountGroups(1).First();

            fake.Code = "";
            fake.Label = "";
            fake.AccountGroupClassificationId = default;

            var postAccountJson = JsonSerializer.Serialize(fake);
            var content = new StringContent(postAccountJson.ToString(), Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync($"/AccountGroups", content);
            var result = await response.Content.ReadFromJsonAsync<CustomProblemDetails>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            result.Should()
                .NotBeNull()
                .And.BeOfType<CustomProblemDetails>()
                .And.Match<CustomProblemDetails>(x => x.Errors.First().Code == "VALIDATION_ERROR" && x.Errors.Count() == 3);
        }

        [Fact]
        public async Task Post_ProblemDetails_TooLongFields()
        {
            //Arrange
            var httpClient = Factory.CreateDefaultClient();

            var accessToken = await AuthHelper.GetAccessTokenReadWrite();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            //Act
            var fake = FakeGenerator.GenerateAddAccountGroups(1).First();

            fake.Code = new string(new Faker("fr_CH").Random.Chars(count: 21));
            fake.Label = new string(new Faker("fr_CH").Random.Chars(count: 101));
            fake.Description = new string(new Faker("fr_CH").Random.Chars(count: 701));

            var postAccountJson = JsonSerializer.Serialize(fake);
            var content = new StringContent(postAccountJson.ToString(), Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync($"/AccountGroups", content);
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
            var fake = FakeGenerator.GenerateUpdAccountGroups(1).First();

            var responseGet = await httpClient.GetAsync($"/AccountGroups/{_testValuesForAccountGroups.AccountGroupId1}");
            var resultGet = await responseGet.Content.ReadFromJsonAsync<GetAccountGroupResult>();

            fake.Version = resultGet!.Version;
            fake.Id = resultGet!.Id;
            fake.Code = _testValuesForAccountGroups.AccountGroupCode1;

            var postAccountJson = JsonSerializer.Serialize(fake);
            var content = new StringContent(postAccountJson.ToString(), Encoding.UTF8, "application/json");

            var response = await httpClient.PutAsync($"/AccountGroups/{_testValuesForAccountGroups.AccountGroupId1}", content);
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
        public async Task Put_ProblemDetails_EmptyFields()
        {
            //Arrange
            var httpClient = Factory.CreateDefaultClient();

            var accessToken = await AuthHelper.GetAccessTokenReadWrite();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            //Act
            var fake = FakeGenerator.GenerateUpdAccountGroups(1).First();

            var responseGet = await httpClient.GetAsync($"/AccountGroups/{_testValuesForAccountGroups.AccountGroupId1}");
            var resultGet = await responseGet.Content.ReadFromJsonAsync<GetAccountResult>();

            fake.Version = default!;
            fake.Code = string.Empty;
            fake.Label = string.Empty;

            var postAccountJson = JsonSerializer.Serialize(fake);
            var content = new StringContent(postAccountJson.ToString(), Encoding.UTF8, "application/json");

            var response = await httpClient.PutAsync($"/AccountGroups/{_testValuesForAccountGroups.AccountGroupId1}", content);
            var result = await response.Content.ReadFromJsonAsync<CustomProblemDetails>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            result.Should()
                .NotBeNull()
                .And.BeOfType<CustomProblemDetails>()
                .And.Match<CustomProblemDetails>(x => x.Errors.First().Code == "VALIDATION_ERROR" && x.Errors.Count() == 3);
        }

        [Fact]
        public async Task Put_ProblemDetails_TooLongFields()
        {
            //Arrange
            var httpClient = Factory.CreateDefaultClient();

            var accessToken = await AuthHelper.GetAccessTokenReadWrite();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            //Act
            var fake = FakeGenerator.GenerateUpdAccountGroups(1).First();

            var responseGet = await httpClient.GetAsync($"/AccountGroups/{_testValuesForAccountGroups.AccountGroupId1}");
            var resultGet = await responseGet.Content.ReadFromJsonAsync<GetAccountResult>();

            fake.Code = new string(new Faker("fr_CH").Random.Chars(count: 21));
            fake.Label = new string(new Faker("fr_CH").Random.Chars(count: 101));
            fake.Description = new string(new Faker("fr_CH").Random.Chars(count: 701));
            fake.Version = resultGet!.Version;

            var postAccountJson = JsonSerializer.Serialize(fake);
            var content = new StringContent(postAccountJson.ToString(), Encoding.UTF8, "application/json");

            var response = await httpClient.PutAsync($"/AccountGroups/{_testValuesForAccountGroups.AccountGroupId1}", content);
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
            var fake = FakeGenerator.GenerateUpdAccountGroups(1).First();
   
            var responseGet = await httpClient.GetAsync($"/AccountGroups/{_testValuesForAccountGroups.AccountGroupId2}");
            var resultGet = await responseGet.Content.ReadFromJsonAsync<GetAccountResult>();

            fake.Id = resultGet!.Id;
            fake.Code = "102";
            fake.Version = resultGet!.Version;

            var postAccountJson = JsonSerializer.Serialize(fake);
            var content = new StringContent(postAccountJson.ToString(), Encoding.UTF8, "application/json");

            var response = await httpClient.PutAsync($"/AccountGroups/{_testValuesForAccountGroups.AccountGroupId2}", content);
            var result = await response.Content.ReadFromJsonAsync<CustomProblemDetails>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Conflict);
            result.Should()
                .NotBeNull()
                .And.BeOfType<CustomProblemDetails>()
                .And.Match<CustomProblemDetails>(x => x.Errors.First().Code == "ACCOUNTGROUP_ALREADY_EXISTS");
        }

        [Fact]
        public async Task Put_ProblemDetails_IdNotFound()
        {
            //Arrange
            var httpClient = Factory.CreateDefaultClient();

            var accessToken = await AuthHelper.GetAccessTokenReadWrite();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            //Act
            var fake = FakeGenerator.GenerateUpdAccountGroups(1).First();

            var responseGet = await httpClient.GetAsync($"/AccountGroups/{_testValuesForAccountGroups.AccountGroupId2}");
            var resultGet = await responseGet.Content.ReadFromJsonAsync<GetAccountResult>();

            fake.Id = Guid.NewGuid();
            fake.Version = resultGet!.Version;

            var postAccountJson = JsonSerializer.Serialize(fake);
            var content = new StringContent(postAccountJson.ToString(), Encoding.UTF8, "application/json");

            var response = await httpClient.PutAsync($"/AccountGroups/{fake.Id}", content);
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
            var fake = FakeGenerator.GenerateUpdAccountGroups(1).First();

            var responseGet = await httpClient.GetAsync($"/AccountGroups/{_testValuesForAccountGroups.AccountGroupId1}");
            var resultGet = await responseGet.Content.ReadFromJsonAsync<GetAccountResult>();

            fake.Version = resultGet!.Version;
            fake.Id = resultGet!.Id;
            fake.Code = _testValuesForAccountGroups.AccountGroupCode1;

            var postAccountJson = JsonSerializer.Serialize(fake);
            var content = new StringContent(postAccountJson.ToString(), Encoding.UTF8, "application/json");

            var response = await httpClient.PutAsync($"/AccountGroups/{_testValuesForAccountGroups.AccountGroupId1}", content);
            var response2 = await httpClient.PutAsync($"/AccountGroups/{_testValuesForAccountGroups.AccountGroupId1}", content);
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
            var response = await httpClient.DeleteAsync($"/AccountGroups/{_testValuesForAccountGroups.AccountGroupIdForDel}");

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
            var response = await httpClient.DeleteAsync($"/AccountGroups/{Guid.NewGuid()}");
            var result = await response.Content.ReadFromJsonAsync<CustomProblemDetails>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            result.Should()
                .NotBeNull()
                .And.BeOfType<CustomProblemDetails>()
                .And.Match<CustomProblemDetails>(x => x.Errors.First().Code == "ACCOUNTGROUP_NOT_FOUND");
        }

        [Fact]
        public async Task Del_ProblemDetails_IdEmpty()
        {
            //Arrange
            var httpClient = Factory.CreateDefaultClient();

            var accessToken = await AuthHelper.GetAccessTokenReadWrite();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            Guid empty = default!;

            //Act
            var response = await httpClient.DeleteAsync($"/AccountGroups/{empty}");
            var result = await response.Content.ReadFromJsonAsync<CustomProblemDetails>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            result.Should()
                .NotBeNull()
                .And.BeOfType<CustomProblemDetails>()
                .And.Match<CustomProblemDetails>(x => x.Errors.First().Code == "VALIDATION_ERROR");
        }
    }
}
