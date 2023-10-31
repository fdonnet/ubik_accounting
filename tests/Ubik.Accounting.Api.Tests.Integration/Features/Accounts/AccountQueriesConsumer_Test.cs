using Ubik.Accounting.Api.Data.Init;
using Ubik.Accounting.Contracts.Accounts.Results;
using MassTransit;
using Ubik.Accounting.Contracts.Accounts.Queries;
using Microsoft.Extensions.DependencyInjection;
using FluentAssertions;
using Ubik.Accounting.Contracts.Accounts.Commands;
using System.Diagnostics;
using MassTransit.Testing;
using Ubik.Accounting.Api.Features.Accounts.Commands;
using Ubik.Accounting.Api.Features;
using Ubik.ApiService.Common.Filters;
using Ubik.ApiService.Common.Services;
using Ubik.Accounting.Api.Tests.Integration.Fake;
using Ubik.ApiService.Common.Exceptions;
using LanguageExt.Pipes;

namespace Ubik.Accounting.Api.Tests.Integration.Features.Accounts
{
    public class AccountQueriesConsumer_Test : BaseIntegrationTest, IAsyncLifetime
    {
        private ITestHarness _harness = default!;
        private IServiceProvider _provider = default!;
        private readonly BaseValuesForAccounts _testValuesForAccounts;

        public AccountQueriesConsumer_Test(IntegrationTestWebAppFactory factory) : base(factory)
        {
            _testValuesForAccounts = new BaseValuesForAccounts();
        }

        public async Task InitializeAsync()
        {
            _provider = new ServiceCollection()
                .AddMassTransitTestHarness(x =>
                {
                    x.AddScoped<ICurrentUserService>(us => new FakeUserService());
                    x.AddRequestClient<GetAllAccountsQuery>();
                    x.AddRequestClient<GetAccountQuery>();
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
        public async Task GetAll_Accounts_Ok()
        {
            //Arrange
            var client = _harness.GetRequestClient<GetAllAccountsQuery>();

            //Act
            var result= await client.GetResponse<IGetAllAccountsResult>(new { });

            //Assert
            result.Message.Should().BeAssignableTo<IGetAllAccountsResult>();
            result.Message.Should().Match<IGetAllAccountsResult>(a => a.Accounts[0] is GetAllAccountsResult);
        }


        [Fact]
        public async Task Get_Account_Ok()
        {
            //Arrange
            var client = _harness.GetRequestClient<GetAccountQuery>();

            //Act
            var result = await client.GetResponse<GetAccountResult>(new GetAccountQuery { Id=_testValuesForAccounts.AccountId1});

            //Assert
            result.Message.Should().BeAssignableTo<GetAccountResult>();
            result.Message.Should().Match<GetAccountResult>(a => a.Id == _testValuesForAccounts.AccountId1);
        }

        [Fact]
        public async Task Get_IServiceAndFeatureException_AccountIdNotFound()
        {
            //Arrange
            var client = _harness.GetRequestClient<GetAccountQuery>();

            //Act
            var (result, error) = await client.GetResponse<GetAccountResult, IServiceAndFeatureException>(new GetAccountQuery { Id = Guid.NewGuid() });

            var prob = await error;

            //Assert
            prob.Message.Should().BeAssignableTo<IServiceAndFeatureException>();
            prob.Message.Should().Match<IServiceAndFeatureException>(a =>
                a.ErrorType == ServiceAndFeatureExceptionType.NotFound
                && a.CustomErrors[0].ErrorCode == "ACCOUNT_NOT_FOUND");
        }
           

        public async Task DisposeAsync()
        {
            await _harness.Stop();
        }

    }
}
