using FluentAssertions;
using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Ubik.Accounting.Api.Features;
using Ubik.Accounting.Api.Features.Classifications.Commands;
using Ubik.Accounting.Api.Models;
using Ubik.Accounting.Contracts.Classifications.Commands;
using Ubik.Accounting.Contracts.Classifications.Events;
using Ubik.Accounting.Contracts.Classifications.Results;

namespace Ubik.Accounting.Api.Tests.UnitTests.Features.Classifications.Commands
{
    public class ClassificationCommands_Test : IAsyncLifetime
    {
        private readonly IServiceManager _serviceManager;
        private ITestHarness _harness = default!;
        private IServiceProvider _provider = default!;


        public ClassificationCommands_Test()
        {
            _serviceManager = Substitute.For<IServiceManager>();
        }

        public async Task InitializeAsync()
        {
            _provider = new ServiceCollection()
                .AddMassTransitTestHarness(x =>
                {
                    x.AddScoped<IServiceManager>(sm => _serviceManager);
                    x.AddConsumer<AddClassificationConsumer>();
                    x.AddConsumer<UpdateClassificationConsumer>();
                    x.AddConsumer<DeleteClassificationConsumer>();

                }).BuildServiceProvider(true);

            _harness = _provider.GetRequiredService<ITestHarness>();
            await _harness.Start();
        }


        [Fact]
        public async Task Add_Classification_Ok()
        {
            //Arrange
            _serviceManager.ClassificationService.AddAsync(Arg.Any<Classification>()).Returns(new Classification { Code = "TEST", Label = "Test" });
            var client = _harness.GetRequestClient<AddClassificationCommand>();
            var consumerHarness = _harness.GetConsumerHarness<AddClassificationConsumer>();

            //Act
            var response = await client.GetResponse<ClassificationStandardResult>(new AddClassificationCommand { Code="TEST",Label="TEST"});

            //Assert
            var sent = await _harness.Sent.Any<ClassificationStandardResult>();
            var consumed = await _harness.Consumed.Any<AddClassificationCommand>();
            var consumerConsumed = await consumerHarness.Consumed.Any<AddClassificationCommand>();

            sent.Should().Be(true);
            consumed.Should().Be(true);
            consumerConsumed.Should().Be(true);
            response.Message.Should()
                .BeOfType<ClassificationStandardResult>()
                .And.Match<ClassificationStandardResult>(a => a.Code == "TEST");
        }
        [Fact]
        public async Task Add_Classification_OkClassificationAddedPublished()
        {
            //Arrange
            _serviceManager.ClassificationService.AddAsync(Arg.Any<Classification>()).Returns(new Classification { Code="TEST",Label="Test"});
            var client = _harness.GetRequestClient<AddClassificationCommand>();

            //Act
            await client.GetResponse<ClassificationStandardResult>(new AddClassificationCommand { Code = "TEST", Label = "TEST" });

            //Assert
            var sent = await _harness.Published.Any<ClassificationAdded>();

            sent.Should().Be(true);
        }

        [Fact]
        public async Task Update_Classification_Ok()
        {
            //Arrange
            _serviceManager.ClassificationService.UpdateAsync(Arg.Any<Classification>()).Returns(new Classification { Code = "TEST", Label = "Test" });
            var client = _harness.GetRequestClient<UpdateClassificationCommand>();
            var consumerHarness = _harness.GetConsumerHarness<UpdateClassificationConsumer>();

            //Act
            var response = await client.GetResponse<UpdateClassificationResult>(new UpdateClassificationCommand { Code = "TEST", Label = "TEST" });

            //Assert
            var sent = await _harness.Sent.Any<UpdateClassificationResult>();
            var consumed = await _harness.Consumed.Any<UpdateClassificationCommand>();
            var consumerConsumed = await consumerHarness.Consumed.Any<UpdateClassificationCommand>();

            sent.Should().Be(true);
            consumed.Should().Be(true);
            consumerConsumed.Should().Be(true);
            response.Message.Should()
                .BeOfType<UpdateClassificationResult>()
                .And.Match<UpdateClassificationResult>(a => a.Code == "TEST");
        }
        [Fact]
        public async Task Update_Classification_OkClassificationUpdatedPublished()
        {
            //Arrange
            _serviceManager.ClassificationService.UpdateAsync(Arg.Any<Classification>()).Returns(new Classification { Code = "TEST", Label = "Test" });
            var client = _harness.GetRequestClient<UpdateClassificationCommand>();

            //Act
            await client.GetResponse<UpdateClassificationResult>(new UpdateClassificationCommand { Code = "TEST", Label = "TEST" });

            //Assert
            var sent = await _harness.Published.Any<ClassificationUpdated>();

            sent.Should().Be(true);
        }

        [Fact]
        public async Task Delete_Classification_Ok()
        {
            //Arrange
            _serviceManager.ClassificationService.DeleteAsync(Arg.Any<Guid>()).Returns(new List<AccountGroup>());
            var client = _harness.GetRequestClient<DeleteClassificationCommand>();
            var consumerHarness = _harness.GetConsumerHarness<DeleteClassificationConsumer>();

            //Act
            var testId = Guid.NewGuid();
            var response = await client.GetResponse<ClassificationDeleteResult>(new DeleteClassificationCommand { Id= testId });

            //Assert
            var sent = await _harness.Sent.Any<ClassificationDeleteResult>();
            var consumed = await _harness.Consumed.Any<DeleteClassificationCommand>();
            var consumerConsumed = await consumerHarness.Consumed.Any<DeleteClassificationCommand>();

            sent.Should().Be(true);
            consumed.Should().Be(true);
            consumerConsumed.Should().Be(true);
            response.Message.Should()
                .BeOfType<ClassificationDeleteResult>()
                .And.Match<ClassificationDeleteResult>(a => a.Id == testId);
        }
        [Fact]
        public async Task Delete_Classification_OkClassificationDeletedPublished()
        {
            //Arrange
            _serviceManager.ClassificationService.DeleteAsync(Arg.Any<Guid>()).Returns(new List<AccountGroup>());
            var client = _harness.GetRequestClient<DeleteClassificationCommand>();

            //Act
            await client.GetResponse<ClassificationDeleteResult>(new DeleteClassificationCommand { Id=Guid.NewGuid() });

            //Assert
            var sent = await _harness.Published.Any<ClassificationDeleted>();

            sent.Should().Be(true);
        }

        public async Task DisposeAsync()
        {
            await _harness.Stop();
        }

    }
}
