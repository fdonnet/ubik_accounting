using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Ubik.Accounting.Structure.Contracts.Classifications.Results;
using Ubik.Accounting.SalesOrVatTax.Contracts.VatRate.Results;

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
        public async Task Get_TaxRates_All_WithRW_OK()
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

    }
}
