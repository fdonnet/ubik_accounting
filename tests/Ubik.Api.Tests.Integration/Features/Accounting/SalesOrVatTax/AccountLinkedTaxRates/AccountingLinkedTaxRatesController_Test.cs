using FluentAssertions;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net;
using Ubik.Accounting.SalesOrVatTax.Contracts.AccountTaxRateConfigs.Results;
using Ubik.ApiService.Common.Exceptions;
using MassTransit;
using Ubik.Accounting.SalesOrVatTax.Contracts.AccountTaxRateConfigs.Commands;

namespace Ubik.Api.Tests.Integration.Features.Accounting.SalesOrVatTax.AccountLinkedTaxRates
{
    public class AccountingLinkedTaxRatesController_Test : BaseIntegrationTestAccountingSalesVatTax
    {

        private readonly string _baseUrlForV1;
        private readonly HttpClient _client;

        public AccountingLinkedTaxRatesController_Test(IntegrationTestProxyFactory factory) : base(factory)
        {
            _baseUrlForV1 = "/accounting/api/v1";
            _client = Factory.CreateDefaultClient();
        }

        [Fact]
        public async Task Get_All_AccountLinkedTaxRates_WithRW_OK()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.RW);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.GetAsync($"{_baseUrlForV1}/accounts/4c6f0000-5dd4-0015-7cad-08dcda56e423/taxrates");
            var result = await response.Content.ReadFromJsonAsync<List<AccountTaxRateConfigStandardResult>>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Should()
                .NotBeNull()
                .And.BeOfType<List<AccountTaxRateConfigStandardResult>>()
                .And.HaveCount(1);
        }

        [Fact]
        public async Task Get_All_AccountLinkedTaxRates_WithRO_OK()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.RO);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.GetAsync($"{_baseUrlForV1}/accounts/4c6f0000-5dd4-0015-7cad-08dcda56e423/taxrates");
            var result = await response.Content.ReadFromJsonAsync<List<AccountTaxRateConfigStandardResult>>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Should()
                .NotBeNull()
                .And.BeOfType<List<AccountTaxRateConfigStandardResult>>()
                .And.HaveCount(1);
        }

        [Fact]
        public async Task Get_All_AccountLinkedTaxRates_WithNoToken_Unauthorized()
        {
            //Act
            var response = await _client.GetAsync($"{_baseUrlForV1}/accounts/4c6f0000-5dd4-0015-7cad-08dcda56e423/taxrates");

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Get_All_AccountLinkedTaxRates_WithAdmin_403()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.MegaAdmin);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.GetAsync($"{_baseUrlForV1}/accounts/4c6f0000-5dd4-0015-7cad-08dcda56e423/taxrates");

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Get_All_AccountLinkedTaxRates_WithOtherTenant_404()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.OtherTenant);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.GetAsync($"{_baseUrlForV1}/accounts/4c6f0000-5dd4-0015-7cad-08dcda56e423/taxrates");
            var result = await response.Content.ReadFromJsonAsync<CustomProblemDetails>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            result.Should()
                .NotBeNull()
                .And.BeOfType<CustomProblemDetails> ()
                .And.Match<CustomProblemDetails>(x => x.Errors.First().Code == "ACCOUNT_NOT_FOUND");
        }

        [Fact]
        public async Task Get_All_AccountLinkedTaxRates_WithNoRole_403()
        {
            var token = await GetAccessTokenAsync(TokenType.NoRole);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.GetAsync($"{_baseUrlForV1}/accounts/4c6f0000-5dd4-0015-7cad-08dcda56e423/taxrates");

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Get_All_AccountLinkedTaxRates_WithBadId_404()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.RW);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.GetAsync($"{_baseUrlForV1}/accounts/{NewId.NextGuid()}/taxrates");
            var result = await response.Content.ReadFromJsonAsync<CustomProblemDetails>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            result.Should()
                .NotBeNull()
                .And.BeOfType<CustomProblemDetails>()
                .And.Match<CustomProblemDetails>(x => x.Errors.First().Code == "ACCOUNT_NOT_FOUND");
        }

        [Fact]
        public async Task Add_AccountLinkedTaxRates_WithRW_OK()
        {
            var token = await GetAccessTokenAsync(TokenType.RW);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var command = new AddAccountTaxRateConfigCommand()
            {
                AccountId = new Guid("60520000-5dd4-0015-5c21-08dcda58ab64"),
                TaxRateId = new Guid("08740000-3c36-7456-6f96-08dcfb48b915"),
                TaxAccountId = new Guid("248e0000-5dd4-0015-443b-08dcd98b545d")
            };

            //Act
            var response = await _client.PostAsJsonAsync($"{_baseUrlForV1}/accounts/{command.AccountId}/taxrates/{command.TaxRateId}", command);
            var result = await response.Content.ReadFromJsonAsync<AccountTaxRateConfigStandardResult>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Should()
            .NotBeNull()
                .And.BeOfType<AccountTaxRateConfigStandardResult>()
                .And.Match<AccountTaxRateConfigStandardResult>(x => x.AccountId == command.AccountId);
        }

        [Fact]
        public async Task Add_AccountLinkedTaxRates_WithRO_403()
        {
            var token = await GetAccessTokenAsync(TokenType.RO);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var command = new AddAccountTaxRateConfigCommand()
            {
                AccountId = new Guid("60520000-5dd4-0015-5c21-08dcda58ab64"),
                TaxRateId = new Guid("08740000-3c36-7456-6f96-08dcfb48b915"),
                TaxAccountId = new Guid("248e0000-5dd4-0015-443b-08dcd98b545d")
            };

            //Act
            var response = await _client.PostAsJsonAsync($"{_baseUrlForV1}/accounts/{command.AccountId}/taxrates/{command.TaxRateId}", command);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Add_AccountLinkedTaxRates_WithNoToken_401()
        {
            var command = new AddAccountTaxRateConfigCommand()
            {
                AccountId = new Guid("60520000-5dd4-0015-5c21-08dcda58ab64"),
                TaxRateId = new Guid("08740000-3c36-7456-6f96-08dcfb48b915"),
                TaxAccountId = new Guid("248e0000-5dd4-0015-443b-08dcd98b545d")
            };

            //Act
            var response = await _client.PostAsJsonAsync($"{_baseUrlForV1}/accounts/{command.AccountId}/taxrates/{command.TaxRateId}", command);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Add_AccountLinkedTaxRates_WithAdmin_403()
        {
            var token = await GetAccessTokenAsync(TokenType.MegaAdmin);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var command = new AddAccountTaxRateConfigCommand()
            {
                AccountId = new Guid("60520000-5dd4-0015-5c21-08dcda58ab64"),
                TaxRateId = new Guid("08740000-3c36-7456-6f96-08dcfb48b915"),
                TaxAccountId = new Guid("248e0000-5dd4-0015-443b-08dcd98b545d")
            };

            //Act
            var response = await _client.PostAsJsonAsync($"{_baseUrlForV1}/accounts/{command.AccountId}/taxrates/{command.TaxRateId}", command);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Add_AccountLinkedTaxRates_WithOtherTenant_404()
        {
            var token = await GetAccessTokenAsync(TokenType.OtherTenant);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var command = new AddAccountTaxRateConfigCommand()
            {
                AccountId = new Guid("60520000-5dd4-0015-5c21-08dcda58ab64"),
                TaxRateId = new Guid("08740000-3c36-7456-6f96-08dcfb48b915"),
                TaxAccountId = new Guid("248e0000-5dd4-0015-443b-08dcd98b545d")
            };

            //Act
            var response = await _client.PostAsJsonAsync($"{_baseUrlForV1}/accounts/{command.AccountId}/taxrates/{command.TaxRateId}", command);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Add_AccountLinkedTaxRates_WithNoRole_403()
        {
            var token = await GetAccessTokenAsync(TokenType.NoRole);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var command = new AddAccountTaxRateConfigCommand()
            {
                AccountId = new Guid("60520000-5dd4-0015-5c21-08dcda58ab64"),
                TaxRateId = new Guid("08740000-3c36-7456-6f96-08dcfb48b915"),
                TaxAccountId = new Guid("248e0000-5dd4-0015-443b-08dcd98b545d")
            };

            //Act
            var response = await _client.PostAsJsonAsync($"{_baseUrlForV1}/accounts/{command.AccountId}/taxrates/{command.TaxRateId}", command);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Add_AccountLinkedTaxRates_WithNotMatchIdAccount_400()
        {
            var token = await GetAccessTokenAsync(TokenType.RW);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var command = new AddAccountTaxRateConfigCommand()
            {
                AccountId = new Guid("78920000-5dd4-0015-6f96-08dcd9a78046"),
                TaxRateId = new Guid("08740000-3c36-7456-6f96-08dcfb48b915"),
                TaxAccountId = new Guid("248e0000-5dd4-0015-443b-08dcd98b545d")
            };

            //Act
            var response = await _client.PostAsJsonAsync($"{_baseUrlForV1}/accounts/248e0000-5dd4-0015-443b-08dcd98b545d/taxrates/{command.TaxRateId}", command);
            var result = await response.Content.ReadFromJsonAsync<CustomProblemDetails>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            result.Should()
                .NotBeNull()
                .And.BeOfType<CustomProblemDetails>()
                .And.Match<CustomProblemDetails>(x => x.Errors.First().Code == "ACCOUNT_COMMAND_IDS_NOT_MATCH");
        }

        [Fact]
        public async Task Add_AccountLinkedTaxRates_WithNotMatchIdTaxRate_400()
        {
            var token = await GetAccessTokenAsync(TokenType.RW);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var command = new AddAccountTaxRateConfigCommand()
            {
                AccountId = new Guid("248e0000-5dd4-0015-443b-08dcd98b545d"),
                TaxRateId = new Guid("08740000-3c36-7456-6f96-08dcfb48b915"),
                TaxAccountId = new Guid("248e0000-5dd4-0015-443b-08dcd98b545d")
            };

            //Act
            var response = await _client.PostAsJsonAsync($"{_baseUrlForV1}/accounts/{command.AccountId}/taxrates/08740000-3c36-7456-dc84-08dcfb48d62d", command);
            var result = await response.Content.ReadFromJsonAsync<CustomProblemDetails>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            result.Should()
                .NotBeNull()
                .And.BeOfType<CustomProblemDetails>()
                .And.Match<CustomProblemDetails>(x => x.Errors.First().Code == "TAXRATE_COMMAND_IDS_NOT_MATCH");
        }

        [Fact]
        public async Task Add_AccountLinkedTaxRates_WithBadAccountId_404()
        {
            var token = await GetAccessTokenAsync(TokenType.RW);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var command = new AddAccountTaxRateConfigCommand()
            {
                AccountId = new Guid("60520000-5dd4-0015-5c21-08dcda58ab61"),
                TaxRateId = new Guid("08740000-3c36-7456-6f96-08dcfb48b915"),
                TaxAccountId = new Guid("248e0000-5dd4-0015-443b-08dcd98b545d")
            };

            //Act
            var response = await _client.PostAsJsonAsync($"{_baseUrlForV1}/accounts/{command.AccountId}/taxrates/{command.TaxRateId}", command);
            var result = await response.Content.ReadFromJsonAsync<CustomProblemDetails>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            result.Should()
            .NotBeNull()
                .And.BeOfType<CustomProblemDetails>()
                .And.Match<CustomProblemDetails>(x => x.Errors.First().Code == "ACCOUNT_NOT_FOUND");
        }

        [Fact]
        public async Task Add_AccountLinkedTaxRates_WithBadTaxRateId_404()
        {
            var token = await GetAccessTokenAsync(TokenType.RW);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var command = new AddAccountTaxRateConfigCommand()
            {
                AccountId = new Guid("ec860000-5dd4-0015-d472-08dcda1dc37c"),
                TaxRateId = new Guid("08740000-3c36-7456-6f96-08dcfb48b914"),
                TaxAccountId = new Guid("248e0000-5dd4-0015-443b-08dcd98b545d")
            };

            //Act
            var response = await _client.PostAsJsonAsync($"{_baseUrlForV1}/accounts/{command.AccountId}/taxrates/{command.TaxRateId}", command);
            var result = await response.Content.ReadFromJsonAsync<CustomProblemDetails>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            result.Should()
            .NotBeNull()
                .And.BeOfType<CustomProblemDetails>()
                .And.Match<CustomProblemDetails>(x => x.Errors.First().Code == "TAXRATE_NOT_FOUND");
        }

        [Fact]
        public async Task Add_AccountLinkedTaxRates_WithBadTaxAccountId_404()
        {
            var token = await GetAccessTokenAsync(TokenType.RW);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var command = new AddAccountTaxRateConfigCommand()
            {
                AccountId = new Guid("60520000-5dd4-0015-5c21-08dcda58ab64"),
                TaxRateId = new Guid("08740000-3c36-7456-6f96-08dcfb48b915"),
                TaxAccountId = new Guid("248e0000-5dd4-0015-443b-08dcd98b5451")
            };

            //Act
            var response = await _client.PostAsJsonAsync($"{_baseUrlForV1}/accounts/{command.AccountId}/taxrates/{command.TaxRateId}", command);
            var result = await response.Content.ReadFromJsonAsync<CustomProblemDetails>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            result.Should()
            .NotBeNull()
                .And.BeOfType<CustomProblemDetails>()
                .And.Match<CustomProblemDetails>(x => x.Errors.First().Code == "ACCOUNTTAXCONFIG_TAXACCOUNT_NOT_FOUND");
        }

        [Fact]
        public async Task Add_AccountLinkedTaxRates_WithAlreadyExisting_400()
        {
            var token = await GetAccessTokenAsync(TokenType.RW);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var command = new AddAccountTaxRateConfigCommand()
            {
                AccountId = new Guid("4c6f0000-5dd4-0015-7cad-08dcda56e423"),
                TaxRateId = new Guid("08740000-3c36-7456-6f96-08dcfb48b915"),
                TaxAccountId = new Guid("248e0000-5dd4-0015-443b-08dcd98b545d")
            };

            //Act
            var response = await _client.PostAsJsonAsync($"{_baseUrlForV1}/accounts/{command.AccountId}/taxrates/{command.TaxRateId}", command);
            var result = await response.Content.ReadFromJsonAsync<CustomProblemDetails>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            result.Should()
            .NotBeNull()
                .And.BeOfType<CustomProblemDetails>()
                .And.Match<CustomProblemDetails>(x => x.Errors.First().Code == "LINKED_TAX_RATE_ALREADY_EXIST");
        }

        [Fact]
        public async Task Delete_AccountLinkedTaxRates_WithRW_OK()
        {
            var token = await GetAccessTokenAsync(TokenType.RW);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.DeleteAsync($"{_baseUrlForV1}/accounts/4c6f0000-5dd4-0015-db2b-08dcda56e424/taxrates/08740000-3c36-7456-dc84-08dcfb48d62d");
            
            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task Delete_AccountLinkedTaxRates_WithRO_403()
        {
            var token = await GetAccessTokenAsync(TokenType.RO);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.DeleteAsync($"{_baseUrlForV1}/accounts/4c6f0000-5dd4-0015-db2b-08dcda56e424/taxrates/08740000-3c36-7456-dc84-08dcfb48d62d");

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Delete_AccountLinkedTaxRates_WithNoToken_401()
        {
            //Act
            var response = await _client.DeleteAsync($"{_baseUrlForV1}/accounts/4c6f0000-5dd4-0015-db2b-08dcda56e424/taxrates/08740000-3c36-7456-dc84-08dcfb48d62d");

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Delete_AccountLinkedTaxRates_WithAdmin_403()
        {
            var token = await GetAccessTokenAsync(TokenType.MegaAdmin);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.DeleteAsync($"{_baseUrlForV1}/accounts/4c6f0000-5dd4-0015-db2b-08dcda56e424/taxrates/08740000-3c36-7456-dc84-08dcfb48d62d");

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Delete_AccountLinkedTaxRates_WithOtherTenant_404()
        {
            var token = await GetAccessTokenAsync(TokenType.OtherTenant);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.DeleteAsync($"{_baseUrlForV1}/accounts/4c6f0000-5dd4-0015-db2b-08dcda56e424/taxrates/08740000-3c36-7456-dc84-08dcfb48d62d");

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Delete_AccountLinkedTaxRates_WithNoRole_403()
        {
            var token = await GetAccessTokenAsync(TokenType.NoRole);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.DeleteAsync($"{_baseUrlForV1}/accounts/4c6f0000-5dd4-0015-db2b-08dcda56e424/taxrates/08740000-3c36-7456-dc84-08dcfb48d62d");

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Delete_AccountLinkedTaxRates_WithBadId_404()
        {
            var token = await GetAccessTokenAsync(TokenType.RW);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.DeleteAsync($"{_baseUrlForV1}/accounts/4c6f0000-5dd4-0015-db2b-08dcda56e421/taxrates/08740000-3c36-7456-dc84-08dcfb48d62d");
            var result = await response.Content.ReadFromJsonAsync<CustomProblemDetails>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            result.Should()
            .NotBeNull()
                .And.BeOfType<CustomProblemDetails>()
                .And.Match<CustomProblemDetails>(x => x.Errors.First().Code == "ACCOUNTTAXRATECONFIG_NOT_FOUND");
        }
    }
}
