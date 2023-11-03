using FluentAssertions;
using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Ubik.Accounting.Api.Features;
using Ubik.Accounting.Api.Features.Classifications.Queries;
using Ubik.Accounting.Api.Models;
using Ubik.Accounting.Contracts.AccountGroups.Queries;
using Ubik.Accounting.Contracts.AccountGroups.Results;
using Ubik.Accounting.Contracts.Classifications.Queries;
using Ubik.Accounting.Contracts.Classifications.Results;
using Ubik.ApiService.Common.Exceptions;

namespace Ubik.Accounting.Api.Tests.UnitTests.Features.Classifications.Queries
{
    public class ClassificationQueries_Test : IAsyncLifetime
    {
        private readonly IServiceManager _serviceManager;
        private readonly IEnumerable<Classification> _values;
        private ITestHarness _harness = default!;
        private IServiceProvider _provider = default!;

        public ClassificationQueries_Test()
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
                    x.AddConsumer<GetClassificationConsumer>();

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

        [Fact]
        public async Task Get_Classification_Ok()
        {
            //Arrange
            var fake = new Classification() { Code = "TEST", Label = "Test" };
            var result = new ResultT<Classification>() { IsSuccess = true, Result = fake };
            var query = new GetClassificationQuery()
            {
                Id = Guid.NewGuid()
            };
            _serviceManager.ClassificationService.GetAsync(query.Id).Returns(result);
            var client = _harness.GetRequestClient<GetClassificationQuery>();
            var consumerHarness = _harness.GetConsumerHarness<GetClassificationConsumer>();

            //Act
            var response = await client.GetResponse<GetClassificationResult>(query);

            //Assert
            var sent = await _harness.Sent.Any<GetClassificationResult>();
            var consumed = await _harness.Consumed.Any<GetClassificationQuery>();
            var consumerConsumed = await consumerHarness.Consumed.Any<GetClassificationQuery>();

            sent.Should().Be(true);
            consumed.Should().Be(true);
            consumerConsumed.Should().Be(true);
            response.Message.Should()
                .BeOfType<GetClassificationResult>()
                .And.Match<GetClassificationResult>(a => a.Code == fake.Code);
        }

        public async Task DisposeAsync()
        {
            await _harness.Stop();
        }
    }
}
