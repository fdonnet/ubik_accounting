using FluentAssertions;
using Ubik.Accounting.Api.Data.Init;
using Ubik.Accounting.Api.Features;
using Ubik.Accounting.Api.Models;
using Ubik.Api.Tests.Integration.Fake;

namespace Ubik.Api.Tests.Integration.Features.Accounting.Currencies
{
    public class CurrenciesService_Test : BaseIntegrationTest
    {
        private readonly BaseValuesForCurrencies _testCurrencies;
        private readonly IServiceManager _serviceManager;

        public CurrenciesService_Test(IntegrationTestWebAppFactory factory) : base(factory)
        {
            _testCurrencies = new BaseValuesForCurrencies();
            _serviceManager = new ServiceManager(DbContext, new FakeUserService());
        }

        [Fact]
        public async Task GetAll_Currencies_Ok()
        {
            //Arrange

            //Act
            var result = await _serviceManager.CurrencyService.GetAllAsync();

            //Assert
            result.Should()
                    .NotBeNull()
                    .And.AllBeOfType<Currency>();
        }
    }
}
