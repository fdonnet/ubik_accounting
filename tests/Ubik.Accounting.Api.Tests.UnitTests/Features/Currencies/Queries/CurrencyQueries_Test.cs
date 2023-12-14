using FluentAssertions;
using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Ubik.Accounting.Api.Features;
using Ubik.Accounting.Api.Features.Currencies.Queries;
using Ubik.Accounting.Api.Models;
using Ubik.Accounting.Contracts.Currencies.Queries;
using Ubik.Accounting.Contracts.Currencies.Results;

namespace Ubik.Accounting.Api.Tests.UnitTests.Features.Currencies.Queries
{
    public class CurrencyQueries_Test : IAsyncLifetime
    {
        private readonly IServiceManager _serviceManager;
        private readonly IEnumerable<Currency> _values;
        private ITestHarness _harness = default!;
        private IServiceProvider _provider = default!;

        public CurrencyQueries_Test()
        {
            _serviceManager = Substitute.For<IServiceManager>();

            _values = new Currency[] { new() { IsoCode = "USD"} };
            _serviceManager.CurrencyService.GetAllAsync().Returns(_values);
        }

        public async Task InitializeAsync()
        {

            _provider = new ServiceCollection()
                .AddMassTransitTestHarness(x =>
                {
                    x.AddScoped<IServiceManager>(sm => _serviceManager);
                    x.AddConsumer<GetAllCurrenciesConsumer>();

                }).BuildServiceProvider(true);

            _harness = _provider.GetRequiredService<ITestHarness>();
            await _harness.Start();
        }

        [Fact]
        public async Task GetAll_Currencies_Ok()
        {
            //Arrange
            var client = _harness.GetRequestClient<GetAllCurrenciesQuery>();
            var consumerHarness = _harness.GetConsumerHarness<GetAllCurrenciesConsumer>();

            //Act
            var response = await client.GetResponse<GetAllCurrenciesResults>(new { });

            //Assert
            var sent = await _harness.Sent.Any<GetAllCurrenciesResults>();
            var consumed = await _harness.Consumed.Any<GetAllCurrenciesQuery>();
            var consumerConsumed = await consumerHarness.Consumed.Any<GetAllCurrenciesQuery>();

            sent.Should().Be(true);
            consumed.Should().Be(true);
            consumerConsumed.Should().Be(true);
            response.Message.Currencies.Should().AllBeOfType<GetAllCurrenciesResult>();
        }
        public async Task DisposeAsync()
        {
            await _harness.Stop();
        }

    }
}
