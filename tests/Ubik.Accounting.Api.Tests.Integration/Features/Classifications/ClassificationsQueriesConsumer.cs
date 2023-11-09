using FluentAssertions;
using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using Ubik.Accounting.Api.Data.Init;
using Ubik.Accounting.Api.Tests.Integration.Fake;
using Ubik.Accounting.Contracts.Classifications.Queries;
using Ubik.Accounting.Contracts.Classifications.Results;
using Ubik.ApiService.Common.Exceptions;
using Ubik.ApiService.Common.Filters;
using Ubik.ApiService.Common.Services;

namespace Ubik.Accounting.Api.Tests.Integration.Features.Classifications
{
    public class ClassificationsQueriesConsumer : BaseIntegrationTest, IAsyncLifetime
    {
        private ITestHarness _harness = default!;
        private IServiceProvider _provider = default!;
        private readonly BaseValuesForClassifications _testValuesForClassifications;

        public ClassificationsQueriesConsumer(IntegrationTestWebAppFactory factory) : base(factory)
        {
            _testValuesForClassifications = new BaseValuesForClassifications();
        }

        public async Task InitializeAsync()
        {
            _provider = new ServiceCollection()
                .AddMassTransitTestHarness(x =>
                {
                    x.AddScoped<ICurrentUserService>(us => new FakeUserService());
                    x.AddRequestClient<GetAllClassificationsQuery>();
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
            var client = _harness.GetRequestClient<GetAllClassificationsQuery>();

            //Act
            var result = await client.GetResponse<GetAllClassificationsResults>(new { });

            //Assert
            result.Message.Should()
                .BeAssignableTo<GetAllClassificationsResults>()
                .And.Match<GetAllClassificationsResults>(a => a.Classifications.First() is GetAllClassificationsResult);
        }

        [Fact]
        public async Task Get_Classification_Ok()
        {
            //Arrange
            var client = _harness.GetRequestClient<GetClassificationQuery>();

            //Act
            var result = await client.GetResponse<GetClassificationResult>(new GetClassificationQuery { Id = _testValuesForClassifications.ClassificationId1 });

            //Assert
            result.Message.Should()
                .BeAssignableTo<GetClassificationResult>()
                .And.Match<GetClassificationResult>(a => a.Id == _testValuesForClassifications.ClassificationId1);
        }

        [Fact]
        public async Task Get_IServiceAndFeatureException_ClassificatioIdNotFound()
        {
            //Arrange
            var client = _harness.GetRequestClient<GetClassificationQuery>();

            //Act
            var (result, error) = await client.GetResponse<GetClassificationResult, IServiceAndFeatureException>(new GetClassificationQuery { Id = Guid.NewGuid() });

            var prob = await error;

            //Assert
            prob.Message.Should()
                .BeAssignableTo<IServiceAndFeatureException>()
                .And.Match<IServiceAndFeatureException>(a =>
                    a.ErrorType == ServiceAndFeatureExceptionType.NotFound
                    && a.CustomErrors[0].ErrorCode == "CLASSIFICATION_NOT_FOUND");
        }

        public async Task DisposeAsync()
        {
            await _harness.Stop();
        }
    }
}
