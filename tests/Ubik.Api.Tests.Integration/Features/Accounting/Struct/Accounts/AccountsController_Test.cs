using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Ubik.Accounting.Structure.Contracts.Accounts.Results;
using Ubik.ApiService.Common.Exceptions;
using Ubik.Accounting.Structure.Contracts.Accounts.Commands;
using Ubik.Accounting.Structure.Contracts.Accounts.Enums;
using MassTransit;

namespace Ubik.Api.Tests.Integration.Features.Accounting.Struct.Accounts
{
    //TODO: write a test case when account is linked to existing accounting entries
    public class AccountsController_Test : BaseIntegrationTestAccountingStruct
    {
        private readonly string _baseUrlForV1;
        private readonly HttpClient _client;
        private readonly static Guid _accountId = new("248e0000-5dd4-0015-ebad-08dcd98b0949");
        private readonly static Guid _accountToDel = new("ec860000-5dd4-0015-e4a1-08dcda3073d0");

        public AccountsController_Test(IntegrationTestProxyFactory factory) : base(factory)
        {
            _baseUrlForV1 = "/accounting/api/v1/accounts";
            _client = Factory.CreateDefaultClient();
        }

        [Fact]
        public async Task Get_Accounts_All_WithRW_OK()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.RW);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.GetAsync(_baseUrlForV1);
            var result = await response.Content.ReadFromJsonAsync<List<AccountStandardResult>>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Should()
                .NotBeNull()
                .And.BeOfType<List<AccountStandardResult>>();
        }

        [Fact]
        public async Task Get_Accounts_All_WithRO_OK()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.RO);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.GetAsync(_baseUrlForV1);
            var result = await response.Content.ReadFromJsonAsync<List<AccountStandardResult>>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Should()
                .NotBeNull()
                .And.BeOfType<List<AccountStandardResult>>();
        }

        [Fact]
        public async Task Get_Accounts_All_WithNoToken_401()
        {
            //Act
            var response = await _client.GetAsync(_baseUrlForV1);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Get_Accounts_All_WithOtherTenant_OK()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.OtherTenant);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.GetAsync(_baseUrlForV1);
            var result = await response.Content.ReadFromJsonAsync<List<AccountStandardResult>>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Should()
                .NotBeNull()
                .And.BeOfType<List<AccountStandardResult>>()
                .And.Match<List<AccountStandardResult>>(x => x.Count == 0);
        }

        [Fact]
        public async Task Get_Accounts_All_WithNoRole_403()
        {             //Arrange
            var token = await GetAccessTokenAsync(TokenType.NoRole);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.GetAsync(_baseUrlForV1);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Get_Accounts_All_WithAdmin_403()
        {             //Arrange
            var token = await GetAccessTokenAsync(TokenType.MegaAdmin);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.GetAsync(_baseUrlForV1);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Get_Account_By_Id_WithRW_OK()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.RW);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.GetAsync($"{_baseUrlForV1}/{_accountId}");
            var result = await response.Content.ReadFromJsonAsync<AccountStandardResult>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Should()
                .NotBeNull()
                .And.BeOfType<AccountStandardResult>()
                .And.Match<AccountStandardResult>(x => x.Id == _accountId);
        }

        [Fact]
        public async Task Get_Account_By_Id_WithRO_OK()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.RO);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.GetAsync($"{_baseUrlForV1}/{_accountId}");
            var result = await response.Content.ReadFromJsonAsync<AccountStandardResult>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Should()
                .NotBeNull()
                .And.BeOfType<AccountStandardResult>()
                .And.Match<AccountStandardResult>(x => x.Id == _accountId);
        }

        [Fact]
        public async Task Get_Account_By_Id_WithNoToken_401()
        {
            //Act
            var response = await _client.GetAsync($"{_baseUrlForV1}/{_accountId}");

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Get_Account_By_Id_WithOtherTenant_404()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.OtherTenant);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.GetAsync($"{_baseUrlForV1}/{_accountId}");
            var result = await response.Content.ReadFromJsonAsync<CustomProblemDetails>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            result.Should()
                .NotBeNull()
                .And.BeOfType<CustomProblemDetails>()
                .And.Match<CustomProblemDetails>(x => x.Errors.First().Code == "ACCOUNT_NOT_FOUND");
        }

        [Fact]
        public async Task Get_Account_By_Id_WithNoRole_403()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.NoRole);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.GetAsync($"{_baseUrlForV1}/{_accountId}");

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Get_Account_By_Id_WithAdmin_403()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.MegaAdmin);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.GetAsync($"{_baseUrlForV1}/{_accountId}");

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Get_Account_By_Id_WithBadId_404()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.RW);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.GetAsync($"{_baseUrlForV1}/{Guid.NewGuid()}");
            var result = await response.Content.ReadFromJsonAsync<CustomProblemDetails>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            result.Should()
                .NotBeNull()
                .And.BeOfType<CustomProblemDetails>()
                .And.Match<CustomProblemDetails>(x => x.Errors.First().Code == "ACCOUNT_NOT_FOUND");
        }

        [Fact]
        public async Task Get_All_AccountGroupLinks_WithRW_OK()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.RW);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.GetAsync($"{_baseUrlForV1}/accountgrouplinks");
            var result = await response.Content.ReadFromJsonAsync<List<AccountGroupLinkResult>>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Should()
                .NotBeNull()
                .And.BeOfType<List<AccountGroupLinkResult>>()
                .And.Match<List<AccountGroupLinkResult>>(x => x.Count > 0);
        }

        [Fact]
        public async Task Get_All_AccountGroupLinks_WithRO_OK()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.RO);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.GetAsync($"{_baseUrlForV1}/accountgrouplinks");
            var result = await response.Content.ReadFromJsonAsync<List<AccountGroupLinkResult>>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Should()
                .NotBeNull()
                .And.BeOfType<List<AccountGroupLinkResult>>()
                .And.Match<List<AccountGroupLinkResult>>(x => x.Count > 0);
        }

        [Fact]
        public async Task Get_All_AccountGroupLinks_WithNoAuth_401()
        {
            //Act
            var response = await _client.GetAsync($"{_baseUrlForV1}/accountgrouplinks");

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Get_All_AccountGroupLinks_WithOtherTenant_OK()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.OtherTenant);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.GetAsync($"{_baseUrlForV1}/accountgrouplinks");
            var result = await response.Content.ReadFromJsonAsync<List<AccountGroupLinkResult>>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Should()
                .NotBeNull()
                .And.BeOfType<List<AccountGroupLinkResult>>()
                .And.Match<List<AccountGroupLinkResult>>(x => x.Count == 0);
        }

        [Fact]
        public async Task Get_All_AccountGroupLinks_WithNoRole_403()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.NoRole);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.GetAsync($"{_baseUrlForV1}/accountgrouplinks");

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Get_All_AccountGroupLinks_WithAdmin_403()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.MegaAdmin);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.GetAsync($"{_baseUrlForV1}/accountgrouplinks");

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Get_All_AccountGroupsWithClassification_WithRW_OK()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.RW);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.GetAsync($"{_baseUrlForV1}/{_accountId}/accountgroups");
            var result = await response.Content.ReadFromJsonAsync<List<AccountGroupWithClassificationResult>>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Should()
                .NotBeNull()
                .And.BeOfType<List<AccountGroupWithClassificationResult>>()
                .And.Match<List<AccountGroupWithClassificationResult>>(x => x.Count > 0);
        }

        [Fact]
        public async Task Get_All_AccountGroupsWithClassification_WithRO_OK()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.RO);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.GetAsync($"{_baseUrlForV1}/{_accountId}/accountgroups");
            var result = await response.Content.ReadFromJsonAsync<List<AccountGroupWithClassificationResult>>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Should()
                .NotBeNull()
                .And.BeOfType<List<AccountGroupWithClassificationResult>>()
                .And.Match<List<AccountGroupWithClassificationResult>>(x => x.Count > 0);
        }

        [Fact]
        public async Task Get_All_AccountGroupsWithClassification_WithNoAuth_401()
        {
            //Act
            var response = await _client.GetAsync($"{_baseUrlForV1}/{_accountId}/accountgroups");

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Get_All_AccountGroupsWithClassification_WithOtherTenant_404()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.OtherTenant);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.GetAsync($"{_baseUrlForV1}/{_accountId}/accountgroups");
            var result = await response.Content.ReadFromJsonAsync<CustomProblemDetails>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            result.Should()
                .NotBeNull()
                .And.BeOfType<CustomProblemDetails>()
                .And.Match<CustomProblemDetails>(x => x.Errors.First().Code == "ACCOUNT_NOT_FOUND");
        }

        [Fact]
        public async Task Get_All_AccountGroupsWithClassification_WithNoRole_403()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.NoRole);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.GetAsync($"{_baseUrlForV1}/{_accountId}/accountgroups");

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Get_All_AccountGroupsWithClassification_WithAdmin_403()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.MegaAdmin);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.GetAsync($"{_baseUrlForV1}/{_accountId}/accountgroups");

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Add_Account_WithRW_OK()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.RW);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var newAccount = new AddAccountCommand
            {
                Code = "ZZZAAA",
                Description = "Test Account",
                Domain = AccountDomain.Asset,
                Category = AccountCategory.Liquidity,
                CurrencyId = new Guid("248e0000-5dd4-0015-38c5-08dcd98e5b2d"),
                Label = "Test Account",
            };

            //Act
            var response = await _client.PostAsJsonAsync(_baseUrlForV1, newAccount);
            var result = await response.Content.ReadFromJsonAsync<AccountStandardResult>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            result.Should()
                .NotBeNull()
                .And.BeOfType<AccountStandardResult>()
                .And.Match<AccountStandardResult>(x => x.Code == "ZZZAAA");
        }

        [Fact]
        public async Task Add_Account_WithRO_403()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.RO);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var newAccount = new AddAccountCommand
            {
                Code = "ZZZAAA",
                Description = "Test Account",
                Domain = AccountDomain.Asset,
                Category = AccountCategory.Liquidity,
                CurrencyId = new Guid("248e0000-5dd4-0015-38c5-08dcd98e5b2d"),
                Label = "Test Account",
            };

            //Act
            var response = await _client.PostAsJsonAsync(_baseUrlForV1, newAccount);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Add_Account_WithNoAuth_401()
        {
            //Arrange
            var newAccount = new AddAccountCommand
            {
                Code = "ZZZAAA",
                Description = "Test Account",
                Domain = AccountDomain.Asset,
                Category = AccountCategory.Liquidity,
                CurrencyId = new Guid("248e0000-5dd4-0015-38c5-08dcd98e5b2d"),
                Label = "Test Account",
            };

            //Act
            var response = await _client.PostAsJsonAsync(_baseUrlForV1, newAccount);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Add_Account_WithOtherTenant_400()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.OtherTenant);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var newAccount = new AddAccountCommand
            {
                Code = "ZZZAAA",
                Description = "Test Account",
                Domain = AccountDomain.Asset,
                Category = AccountCategory.Liquidity,
                CurrencyId = new Guid("248e0000-5dd4-0015-38c5-08dcd98e5b2d"),
                Label = "Test Account",
            };

            //Act
            var response = await _client.PostAsJsonAsync(_baseUrlForV1, newAccount);
            var result = await response.Content.ReadFromJsonAsync<CustomProblemDetails>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            result.Should()
                .NotBeNull()
                .And.BeOfType<CustomProblemDetails>()
                .And.Match<CustomProblemDetails>(e => e.Errors.First().Code == "ACCOUNT_CURRENCY_NOT_FOUND");
        }

        [Fact]
        public async Task Add_Account_WithNoRole_403()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.NoRole);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var newAccount = new AddAccountCommand
            {
                Code = "ZZZAAA",
                Description = "Test Account",
                Domain = AccountDomain.Asset,
                Category = AccountCategory.Liquidity,
                CurrencyId = new Guid("248e0000-5dd4-0015-38c5-08dcd98e5b2d"),
                Label = "Test Account",
            };

            //Act
            var response = await _client.PostAsJsonAsync(_baseUrlForV1, newAccount);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Add_Account_WithAdmin_403()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.MegaAdmin);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var newAccount = new AddAccountCommand
            {
                Code = "ZZZAAA",
                Description = "Test Account",
                Domain = AccountDomain.Asset,
                Category = AccountCategory.Liquidity,
                CurrencyId = new Guid("248e0000-5dd4-0015-38c5-08dcd98e5b2d"),
                Label = "Test Account",
            };

            //Act
            var response = await _client.PostAsJsonAsync(_baseUrlForV1, newAccount);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Add_Account_WithExistingCode_409()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.RW);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var newAccount = new AddAccountCommand
            {
                Code = "1000",
                Description = "Test Account",
                Domain = AccountDomain.Asset,
                Category = AccountCategory.Liquidity,
                CurrencyId = new Guid("248e0000-5dd4-0015-38c5-08dcd98e5b2d"),
                Label = "Test Account",
            };

            //Act
            var response = await _client.PostAsJsonAsync(_baseUrlForV1, newAccount);
            var result = await response.Content.ReadFromJsonAsync<CustomProblemDetails>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Conflict);
            result.Should()
                .NotBeNull()
                .And.BeOfType<CustomProblemDetails>()
                .And.Match<CustomProblemDetails>(e => e.Errors.First().Code == "ACCOUNT_ALREADY_EXISTS");
        }

        [Fact]
        public async Task Add_Account_WithBadCurrency_400()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.RW);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var newAccount = new AddAccountCommand
            {
                Code = "78zrt",
                Description = "Test Account",
                Domain = AccountDomain.Asset,
                Category = AccountCategory.Liquidity,
                CurrencyId = NewId.NextGuid(),
                Label = "Test Account",
            };

            //Act
            var response = await _client.PostAsJsonAsync(_baseUrlForV1, newAccount);
            var result = await response.Content.ReadFromJsonAsync<CustomProblemDetails>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            result.Should()
                .NotBeNull()
                .And.BeOfType<CustomProblemDetails>()
                .And.Match<CustomProblemDetails>(e => e.Errors.First().Code == "ACCOUNT_CURRENCY_NOT_FOUND");
        }

        [Fact]
        public async Task Update_Account_WithRW_OK()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.RW);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var updateAccount = new UpdateAccountCommand
            {
                Id = new Guid("ec860000-5dd4-0015-1cc9-08dcda20a76e"),
                Code = "ZZZAAA1",
                Description = "Test Account",
                Domain = AccountDomain.Asset,
                Category = AccountCategory.Liquidity,
                CurrencyId = new Guid("248e0000-5dd4-0015-38c5-08dcd98e5b2d"),
                Label = "Test Account",
                Version = new Guid("ec860000-5dd4-0015-234c-08dcda20a76e")
            };

            //Act
            var response = await _client.PutAsJsonAsync($"{_baseUrlForV1}/ec860000-5dd4-0015-1cc9-08dcda20a76e", updateAccount);
            var result = await response.Content.ReadFromJsonAsync<AccountStandardResult>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Should()
                .NotBeNull()
                .And.BeOfType<AccountStandardResult>()
                .And.Match<AccountStandardResult>(x => x.Code == "ZZZAAA1");
        }

        [Fact]
        public async Task Update_Account_WithRO_403()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.RO);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var updateAccount = new UpdateAccountCommand
            {
                Id = new Guid("ec860000-5dd4-0015-1cc9-08dcda20a76e"),
                Code = "ZZZAAA1",
                Description = "Test Account",
                Domain = AccountDomain.Asset,
                Category = AccountCategory.Liquidity,
                CurrencyId = new Guid("248e0000-5dd4-0015-38c5-08dcd98e5b2d"),
                Label = "Test Account",
                Version = new Guid("ec860000-5dd4-0015-234c-08dcda20a76e")
            };

            //Act
            var response = await _client.PutAsJsonAsync($"{_baseUrlForV1}/{_accountId}", updateAccount);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Update_Account_WithNoAuth_401()
        {
            //Arrange
            var updateAccount = new UpdateAccountCommand
            {
                Id = new Guid("ec860000-5dd4-0015-1cc9-08dcda20a76e"),
                Code = "ZZZAAA1",
                Description = "Test Account",
                Domain = AccountDomain.Asset,
                Category = AccountCategory.Liquidity,
                CurrencyId = new Guid("248e0000-5dd4-0015-38c5-08dcd98e5b2d"),
                Label = "Test Account",
                Version = new Guid("ec860000-5dd4-0015-234c-08dcda20a76e")
            };

            //Act
            var response = await _client.PutAsJsonAsync($"{_baseUrlForV1}/{_accountId}", updateAccount);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Update_Account_WithOtherTenant_404()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.OtherTenant);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var updateAccount = new UpdateAccountCommand
            {
                Id = new Guid("ec860000-5dd4-0015-1cc9-08dcda20a76e"),
                Code = "ZZZAAA1",
                Description = "Test Account",
                Domain = AccountDomain.Asset,
                Category = AccountCategory.Liquidity,
                CurrencyId = new Guid("248e0000-5dd4-0015-38c5-08dcd98e5b2d"),
                Label = "Test Account",
                Version = new Guid("ec860000-5dd4-0015-234c-08dcda20a76e")
            };

            //Act
            var response = await _client.PutAsJsonAsync($"{_baseUrlForV1}/ec860000-5dd4-0015-1cc9-08dcda20a76e", updateAccount);
            var result = await response.Content.ReadFromJsonAsync<CustomProblemDetails>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            result.Should()
                .NotBeNull()
                .And.BeOfType<CustomProblemDetails>()
                .And.Match<CustomProblemDetails>(x => x.Errors.First().Code == "ACCOUNT_NOT_FOUND");
        }

        [Fact]
        public async Task Update_Account_WithNoRole_403()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.NoRole);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var updateAccount = new UpdateAccountCommand
            {
                Id = new Guid("ec860000-5dd4-0015-1cc9-08dcda20a76e"),
                Code = "ZZZAAA1",
                Description = "Test Account",
                Domain = AccountDomain.Asset,
                Category = AccountCategory.Liquidity,
                CurrencyId = new Guid("248e0000-5dd4-0015-38c5-08dcd98e5b2d"),
                Label = "Test Account",
                Version = new Guid("ec860000-5dd4-0015-234c-08dcda20a76e")
            };

            //Act
            var response = await _client.PutAsJsonAsync($"{_baseUrlForV1}/{_accountId}", updateAccount);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Update_Account_WithAdmin_403()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.MegaAdmin);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var updateAccount = new UpdateAccountCommand
            {
                Id = new Guid("ec860000-5dd4-0015-1cc9-08dcda20a76e"),
                Code = "ZZZAAA1",
                Description = "Test Account",
                Domain = AccountDomain.Asset,
                Category = AccountCategory.Liquidity,
                CurrencyId = new Guid("248e0000-5dd4-0015-38c5-08dcd98e5b2d"),
                Label = "Test Account",
                Version = new Guid("ec860000-5dd4-0015-234c-08dcda20a76e")
            };

            //Act
            var response = await _client.PutAsJsonAsync($"{_baseUrlForV1}/{_accountId}", updateAccount);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Update_Account_WithBadId_404()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.RW);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var id = NewId.NextGuid();
            var updateAccount = new UpdateAccountCommand
            {
                Id = id,
                Code = "ZZZAAA1",
                Description = "Test Account",
                Domain = AccountDomain.Asset,
                Category = AccountCategory.Liquidity,
                CurrencyId = new Guid("248e0000-5dd4-0015-38c5-08dcd98e5b2d"),
                Label = "Test Account",
                Version = new Guid("ec860000-5dd4-0015-234c-08dcda20a76e")
            };

            //Act
            var response = await _client.PutAsJsonAsync($"{_baseUrlForV1}/{id}", updateAccount);
            var result = await response.Content.ReadFromJsonAsync<CustomProblemDetails>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            result.Should()
                .NotBeNull()
                .And.BeOfType<CustomProblemDetails>()
                .And.Match<CustomProblemDetails>(x => x.Errors.First().Code == "ACCOUNT_NOT_FOUND");
        }

        [Fact]
        public async Task Update_Account_WithIDNotMatch_400()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.RW);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var updateAccount = new UpdateAccountCommand
            {
                Id = new Guid("ec860000-5dd4-0015-1cc9-08dcda20a76e"),
                Code = "ZZZAAA1",
                Description = "Test Account",
                Domain = AccountDomain.Asset,
                Category = AccountCategory.Liquidity,
                CurrencyId = new Guid("248e0000-5dd4-0015-38c5-08dcd98e5b2d"),
                Label = "Test Account",
                Version = new Guid("ec860000-5dd4-0015-234c-08dcda20a76e")
            };

            //Act
            var response = await _client.PutAsJsonAsync($"{_baseUrlForV1}/{NewId.NextGuid()}", updateAccount);
            var result = await response.Content.ReadFromJsonAsync<CustomProblemDetails>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            result.Should()
                .NotBeNull()
                .And.BeOfType<CustomProblemDetails>()
                .And.Match<CustomProblemDetails>(x => x.Errors.First().Code == "ACCOUNT_COMMAND_IDS_NOT_MATCH");
        }

        [Fact]
        public async Task Update_Account_WithAlreadyExists_409()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.RW);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var updateAccount = new UpdateAccountCommand
            {
                Id = new Guid("ec860000-5dd4-0015-ce96-08dcda306be5"),
                Code = "2005",
                Description = "Test Account",
                Domain = AccountDomain.Asset,
                Category = AccountCategory.Liquidity,
                CurrencyId = new Guid("248e0000-5dd4-0015-38c5-08dcd98e5b2d"),
                Label = "Test Account",
                Version = new Guid("ec860000-5dd4-0015-0a00-08dcda307df9")
            };

            //Act
            var response = await _client.PutAsJsonAsync($"{_baseUrlForV1}/ec860000-5dd4-0015-ce96-08dcda306be5", updateAccount);
            var result = await response.Content.ReadFromJsonAsync<CustomProblemDetails>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Conflict);
            result.Should()
                .NotBeNull()
                .And.BeOfType<CustomProblemDetails>()
                .And.Match<CustomProblemDetails>(x => x.Errors.First().Code == "ACCOUNT_ALREADY_EXISTS");
        }

        [Fact]
        public async Task Update_Account_WithBadVersion_409()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.RW);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var updateAccount = new UpdateAccountCommand
            {
                Id = new Guid("ec860000-5dd4-0015-ce96-08dcda306be5"),
                Code = "UUUAAA",
                Description = "Test Account",
                Domain = AccountDomain.Asset,
                Category = AccountCategory.Liquidity,
                CurrencyId = new Guid("248e0000-5dd4-0015-38c5-08dcd98e5b2d"),
                Label = "Test Account",
                Version = new Guid("ec850000-5dd4-0015-0a00-08dcda307df9")
            };

            //Act
            var response = await _client.PutAsJsonAsync($"{_baseUrlForV1}/ec860000-5dd4-0015-ce96-08dcda306be5", updateAccount);
            var result = await response.Content.ReadFromJsonAsync<CustomProblemDetails>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Conflict);
            result.Should()
                .NotBeNull()
                .And.BeOfType<CustomProblemDetails>()
                .And.Match<CustomProblemDetails>(x => x.Errors.First().Code == "ACCOUNT_UPDATE_CONCURRENCY");
        }

        [Fact]
        public async Task Update_Account_WithBadCurrency_400()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.RW);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var updateAccount = new UpdateAccountCommand
            {
                Id = new Guid("ec860000-5dd4-0015-ce96-08dcda306be5"),
                Code = "UUUAAA",
                Description = "Test Account",
                Domain = AccountDomain.Asset,
                Category = AccountCategory.Liquidity,
                CurrencyId = new Guid("242e0000-5dd4-0015-38c5-08dcd98e5b2d"),
                Label = "Test Account",
                Version = new Guid("ec850000-5dd4-0015-0a00-08dcda307df9")
            };

            //Act
            var response = await _client.PutAsJsonAsync($"{_baseUrlForV1}/ec860000-5dd4-0015-ce96-08dcda306be5", updateAccount);
            var result = await response.Content.ReadFromJsonAsync<CustomProblemDetails>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            result.Should()
                .NotBeNull()
                .And.BeOfType<CustomProblemDetails>()
                .And.Match<CustomProblemDetails>(x => x.Errors.First().Code == "ACCOUNT_CURRENCY_NOT_FOUND");
        }

        [Fact]
        public async Task Delete_Account_WithRW_OK()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.RW);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.DeleteAsync($"{_baseUrlForV1}/{_accountToDel}");

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task Delete_Account_WithRO_403()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.RO);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.DeleteAsync($"{_baseUrlForV1}/{_accountToDel}");

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Delete_Account_WithNoAuth_401()
        {
            //Act
            var response = await _client.DeleteAsync($"{_baseUrlForV1}/{_accountToDel}");

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Delete_Account_WithOtherTenant_404()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.OtherTenant);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.DeleteAsync($"{_baseUrlForV1}/{_accountToDel}");
            var result = await response.Content.ReadFromJsonAsync<CustomProblemDetails>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            result.Should()
                .NotBeNull()
                .And.BeOfType<CustomProblemDetails>()
                .And.Match<CustomProblemDetails>(x => x.Errors.First().Code == "ACCOUNT_NOT_FOUND");
        }

        [Fact]
        public async Task Delete_Account_WithNoRole_403()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.NoRole);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.DeleteAsync($"{_baseUrlForV1}/{_accountToDel}");

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Delete_Account_WithAdmin_403()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.MegaAdmin);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.DeleteAsync($"{_baseUrlForV1}/{_accountToDel}");

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Delete_Account_WithBadId_404()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.RW);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.DeleteAsync($"{_baseUrlForV1}/{NewId.NextGuid()}");
            var result = await response.Content.ReadFromJsonAsync<CustomProblemDetails>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            result.Should()
                .NotBeNull()
                .And.BeOfType<CustomProblemDetails>()
                .And.Match<CustomProblemDetails>(x => x.Errors.First().Code == "ACCOUNT_NOT_FOUND");
        }

        [Fact]
        public async Task Attach_Account_InAccountgroup_WithRW_OK()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.RW);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.PostAsync($"{_baseUrlForV1}/ec860000-5dd4-0015-ce96-08dcda306be5/accountgroups/4c470000-5dd4-0015-ddd1-08dcdb1e7283", null);
            var result = await response.Content.ReadFromJsonAsync<AccountInAccountGroupResult>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Should()
                .NotBeNull()
                .And.BeOfType<AccountInAccountGroupResult>()
                .And.Match<AccountInAccountGroupResult>(x => x.AccountId == new Guid("ec860000-5dd4-0015-ce96-08dcda306be5"));
        }

        [Fact]
        public async Task Attach_Account_InAccountgroup_WithRO_403()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.RO);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.PostAsync($"{_baseUrlForV1}/14320000-5dd4-0015-8c30-08dcdb1c487d/accountgroups/4c470000-5dd4-0015-ddd1-08dcdb1e7283", null);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Attach_Account_InAccountgroup_WithNoAuth_401()
        {
            //Act
            var response = await _client.PostAsync($"{_baseUrlForV1}/14320000-5dd4-0015-8c30-08dcdb1c487d/accountgroups/4c470000-5dd4-0015-ddd1-08dcdb1e7283", null);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Attach_Account_InAccountgroup_WithOtherTenant_404()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.OtherTenant);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.PostAsync($"{_baseUrlForV1}/14320000-5dd4-0015-8c30-08dcdb1c487d/accountgroups/4c470000-5dd4-0015-ddd1-08dcdb1e7283", null);
            var result = await response.Content.ReadFromJsonAsync<CustomProblemDetails>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            result.Should()
                .NotBeNull()
                .And.BeOfType<CustomProblemDetails>()
                .And.Match<CustomProblemDetails>(x => x.Errors.First().Code == "ACCOUNT_NOT_FOUND");
        }

        [Fact]
        public async Task Attach_Account_InAccountgroup_WithNoRole_403()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.NoRole);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.PostAsync($"{_baseUrlForV1}/14320000-5dd4-0015-8c30-08dcdb1c487d/accountgroups/4c470000-5dd4-0015-ddd1-08dcdb1e7283", null);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Attach_Account_InAccountgroup_WithAdmin_403()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.MegaAdmin);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.PostAsync($"{_baseUrlForV1}/14320000-5dd4-0015-8c30-08dcdb1c487d/accountgroups/4c470000-5dd4-0015-ddd1-08dcdb1e7283", null);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Attach_Account_InAccountgroup_WithBadAccountId_404()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.RW);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.PostAsync($"{_baseUrlForV1}/{NewId.NextGuid()}/accountgroups/4c470000-5dd4-0015-ddd1-08dcdb1e7283", null);
            var result = await response.Content.ReadFromJsonAsync<CustomProblemDetails>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            result.Should()
                .NotBeNull()
                .And.BeOfType<CustomProblemDetails>()
                .And.Match<CustomProblemDetails>(x => x.Errors.First().Code == "ACCOUNT_NOT_FOUND");
        }

        [Fact]
        public async Task Attach_Account_InAccountgroup_WithBadAccountGroupId_400()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.RW);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.PostAsync($"{_baseUrlForV1}/14320000-5dd4-0015-8c30-08dcdb1c487d/accountgroups/{NewId.NextGuid()}", null);
            var result = await response.Content.ReadFromJsonAsync<CustomProblemDetails>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            result.Should()
                .NotBeNull()
                .And.BeOfType<CustomProblemDetails>()
                .And.Match<CustomProblemDetails>(x => x.Errors.First().Code == "ACCOUNT_ACCOUNTGROUP_NOT_FOUND");
        }

        [Fact]
        public async Task Attach_Account_InAccountgroup_WithAccountGroupNotExistInClassification_409()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.RW);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.PostAsync($"{_baseUrlForV1}/248e0000-5dd4-0015-ebad-08dcd98b0949/accountgroups/cc100000-5dd4-0015-ca0c-08dcd967caca", null);
            var result = await response.Content.ReadFromJsonAsync<CustomProblemDetails>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Conflict);
            result.Should()
                .NotBeNull()
                .And.BeOfType<CustomProblemDetails>()
                .And.Match<CustomProblemDetails>(x => x.Errors.First().Code == "ACCOUNT_ALREADY_EXISTS_IN_CLASSIFICATION");
        }

        [Fact]
        public async Task Detach_Account_InAccountgroup_WithRW_OK()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.RW);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.DeleteAsync($"{_baseUrlForV1}/ec860000-5dd4-0015-11c5-08dcda1de501/accountgroups/ec860000-5dd4-0015-7da9-08dcda1e1e40");

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task Detach_Account_InAccountgroup_WithRO_403()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.RO);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.DeleteAsync($"{_baseUrlForV1}/ec860000-5dd4-0015-11c5-08dcda1de501/accountgroups/ec860000-5dd4-0015-7da9-08dcda1e1e40");

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Detach_Account_InAccountgroup_WithNoAuth_401()
        {
            //Act
            var response = await _client.DeleteAsync($"{_baseUrlForV1}/ec860000-5dd4-0015-11c5-08dcda1de501/accountgroups/ec860000-5dd4-0015-7da9-08dcda1e1e40");

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Detach_Account_InAccountgroup_WithOtherTenant_404()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.OtherTenant);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.DeleteAsync($"{_baseUrlForV1}/ec860000-5dd4-0015-11c5-08dcda1de501/accountgroups/ec860000-5dd4-0015-7da9-08dcda1e1e40");
            var result = await response.Content.ReadFromJsonAsync<CustomProblemDetails>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            result.Should()
                .NotBeNull()
                .And.BeOfType<CustomProblemDetails>()
                .And.Match<CustomProblemDetails>(x => x.Errors.First().Code == "ACCOUNT_NOT_EXISTS_IN_ACCOUNTGROUP");
        }

        [Fact]
        public async Task Detach_Account_InAccountgroup_WithNoRole_403()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.NoRole);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.DeleteAsync($"{_baseUrlForV1}/ec860000-5dd4-0015-11c5-08dcda1de501/accountgroups/ec860000-5dd4-0015-7da9-08dcda1e1e40");

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Detach_Account_InAccountgroup_WithAdmin_403()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.MegaAdmin);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.DeleteAsync($"{_baseUrlForV1}/ec860000-5dd4-0015-11c5-08dcda1de501/accountgroups/ec860000-5dd4-0015-7da9-08dcda1e1e40");

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Detach_Account_InAccountgroup_WithBadAccountAccountGroupId_404()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.RW);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.DeleteAsync($"{_baseUrlForV1}/{NewId.NextGuid()}/accountgroups/{NewId.NextGuid()}");
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
