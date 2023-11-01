using FluentAssertions;
using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ubik.Accounting.Api.Features;
using Ubik.Accounting.Api.Features.AccountGroups.Queries;
using Ubik.Accounting.Api.Features.Classifications.Queries;
using Ubik.Accounting.Api.Models;
using Ubik.Accounting.Contracts.AccountGroups.Queries;
using Ubik.Accounting.Contracts.AccountGroups.Results;
using Ubik.Accounting.Contracts.Classifications.Queries;
using Ubik.Accounting.Contracts.Classifications.Results;

namespace Ubik.Accounting.Api.Tests.UnitTests.Features.Classifications.Queries
{
    public class GetAllClassifications_Test : IAsyncLifetime
    {
        private readonly IServiceManager _serviceManager;
        private readonly IEnumerable<Classification> _values;
        private ITestHarness _harness = default!;
        private IServiceProvider _provider = default!;

        public GetAllClassifications_Test()
        {
            _serviceManager = Substitute.For<IServiceManager>();

            _values = new Classification[] { new Classification() { Code = "TEST", Label = "Test" } };
            _serviceManager.ClassificationService.GetAllAsync().Returns(_values);
        }

        public async Task InitializeAsync()
        {

            _provider = new ServiceCollection()
                .AddMassTransitTestHarness(x =>
                {
                    x.AddScoped<IServiceManager>(sm => _serviceManager);
                    x.AddConsumer<GetAllClassificationsConsumer>();

                }).BuildServiceProvider(true);

            _harness = _provider.GetRequiredService<ITestHarness>();
            await _harness.Start();
        }

        [Fact]
        public async Task GetAll_Classifications_Ok()
        {
            //Arrange
            var client = _harness.GetRequestClient<GetAllClassificationsQuery>();
            var consumerHarness = _harness.GetConsumerHarness<GetAllClassificationsConsumer>();

            //Act
            var response = await client.GetResponse<GetAllClassificationsResults>(new { });

            //Assert
            var sent = await _harness.Sent.Any<GetAllClassificationsResults>();
            var consumed = await _harness.Consumed.Any<GetAllClassificationsQuery>();
            var consumerConsumed = await consumerHarness.Consumed.Any<GetAllClassificationsQuery>();

            sent.Should().Be(true);
            consumed.Should().Be(true);
            consumerConsumed.Should().Be(true);
            response.Message.Classifications.Should().AllBeOfType<GetAllClassificationsResult>();
        }
        public async Task DisposeAsync()
        {
            await _harness.Stop();
        }
    }
}
