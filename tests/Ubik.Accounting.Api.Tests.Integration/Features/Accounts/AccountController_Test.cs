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
            var fake = FakeGenerator.GenerateAccounts(1, _testDBValues.AccountGroupId1);
            var postAccountJson = JsonSerializer.Serialize(fake.First());
            var content = new StringContent(postAccountJson.ToString(), Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync($"/Account", content);
            var result = await response.Content.ReadFromJsonAsync<AddAccountResult>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            result.Should()
                .NotBeNull()
                .And.BeOfType<AddAccountResult>()
                .And.Match<AddAccountResult>(x => x.Code == fake.First().Code);
        }

        [Fact]
        public async Task Post_ProblemDetails_AccountCodeExist()
        {
            //Arrange
            var httpClient = Factory.CreateDefaultClient();

            //Act
            var fake = FakeGenerator.GenerateAccounts(1, _testDBValues.AccountGroupId1).First();
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
            var fake = FakeGenerator.GenerateAccounts(1, _testDBValues.AccountGroupId1).First();

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
    }
}
