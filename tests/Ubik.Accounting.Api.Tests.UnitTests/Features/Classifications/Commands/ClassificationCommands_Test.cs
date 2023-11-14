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
using Ubik.Accounting.Api.Features.AccountGroups.Commands;
using Ubik.Accounting.Api.Features.Classifications.Commands;
using Ubik.Accounting.Api.Models;
using Ubik.Accounting.Contracts.AccountGroups.Commands;
using Ubik.Accounting.Contracts.AccountGroups.Events;
using Ubik.Accounting.Contracts.AccountGroups.Results;
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
            var response = await client.GetResponse<AddClassificationResult>(new AddClassificationCommand { Code="TEST",Label="TEST"});

            //Assert
            var sent = await _harness.Sent.Any<AddClassificationResult>();
            var consumed = await _harness.Consumed.Any<AddClassificationCommand>();
            var consumerConsumed = await consumerHarness.Consumed.Any<AddClassificationCommand>();

            sent.Should().Be(true);
            consumed.Should().Be(true);
            consumerConsumed.Should().Be(true);
            response.Message.Should()
                .BeOfType<AddClassificationResult>()
                .And.Match<AddClassificationResult>(a => a.Code == "TEST");
        }
        [Fact]
        public async Task Add_Classification_OkClassificationAddedPublished()
        {
            //Arrange
            _serviceManager.ClassificationService.AddAsync(Arg.Any<Classification>()).Returns(new Classification { Code="TEST",Label="Test"});
            var client = _harness.GetRequestClient<AddClassificationCommand>();

            //Act
            await client.GetResponse<AddClassificationResult>(new AddClassificationCommand { Code = "TEST", Label = "TEST" });

            //Assert
            var sent = await _harness.Published.Any<ClassificationAdded>();

            sent.Should().Be(true);
        }

        public async Task DisposeAsync()
        {
            await _harness.Stop();
        }

    }
}
