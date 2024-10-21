using FluentAssertions;
using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Ubik.Accounting.Api.Features;
using Ubik.Accounting.Api.Features.Classifications.Queries;
using Ubik.Accounting.Api.Features.Classifications.Queries.CustomPoco;
using Ubik.Accounting.Api.Models;
using Ubik.Accounting.Contracts.Classifications.Queries;
using Ubik.Accounting.Contracts.Classifications.Results;

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

            _values = new Classification[] { new() { Code = "TEST", Label = "Test" } };
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
                    x.AddConsumer<GetClassificationAccountsConsumer>();
                    x.AddConsumer<GetClassificationAccountsMissingConsumer>();
                    x.AddConsumer<GetClassificationStatusConsumer>();

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
            var query = new GetClassificationQuery()
            {
                Id = Guid.NewGuid()
            };
            _serviceManager.ClassificationService.GetAsync(query.Id).Returns(fake);
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

        [Fact]
        public async Task GetAccounts_Accounts_Ok()
        {
            //Arrange
            var fake = new Account() { Code = "TEST", Label = "Test", CurrencyId=NewId.NextGuid() };
            var query = new GetClassificationAccountsQuery()
            {
                ClassificationId = Guid.NewGuid()
            };
            _serviceManager.ClassificationService.GetClassificationAccountsAsync(query.ClassificationId).Returns(new[] { fake });
            var client = _harness.GetRequestClient<GetClassificationAccountsQuery>();
            var consumerHarness = _harness.GetConsumerHarness<GetClassificationAccountsConsumer>();

            //Act
            var response = await client.GetResponse<GetClassificationAccountsResults>(query);

            //Assert
            var sent = await _harness.Sent.Any<GetClassificationAccountsResults>();
            var consumed = await _harness.Consumed.Any<GetClassificationAccountsQuery>();
            var consumerConsumed = await consumerHarness.Consumed.Any<GetClassificationAccountsQuery>();

            sent.Should().Be(true);
            consumed.Should().Be(true);
            consumerConsumed.Should().Be(true);
            response.Message.Should()
                .BeOfType<GetClassificationAccountsResults>()
                .And.Match<GetClassificationAccountsResults>(a => a.Accounts.Any());
        }
        [Fact]
        public async Task GetMissingAccounts_Accounts_Ok()
        {
            //Arrange
            var fake = new Account() { Code = "TEST", Label = "Test", CurrencyId = NewId.NextGuid() };
            var query = new GetClassificationAccountsQuery()
            {
                ClassificationId = Guid.NewGuid()
            };
            _serviceManager.ClassificationService.GetClassificationAccountsMissingAsync(query.ClassificationId).Returns(new[] { fake });
            var client = _harness.GetRequestClient<GetClassificationAccountsMissingQuery>();
            var consumerHarness = _harness.GetConsumerHarness<GetClassificationAccountsMissingConsumer>();

            //Act
            var response = await client.GetResponse<GetClassificationAccountsMissingResults>(query);

            //Assert
            var sent = await _harness.Sent.Any<GetClassificationAccountsMissingResults>();
            var consumed = await _harness.Consumed.Any<GetClassificationAccountsMissingQuery>();
            var consumerConsumed = await consumerHarness.Consumed.Any<GetClassificationAccountsMissingQuery>();

            sent.Should().Be(true);
            consumed.Should().Be(true);
            consumerConsumed.Should().Be(true);
            response.Message.Should()
                .BeOfType<GetClassificationAccountsMissingResults>()
                .And.Match<GetClassificationAccountsMissingResults>(a => a.Accounts.Any());
        }

        [Fact]
        public async Task GetStatus_ClassificationStatus_Ok()
        {
            //Arrange
            var fake = new ClassificationStatus() { Id = NewId.NextGuid(), IsReady = true, MissingAccounts = new Account[]
            { new() { Code = "TEST", Label = "TEST", CurrencyId = NewId.NextGuid() } } };

            var query = new GetClassificationStatusQuery()
            {
                Id = Guid.NewGuid()
            };
            _serviceManager.ClassificationService.GetClassificationStatusAsync(query.Id).Returns(fake);
            var client = _harness.GetRequestClient<GetClassificationStatusQuery>();
            var consumerHarness = _harness.GetConsumerHarness<GetClassificationStatusConsumer>();

            //Act
            var response = await client.GetResponse<ClassificationStatusResult>(query);

            //Assert
            var sent = await _harness.Sent.Any<ClassificationStatusResult>();
            var consumed = await _harness.Consumed.Any<GetClassificationStatusQuery>();
            var consumerConsumed = await consumerHarness.Consumed.Any<GetClassificationStatusQuery>();

            sent.Should().Be(true);
            consumed.Should().Be(true);
            consumerConsumed.Should().Be(true);
            response.Message.Should()
                .BeOfType<ClassificationStatusResult>()
                .And.Match<ClassificationStatusResult>(a => a.IsReady == fake.IsReady);
        }

        public async Task DisposeAsync()
        {
            await _harness.Stop();
        }
    }
}
