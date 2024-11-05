using FluentAssertions;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net;
using Ubik.Accounting.SalesOrVatTax.Contracts.SalesOrVatTaxRate.Results;
using Ubik.ApiService.Common.Exceptions;
using MassTransit;
using Ubik.Accounting.SalesOrVatTax.Contracts.SalesOrVatTaxRate.Commands;

namespace Ubik.Api.Tests.Integration.Features.Accounting.SalesOrVatTax.TaxRates
{
    public class TaxRatesController_Test : BaseIntegrationTestAccountingSalesVatTax
    {

        private readonly string _baseUrlForV1;
        private readonly HttpClient _client;
        private readonly static Guid _id = new("08740000-3c36-7456-6f96-08dcfb48b915");
        private readonly static Guid _idToDel = new("e4220000-3c36-7456-212b-08dcfb4a3d2b");
        private readonly static Guid _idToUpd = new("e4220000-3c36-7456-d3e1-08dcfb4a4b65");

        public TaxRatesController_Test(IntegrationTestProxyFactory factory) : base(factory)
        {
            _baseUrlForV1 = "/accounting/api/v1/sales-vat-tax/taxrates";
            _client = Factory.CreateDefaultClient();
        }

        [Fact]
        public async Task Get_All_TaxRates_WithRW_OK()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.RW);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.GetAsync(_baseUrlForV1);
            var result = await response.Content.ReadFromJsonAsync<List<SalesOrVatTaxRateStandardResult>>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Should()
                .NotBeNull()
                .And.BeOfType<List<SalesOrVatTaxRateStandardResult>>();
        }

        [Fact]
        public async Task Get_All_TaxRates_WithRO_OK()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.RO);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.GetAsync(_baseUrlForV1);
            var result = await response.Content.ReadFromJsonAsync<List<SalesOrVatTaxRateStandardResult>>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Should()
                .NotBeNull()
                .And.BeOfType<List<SalesOrVatTaxRateStandardResult>>();
        }

        [Fact]
        public async Task Get_All_TaxRates_WithNoToken_401()
        {
            //Act
            var response = await _client.GetAsync(_baseUrlForV1);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Get_All_TaxRates_WithNoRole_401()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.NoRole);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.GetAsync(_baseUrlForV1);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Get_All_TaxRates_WithAdmin_403()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.MegaAdmin);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.GetAsync(_baseUrlForV1);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Get_All_TaxRates_WithOtherTenant_OK()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.OtherTenant);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.GetAsync(_baseUrlForV1);
            var result = await response.Content.ReadFromJsonAsync<List<SalesOrVatTaxRateStandardResult>>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Should()
                .NotBeNull()
                .And.BeOfType<List<SalesOrVatTaxRateStandardResult>>();
        }

        [Fact]
        public async Task Get_TaxRate_By_Id_WithRW_OK()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.RW);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.GetAsync($"{_baseUrlForV1}/{_id}");
            var result = await response.Content.ReadFromJsonAsync<SalesOrVatTaxRateStandardResult>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Should()
                .NotBeNull()
                .And.BeOfType<SalesOrVatTaxRateStandardResult>();
        }

        [Fact]
        public async Task Get_TaxRate_By_Id_WithRO_OK()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.RO);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.GetAsync($"{_baseUrlForV1}/{_id}");
            var result = await response.Content.ReadFromJsonAsync<SalesOrVatTaxRateStandardResult>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Should()
                .NotBeNull()
                .And.BeOfType<SalesOrVatTaxRateStandardResult>();
        }

        [Fact]
        public async Task Get_TaxRate_By_Id_WithNoToken_401()
        {
            //Act
            var response = await _client.GetAsync($"{_baseUrlForV1}/{_id}");

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Get_TaxRate_By_Id_WithNoRole_401()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.NoRole);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.GetAsync($"{_baseUrlForV1}/{_id}");

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Get_TaxRate_By_Id_WithAdmin_403()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.MegaAdmin);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.GetAsync($"{_baseUrlForV1}/{_id}");

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Get_TaxRate_By_Id_WithOtherTenant_OK()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.OtherTenant);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.GetAsync($"{_baseUrlForV1}/{_id}");
            var result = await response.Content.ReadFromJsonAsync<CustomProblemDetails>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            result.Should()
                .NotBeNull()
                .And.BeOfType<CustomProblemDetails>()
                .And.Match<CustomProblemDetails>(x => x.Errors.First().Code == "TAXRATE_NOT_FOUND");
        }

        [Fact]
        public async Task Get_TaxRate_By_Id_WithBadId_404()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.OtherTenant);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.GetAsync($"{_baseUrlForV1}/{NewId.NextGuid()}");
            var result = await response.Content.ReadFromJsonAsync<CustomProblemDetails>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            result.Should()
                .NotBeNull()
                .And.BeOfType<CustomProblemDetails>()
                .And.Match<CustomProblemDetails>(x => x.Errors.First().Code == "TAXRATE_NOT_FOUND");
        }

        [Fact]
        public async Task Add_TaxRate_WithRW_OK()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.RW);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var command = new AddSalesOrVatTaxRateCommand
            {
                Code = "Test",
                Description = "Description",
                Rate = 7.4m,
                ValidFrom = DateOnly.FromDateTime(DateTime.Now),
                ValidTo = null,
            };

            //Act
            var response = await _client.PostAsJsonAsync(_baseUrlForV1, command);
            var tmp = await response.Content.ReadAsStringAsync();
            var result = await response.Content.ReadFromJsonAsync<SalesOrVatTaxRateStandardResult>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            result.Should()
            .NotBeNull()
                .And.BeOfType<SalesOrVatTaxRateStandardResult>()
                .And.Match<SalesOrVatTaxRateStandardResult>(x => x.Code == command.Code);
        }

        [Fact]
        public async Task Add_TaxRate_WithAlreadyExists_409()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.RW);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var command = new AddSalesOrVatTaxRateCommand
            {
                Code = "v81",
                Description = "Description",
                Rate = 7.4m,
                ValidFrom = DateOnly.FromDateTime(DateTime.Now),
                ValidTo = null,
            };

            //Act
            var response = await _client.PostAsJsonAsync(_baseUrlForV1, command);
            var tmp = await response.Content.ReadAsStringAsync();
            var result = await response.Content.ReadFromJsonAsync<CustomProblemDetails>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Conflict);
            result.Should()
            .NotBeNull()
                .And.BeOfType<CustomProblemDetails>()
                .And.Match<CustomProblemDetails>(x => x.Errors.First().Code == "TAXRATE_ALREADY_EXISTS");
        }

        [Fact]
        public async Task Add_TaxRate_WithRO_403()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.RO);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var command = new AddSalesOrVatTaxRateCommand
            {
                Code = "Test",
                Description = "Description",
                Rate = 7.4m,
                ValidFrom = DateOnly.FromDateTime(DateTime.Now),
                ValidTo = null
            };

            //Act
            var response = await _client.PostAsJsonAsync(_baseUrlForV1, command);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Add_TaxRate_WithNoToken_401()
        {
            //Arrange
            var command = new AddSalesOrVatTaxRateCommand
            {
                Code = "Test",
                Description = "Description",
                Rate = 7.4m,
                ValidFrom = DateOnly.FromDateTime(DateTime.Now),
                ValidTo = null
            };

            //Act
            var response = await _client.PostAsJsonAsync(_baseUrlForV1, command);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Add_TaxRate_WithAdmin_403()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.MegaAdmin);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var command = new AddSalesOrVatTaxRateCommand
            {
                Code = "Test",
                Description = "Description",
                Rate = 7.4m,
                ValidFrom = DateOnly.FromDateTime(DateTime.Now),
                ValidTo = null
            };

            //Act
            var response = await _client.PostAsJsonAsync(_baseUrlForV1, command);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Add_TaxRate_WithOtherTenant_OK()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.OtherTenant);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var command = new AddSalesOrVatTaxRateCommand
            {
                Code = "Test",
                Description = "Description",
                Rate = 7.4m,
                ValidFrom = DateOnly.FromDateTime(DateTime.Now),
                ValidTo = null
            };

            //Act
            var response = await _client.PostAsJsonAsync(_baseUrlForV1, command);
            var result = await response.Content.ReadFromJsonAsync<SalesOrVatTaxRateStandardResult>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            result.Should()
                .NotBeNull()
                .And.BeOfType<SalesOrVatTaxRateStandardResult>()
                .And.Match<SalesOrVatTaxRateStandardResult>(x => x.Code == command.Code);
        }

        [Fact]
        public async Task Add_TaxRate_WithNoRole_403()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.NoRole);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var command = new AddSalesOrVatTaxRateCommand
            {
                Code = "Test",
                Description = "Description",
                Rate = 7.4m,
                ValidFrom = DateOnly.FromDateTime(DateTime.Now),
                ValidTo = null
            };

            //Act
            var response = await _client.PostAsJsonAsync(_baseUrlForV1, command);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Update_TaxRate_WithRW_OK()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.RW);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var command = new UpdateSalesOrVatTaxRateCommand
            {
                Id = _idToUpd,
                Code = "Test2",
                Description = "Description",
                Rate = 7.4m,
                ValidFrom = DateOnly.FromDateTime(DateTime.Now),
                ValidTo = null,
                Version = new Guid("e4220000-3c36-7456-088c-08dcfb4a4b66")
            };

            //Act
            var response = await _client.PutAsJsonAsync($"{_baseUrlForV1}/{_idToUpd}", command);
            var result = await response.Content.ReadFromJsonAsync<SalesOrVatTaxRateStandardResult>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Should()
                .NotBeNull()
                .And.BeOfType<SalesOrVatTaxRateStandardResult>()
                .And.Match<SalesOrVatTaxRateStandardResult>(x => x.Code == command.Code);
        }

        [Fact]
        public async Task Update_TaxRate_WithRO_403()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.RO);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var command = new UpdateSalesOrVatTaxRateCommand
            {
                Id = _idToUpd,
                Code = "Test2",
                Description = "Description",
                Rate = 7.4m,
                ValidFrom = DateOnly.FromDateTime(DateTime.Now),
                ValidTo = null,
                Version = new Guid("e4220000-3c36-7456-088c-08dcfb4a4b66")
            };

            //Act
            var response = await _client.PutAsJsonAsync($"{_baseUrlForV1}/{_idToUpd}", command);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Update_TaxRate_WithNoToken_401()
        {
            //Arrange
            var command = new UpdateSalesOrVatTaxRateCommand
            {
                Id = _idToUpd,
                Code = "Test2",
                Description = "Description",
                Rate = 7.4m,
                ValidFrom = DateOnly.FromDateTime(DateTime.Now),
                ValidTo = null,
                Version = new Guid("e4220000-3c36-7456-088c-08dcfb4a4b66")
            };

            //Act
            var response = await _client.PutAsJsonAsync($"{_baseUrlForV1}/{_idToUpd}", command);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Update_TaxRate_WithAdmin_403()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.MegaAdmin);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var command = new UpdateSalesOrVatTaxRateCommand
            {
                Id = _idToUpd,
                Code = "Test2",
                Description = "Description",
                Rate = 7.4m,
                ValidFrom = DateOnly.FromDateTime(DateTime.Now),
                ValidTo = null,
                Version = new Guid("e4220000-3c36-7456-088c-08dcfb4a4b66")
            };

            //Act
            var response = await _client.PutAsJsonAsync($"{_baseUrlForV1}/{_idToUpd}", command);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Update_TaxRate_WithOtherTenant_404()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.OtherTenant);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var command = new UpdateSalesOrVatTaxRateCommand
            {
                Id = _idToUpd,
                Code = "Test2",
                Description = "Description",
                Rate = 7.4m,
                ValidFrom = DateOnly.FromDateTime(DateTime.Now),
                ValidTo = null,
                Version = new Guid("e4220000-3c36-7456-088c-08dcfb4a4b66")
            };

            //Act
            var response = await _client.PutAsJsonAsync($"{_baseUrlForV1}/{_idToUpd}", command);
            var result = await response.Content.ReadFromJsonAsync<CustomProblemDetails>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            result.Should()
                .NotBeNull()
                .And.BeOfType<CustomProblemDetails>()
                .And.Match<CustomProblemDetails>(x => x.Errors.First().Code == "TAXRATE_NOT_FOUND");
        }

        [Fact]
        public async Task Update_TaxRate_WithNoRole_403()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.NoRole);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var command = new UpdateSalesOrVatTaxRateCommand
            {
                Id = _idToUpd,
                Code = "Test2",
                Description = "Description",
                Rate = 7.4m,
                ValidFrom = DateOnly.FromDateTime(DateTime.Now),
                ValidTo = null,
                Version = new Guid("e4220000-3c36-7456-088c-08dcfb4a4b66")
            };

            //Act
            var response = await _client.PutAsJsonAsync($"{_baseUrlForV1}/{_idToUpd}", command);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }



        [Fact]
        public async Task Update_TaxRate_WithNotMatchId_400()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.RW);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var command = new UpdateSalesOrVatTaxRateCommand
            {
                Id = NewId.NextGuid(),
                Code = "Test3",
                Description = "Description",
                Rate = 7.4m,
                ValidFrom = DateOnly.FromDateTime(DateTime.Now),
                ValidTo = null,
                Version = new Guid("e4220000-3c36-7456-088c-08dcfb4a4b66")
            };

            //Act
            var response = await _client.PutAsJsonAsync($"{_baseUrlForV1}/{_idToUpd}", command);
            var result = await response.Content.ReadFromJsonAsync<CustomProblemDetails>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            result.Should()
                .NotBeNull()
                .And.BeOfType<CustomProblemDetails>()
                .And.Match<CustomProblemDetails>(x => x.Errors.First().Code == "TAXRATE_COMMAND_IDS_NOT_MATCH");
        }

        [Fact]
        public async Task Update_TaxRate_WithAlreadyExists_409()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.RW);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var command = new UpdateSalesOrVatTaxRateCommand
            {
                Id = _idToUpd,
                Code = "v81",
                Description = "Description",
                Rate = 7.4m,
                ValidFrom = DateOnly.FromDateTime(DateTime.Now),
                ValidTo = null,
                Version = new Guid("e4220000-3c36-7456-088c-08dcfb4a4b66")
            };

            //Act
            var response = await _client.PutAsJsonAsync($"{_baseUrlForV1}/{_idToUpd}", command);
            var result = await response.Content.ReadFromJsonAsync<CustomProblemDetails>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Conflict);
            result.Should()
                .NotBeNull()
                .And.BeOfType<CustomProblemDetails>()
                .And.Match<CustomProblemDetails>(x => x.Errors.First().Code == "TAXRATE_ALREADY_EXISTS");
        }

        [Fact]
        public async Task Update_TaxRate_WithBadId_404()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.RW);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var id = NewId.NextGuid();
            var command = new UpdateSalesOrVatTaxRateCommand
            {
                Id = id,
                Code = "v81",
                Description = "Description",
                Rate = 7.4m,
                ValidFrom = DateOnly.FromDateTime(DateTime.Now),
                ValidTo = null,
                Version = new Guid("e4220000-3c36-7456-088c-08dcfb4a4b66")
            };

            //Act
            var response = await _client.PutAsJsonAsync($"{_baseUrlForV1}/{id}", command);
            var result = await response.Content.ReadFromJsonAsync<CustomProblemDetails>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            result.Should()
                .NotBeNull()
                .And.BeOfType<CustomProblemDetails>()
                .And.Match<CustomProblemDetails>(x => x.Errors.First().Code == "TAXRATE_NOT_FOUND");
        }

        [Fact]
        public async Task Delete_TaxRate_WithRW_OK()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.RW);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.DeleteAsync($"{_baseUrlForV1}/{_idToDel}");

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task Delete_TaxRate_WithRO_403()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.RO);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.DeleteAsync($"{_baseUrlForV1}/{_idToDel}");

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Delete_TaxRate_WithNoToken_401()
        {
            //Act
            var response = await _client.DeleteAsync($"{_baseUrlForV1}/{_idToDel}");

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Delete_TaxRate_WithAdmin_403()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.MegaAdmin);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.DeleteAsync($"{_baseUrlForV1}/{_idToDel}");

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Delete_TaxRate_WithOtherTenant_404()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.OtherTenant);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.DeleteAsync($"{_baseUrlForV1}/{_idToDel}");

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Delete_TaxRate_WithNoRole_403()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.NoRole);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.DeleteAsync($"{_baseUrlForV1}/{_idToDel}");

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Delete_TaxRate_WithBadId_404()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.RW);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.DeleteAsync($"{_baseUrlForV1}/{NewId.NextGuid()}");

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}
