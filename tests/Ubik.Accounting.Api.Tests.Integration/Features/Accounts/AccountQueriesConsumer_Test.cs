using Ubik.Accounting.Api.Data.Init;
using Ubik.Accounting.Contracts.Accounts.Results;
using MassTransit;
using Ubik.Accounting.Contracts.Accounts.Queries;
using Microsoft.Extensions.DependencyInjection;
using FluentAssertions;
using Ubik.Accounting.Contracts.Accounts.Commands;
using System.Diagnostics;

namespace Ubik.Accounting.Api.Tests.Integration.Features.Accounts
{
    public class AccountQueriesConsumer_Test : BaseIntegrationTest
    {
        private IRequestClient<GetAllAccountsQuery> _client = default!;
        public AccountQueriesConsumer_Test(IntegrationTestWebAppFactory factory
            , IRequestClient<GetAllAccountsQuery> client) : base(factory)
        {
            _client = client;
        }

        [Fact]
        public async Task GetAll_Accounts_Ok()
        {
            //Arrange

            //Act
            var result= await _client.GetResponse<IGetAllAccountsResult>(new { });

            //Assert
            result.Message.Should().BeAssignableTo<IGetAllAccountsResult>();
            result.Message.Should().Match<IGetAllAccountsResult>(a => a.Accounts[0] is GetAllAccountsResult);
        }

    }
}
