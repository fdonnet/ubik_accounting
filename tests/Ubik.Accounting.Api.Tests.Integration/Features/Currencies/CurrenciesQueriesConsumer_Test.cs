﻿using FluentAssertions;
using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using Ubik.Accounting.Api.Data.Init;
using Ubik.Accounting.Api.Tests.Integration.Fake;
using Ubik.Accounting.Contracts.Currencies.Queries;
using Ubik.Accounting.Contracts.Currencies.Results;
using Ubik.ApiService.Common.Filters;
using Ubik.ApiService.Common.Services;

namespace Ubik.Accounting.Api.Tests.Integration.Features.Currencies
{
    public class CurrenciesQueriesConsumer_Test : BaseIntegrationTest, IAsyncLifetime
    {
        private ITestHarness _harness = default!;
        private IServiceProvider _provider = default!;
        private readonly BaseValuesForCurrencies _testValuesForCurrencies;
        public CurrenciesQueriesConsumer_Test(IntegrationTestWebAppFactory factory) : base(factory)
        {
            _testValuesForCurrencies = new BaseValuesForCurrencies();
        }

        public async Task InitializeAsync()
        {
            _provider = new ServiceCollection()
                .AddMassTransitTestHarness(x =>
                {
                    x.AddScoped<ICurrentUserService>(us => new FakeUserService());
                    x.AddRequestClient<GetAllCurrenciesQuery>();
                    x.UsingRabbitMq((context, configurator) =>
                    {

                        configurator.Host(new Uri(Environment.GetEnvironmentVariable("MessageBroker__Host")!), h =>
                        {
                            h.Username("guest");
                            h.Password("guest");
                        });

                        configurator.ConfigureEndpoints(context);
                        configurator.UseSendFilter(typeof(TenantIdSendFilter<>), context);
                        configurator.UsePublishFilter(typeof(TenantIdPublishFilter<>), context);
                    });

                }).BuildServiceProvider(true);

            _harness = _provider.GetRequiredService<ITestHarness>();
            await _harness.Start();
        }

        [Fact]
        public async Task GetAll_Classifications_Ok()
        {
            //Arrange
            var client = _harness.GetRequestClient<GetAllCurrenciesQuery>();

            //Act
            var result = await client.GetResponse<GetAllCurrenciesResults>(new { });

            //Assert
            result.Message.Should()
                .BeAssignableTo<GetAllCurrenciesResults>()
                .And.Match<GetAllCurrenciesResults>(a => a.Currencies.First() is GetAllCurrenciesResult);
        }

        public async Task DisposeAsync()
        {
            await _harness.Stop();
        }
    }
}
