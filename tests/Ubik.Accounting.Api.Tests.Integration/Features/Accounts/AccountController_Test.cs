using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Ubik.Accounting.Api.Data;
using Ubik.Accounting.Api.Features;
using Ubik.Accounting.Api.Models;
using static Ubik.Accounting.Api.Features.Accounts.Queries.GetAllAccounts;
using static Ubik.Accounting.Api.Features.Accounts.Queries.GetAccount;
using System.Net;
using Ubik.ApiService.Common.Exceptions;
using static Ubik.Accounting.Api.Features.Accounts.Commands.AddAccount;
using Bogus;
using System.Text.Json.Nodes;
using System.Text.Json;
using Xunit.Abstractions;
using DotNet.Testcontainers.Containers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Org.BouncyCastle.Tls;
using Microsoft.EntityFrameworkCore.Internal;
using MediatR;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using static Ubik.Accounting.Api.Features.Accounts.Commands.UpdateAccount;
using Ubik.Accounting.Api.Features.Accounts.Mappers;
using static Ubik.Accounting.Api.Features.Accounts.Commands.DeleteAccount;

namespace Ubik.Accounting.Api.Tests.Integration.Features.Accounts
{
    public class AccountController_Test : BaseIntegrationTest
    {
        private readonly DbInitializer _testDBValues;


        public AccountController_Test(IntegrationTestWebAppFactory factory) : base(factory)
        {
            _testDBValues = new DbInitializer();
        }

        [Fact]
        public async Task Get_Accounts_Ok()
        {
            //Arrange
            var httpClient = Factory.CreateDefaultClient();

            //Act
            var response = await httpClient.GetAsync("/Account");
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

            //Act
            var response = await httpClient.GetAsync($"/Account/{_testDBValues.AccountId1}");
            var result = await response.Content.ReadFromJsonAsync<GetAccountResult>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Should()
                .NotBeNull()
                .And.BeOfType<GetAccountResult>()
                .And.Match<GetAccountResult>(x => x.Code == "1020");
        }

        [Fact]
        public async Task Get_ProblemDetails_AccountIdNotFound()
        {
            //Arrange
            var httpClient = Factory.CreateDefaultClient();

            //Act
            var response = await httpClient.GetAsync($"/Account/{Guid.NewGuid()}");
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

            //Act
            var fakeAc = FakeGenerator.GenerateAccounts(1, _testDBValues.AccountGroupId1).First();
            var fake = fakeAc.ToAddAccountResult();
            var postAccountJson = JsonSerializer.Serialize(fake);
            var content = new StringContent(postAccountJson.ToString(), Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync($"/Account", content);
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
        public async Task Post_ProblemDetails_AccountCodeExist()
        {
            //Arrange
            var httpClient = Factory.CreateDefaultClient();

            //Act
            var fakeAc = FakeGenerator.GenerateAccounts(1, _testDBValues.AccountGroupId1).First();
            var fake = fakeAc.ToAddAccountResult();
            fake.Code = _testDBValues.AccountCode1;

            var postAccountJson = JsonSerializer.Serialize(fake);
            var content = new StringContent(postAccountJson.ToString(), Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync($"/Account", content);
            var result = await response.Content.ReadFromJsonAsync<CustomProblemDetails>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Conflict);
            result.Should()
                .NotBeNull()
                .And.BeOfType<CustomProblemDetails>()
                .And.Match<CustomProblemDetails>(x => x.Errors.First().Code == "ACCOUNT_ALREADY_EXISTS");
        }

        [Fact]
        public async Task Post_ProblemDetails_AccountEmptyFields()
        {
            //Arrange
            var httpClient = Factory.CreateDefaultClient();

            //Act
            var fakeAc = FakeGenerator.GenerateAccounts(1, _testDBValues.AccountGroupId1).First();
            var fake = fakeAc.ToAddAccountResult();

            fake.Code = "";
            fake.Label = "";
            fake.AccountGroupId = default!;

            var postAccountJson = JsonSerializer.Serialize(fake);
            var content = new StringContent(postAccountJson.ToString(), Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync($"/Account", content);
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

            //Act
            var fakeAc = FakeGenerator.GenerateAccounts(1, _testDBValues.AccountGroupId1).First();
            var fake = fakeAc.ToAddAccountResult();

            fake.Code = new string(new Faker("fr_CH").Random.Chars(count: 21));
            fake.Label = new string(new Faker("fr_CH").Random.Chars(count: 101));
            fake.Description = new string(new Faker("fr_CH").Random.Chars(count: 701));

            var postAccountJson = JsonSerializer.Serialize(fake);
            var content = new StringContent(postAccountJson.ToString(), Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync($"/Account", content);
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

            //Act
            var fakeAc = FakeGenerator.GenerateAccounts(1, _testDBValues.AccountGroupId1).First();
            var fake = fakeAc.ToAddAccountResult();

            var responseGet = await httpClient.GetAsync($"/Account/{_testDBValues.AccountId1}");
            var resultGet = await responseGet.Content.ReadFromJsonAsync<GetAccountResult>();

            fake.Version = resultGet!.Version;
            fake.Id = resultGet!.Id;
            fake.Code = _testDBValues.AccountCode1;

            var postAccountJson = JsonSerializer.Serialize(fake);
            var content = new StringContent(postAccountJson.ToString(), Encoding.UTF8, "application/json");

            var response = await httpClient.PutAsync($"/Account/{_testDBValues.AccountId1}", content);
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
        public async Task Put_ProblemDetails_AccountEmptyFields()
        {
            //Arrange
            var httpClient = Factory.CreateDefaultClient();

            //Act
            var fakeAc = FakeGenerator.GenerateAccounts(1, _testDBValues.AccountGroupId1).First();
            var fake = fakeAc.ToAddAccountResult();

            var responseGet = await httpClient.GetAsync($"/Account/{_testDBValues.AccountId1}");
            var resultGet = await responseGet.Content.ReadFromJsonAsync<GetAccountResult>();

            fake.Id = default!;
            fake.Version = default!;
            fake.Code = string.Empty;
            fake.Label = string.Empty;
            fake.AccountGroupId = default!;

            var postAccountJson = JsonSerializer.Serialize(fake);
            var content = new StringContent(postAccountJson.ToString(), Encoding.UTF8, "application/json");

            var response = await httpClient.PutAsync($"/Account/{_testDBValues.AccountId1}", content);
            var result = await response.Content.ReadFromJsonAsync<CustomProblemDetails>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            result.Should()
                .NotBeNull()
                .And.BeOfType<CustomProblemDetails>()
                .And.Match<CustomProblemDetails>(x => x.Errors.First().Code == "VALIDATION_ERROR" && x.Errors.Count() == 4);
        }

        [Fact]
        public async Task Put_ProblemDetails_AccountTooLongFields()
        {
            //Arrange
            var httpClient = Factory.CreateDefaultClient();

            //Act
            var fakeAc = FakeGenerator.GenerateAccounts(1, _testDBValues.AccountGroupId1).First();
            var fake = fakeAc.ToAddAccountResult();

            var responseGet = await httpClient.GetAsync($"/Account/{_testDBValues.AccountId1}");
            var resultGet = await responseGet.Content.ReadFromJsonAsync<GetAccountResult>();

            fake.Code = new string(new Faker("fr_CH").Random.Chars(count: 21));
            fake.Label = new string(new Faker("fr_CH").Random.Chars(count: 101));
            fake.Description = new string(new Faker("fr_CH").Random.Chars(count: 701));
            fake.Version = resultGet!.Version;

            var postAccountJson = JsonSerializer.Serialize(fake);
            var content = new StringContent(postAccountJson.ToString(), Encoding.UTF8, "application/json");

            var response = await httpClient.PutAsync($"/Account/{_testDBValues.AccountId1}", content);
            var result = await response.Content.ReadFromJsonAsync<CustomProblemDetails>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            result.Should()
                .NotBeNull()
                .And.BeOfType<CustomProblemDetails>()
                .And.Match<CustomProblemDetails>(x => x.Errors.First().Code == "VALIDATION_ERROR" && x.Errors.Count() == 3);
        }

        [Fact]
        public async Task Put_ProblemDetails_AccountCodeExistsWithDifferentId()
        {
            //Arrange
            var httpClient = Factory.CreateDefaultClient();

            //Act
            var fakeAc = FakeGenerator.GenerateAccounts(1, _testDBValues.AccountGroupId1).First();
            var fake = fakeAc.ToAddAccountResult();

            var responseGet = await httpClient.GetAsync($"/Account/{_testDBValues.AccountId2}");
            var resultGet = await responseGet.Content.ReadFromJsonAsync<GetAccountResult>();

            fake.Id = resultGet!.Id;
            fake.Code = "1020";
            fake.Version = resultGet!.Version;

            var postAccountJson = JsonSerializer.Serialize(fake);
            var content = new StringContent(postAccountJson.ToString(), Encoding.UTF8, "application/json");

            var response = await httpClient.PutAsync($"/Account/{_testDBValues.AccountId2}", content);
            var result = await response.Content.ReadFromJsonAsync<CustomProblemDetails>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Conflict);
            result.Should()
                .NotBeNull()
                .And.BeOfType<CustomProblemDetails>()
                .And.Match<CustomProblemDetails>(x => x.Errors.First().Code == "ACCOUNT_ALREADY_EXISTS");
        }

        [Fact]
        public async Task Put_ProblemDetails_AccountIdNotFound()
        {
            //Arrange
            var httpClient = Factory.CreateDefaultClient();

            //Act
            var fakeAc = FakeGenerator.GenerateAccounts(1, _testDBValues.AccountGroupId1).First();
            var fake = fakeAc.ToAddAccountResult();

            var responseGet = await httpClient.GetAsync($"/Account/{_testDBValues.AccountId2}");
            var resultGet = await responseGet.Content.ReadFromJsonAsync<GetAccountResult>();

            fake.Id = Guid.NewGuid();
            fake.Version = resultGet!.Version;

            var postAccountJson = JsonSerializer.Serialize(fake);
            var content = new StringContent(postAccountJson.ToString(), Encoding.UTF8, "application/json");

            var response = await httpClient.PutAsync($"/Account/{fake.Id}", content);
            var result = await response.Content.ReadFromJsonAsync<CustomProblemDetails>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            result.Should()
                .NotBeNull()
                .And.BeOfType<CustomProblemDetails>()
                .And.Match<CustomProblemDetails>(x => x.Errors.First().Code == "ACCOUNT_NOT_FOUND");
        }

        [Fact]
        public async Task Put_ProblemDetails_AccountModifiedByAnotherProcess()
        {
            //Arrange
            var httpClient = Factory.CreateDefaultClient();

            //Act
            var fakeAc = FakeGenerator.GenerateAccounts(1, _testDBValues.AccountGroupId1).First();
            var fake = fakeAc.ToAddAccountResult();

            var responseGet = await httpClient.GetAsync($"/Account/{_testDBValues.AccountId1}");
            var resultGet = await responseGet.Content.ReadFromJsonAsync<GetAccountResult>();

            fake.Version = resultGet!.Version;
            fake.Id = resultGet!.Id;
            fake.Code = _testDBValues.AccountCode1;

            var postAccountJson = JsonSerializer.Serialize(fake);
            var content = new StringContent(postAccountJson.ToString(), Encoding.UTF8, "application/json");

            var response = await httpClient.PutAsync($"/Account/{_testDBValues.AccountId1}", content);
            var response2 = await httpClient.PutAsync($"/Account/{_testDBValues.AccountId1}", content);
            var result = await response2.Content.ReadFromJsonAsync<CustomProblemDetails>();

            //Assert
            response2.StatusCode.Should().Be(HttpStatusCode.Conflict);
            result.Should()
                .NotBeNull()
                .And.BeOfType<CustomProblemDetails>()
                .And.Match<CustomProblemDetails>(x => x.Errors.First().Code == "ACCOUNT_CONFLICT");
        }

        [Fact]
        public async Task Del_NoContent_Ok()
        {
            //Arrange
            var httpClient = Factory.CreateDefaultClient();

            //Act
            var response = await httpClient.DeleteAsync($"/Account/{_testDBValues.AccountIdForDel}");

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task Del_ProblemDetails_AccountIdNotFound()
        {
            //Arrange
            var httpClient = Factory.CreateDefaultClient();

            //Act
            var response = await httpClient.DeleteAsync($"/Account/{Guid.NewGuid()}");
            var result = await response.Content.ReadFromJsonAsync<CustomProblemDetails>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            result.Should()
                .NotBeNull()
                .And.BeOfType<CustomProblemDetails>()
                .And.Match<CustomProblemDetails>(x => x.Errors.First().Code == "ACCOUNT_NOT_FOUND");
        }

        [Fact]
        public async Task Del_ProblemDetails_AccountIdEmpty()
        {
            //Arrange
            var httpClient = Factory.CreateDefaultClient();
            Guid empty = default!;

            //Act
            var response = await httpClient.DeleteAsync($"/Account/{empty}");
            var result = await response.Content.ReadFromJsonAsync<CustomProblemDetails>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            result.Should()
                .NotBeNull()
                .And.BeOfType<CustomProblemDetails>()
                .And.Match<CustomProblemDetails>(x => x.Errors.First().Code == "VALIDATION_ERROR");
        }

        private async Task<string> GetAccessToken()
        {
            var tokenUrl = Environment.GetEnvironmentVariable("TokenUrl");
            var httpClient = Factory.CreateDefaultClient();
            httpClient.DefaultRequestHeaders.Add("Content-Type", "application/x-www-form-urlencoded");

            var keyValuesList = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("grant_type", "password"),
                new KeyValuePair<string, string>("username", "admin"),
                new KeyValuePair<string, string>("password", "admin")
            };
            var content = new FormUrlEncodedContent(keyValuesList);

            var response = await httpClient.PutAsync(tokenUrl, content);

            return await response.Content.ReadAsStringAsync();

        }
    }
}
