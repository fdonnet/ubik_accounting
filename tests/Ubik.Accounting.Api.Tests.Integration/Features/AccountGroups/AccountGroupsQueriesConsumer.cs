using FluentAssertions;
using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using Ubik.Accounting.Api.Data.Init;
using Ubik.Accounting.Api.Tests.Integration.Fake;
using Ubik.Accounting.Contracts.AccountGroups.Queries;
using Ubik.Accounting.Contracts.AccountGroups.Results;
using Ubik.ApiService.Common.Errors;
using Ubik.ApiService.Common.Filters;
using Ubik.ApiService.Common.Services;

namespace Ubik.Accounting.Api.Tests.Integration.Features.AccountGroups
{
    public  class AccountGroupsQueriesConsumer : BaseIntegrationTest, IAsyncLifetime
    {
        private ITestHarness _harness = default!;
        private IServiceProvider _provider = default!;
        private readonly BaseValuesForAccountGroups _testValuesForAccountGroups;

        public AccountGroupsQueriesConsumer(IntegrationTestWebAppFactory factory) : base(factory)
        {
            _testValuesForAccountGroups = new BaseValuesForAccountGroups();
        }

        public async Task InitializeAsync()
        {
            _provider = new ServiceCollection()
                .AddMassTransitTestHarness(x =>
                {
                    x.AddScoped<ICurrentUserService>(us => new FakeUserService());
                    x.AddRequestClient<GetAllAccountGroupsQuery>();
                    x.AddRequestClient<GetAccountGroupQuery>();
                    x.AddRequestClient<GetChildAccountsQuery>();
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
        public async Task GetAll_AccountGroups_Ok()
        {
            //Arrange
            var client = _harness.GetRequestClient<GetAllAccountGroupsQuery>();

            //Act
            var result = await client.GetResponse<GetAllAccountGroupsResults>(new { });

            //Assert
            result.Message.Should()
                .BeAssignableTo<GetAllAccountGroupsResults>()
                .And.Match<GetAllAccountGroupsResults>(a => a.AccountGroups[0] is GetAllAccountGroupsResult);
        }

        [Fact]
        public async Task GetChildAccounts_Accounts_Ok()
        {
            //Arrange
            var client = _harness.GetRequestClient<GetChildAccountsQuery>();

            //Act
            var result = await client.GetResponse<GetChildAccountsResults>(new GetChildAccountsQuery 
            { AccountGroupId = _testValuesForAccountGroups.AccountGroupId2 });

            //Assert
            result.Message.Should()
                .BeAssignableTo<GetChildAccountsResults>()
                .And.Match<GetChildAccountsResults>(a => a.ChildAccounts[0] is GetChildAccountsResult);
        }

        [Fact]
        public async Task Get_AccountGroup_Ok()
        {
            //Arrange
            var client = _harness.GetRequestClient<GetAccountGroupQuery>();

            //Act
            var result = await client.GetResponse<GetAccountGroupResult>(new GetAccountGroupQuery { Id = _testValuesForAccountGroups.AccountGroupId1 });

            //Assert
            result.Message.Should()
                .BeAssignableTo<GetAccountGroupResult>()
                .And.Match<GetAccountGroupResult>(a => a.Id == _testValuesForAccountGroups.AccountGroupId1);
        }

        [Fact]
        public async Task Get_IServiceAndFeatureException_AccountGroupIdNotFound()
        {
            //Arrange
            var client = _harness.GetRequestClient<GetAccountGroupQuery>();

            //Act
            var (result, error) = await client.GetResponse<GetAccountGroupResult, IServiceAndFeatureError>(new GetAccountGroupQuery { Id = Guid.NewGuid() });

            var prob = await error;

            //Assert
            prob.Message.Should()
                .BeAssignableTo<IServiceAndFeatureError>()
                .And.Match<IServiceAndFeatureError>(a =>
                    a.ErrorType == ServiceAndFeatureErrorType.NotFound
                    && a.CustomErrors[0].ErrorCode == "ACCOUNTGROUP_NOT_FOUND");
        }

        public async Task DisposeAsync()
        {
            await _harness.Stop();
        }
    }
}
