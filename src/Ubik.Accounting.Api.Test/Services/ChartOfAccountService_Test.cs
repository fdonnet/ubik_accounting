using LanguageExt.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ubik.Accounting.Api.Services;

namespace Ubik.Accounting.Api.Test.Services
{
    public class ChartOfAccountService_Test : IClassFixture<AccountingTestDbFixture>
    {
        public ChartOfAccountService_Test(AccountingTestDbFixture fixture)
                => Fixture = fixture;
        public AccountingTestDbFixture Fixture { get; }

        [Fact]
        public async Task GetAccounts_Test()
        {
            using var context = Fixture.CreateContext();
            var service = new ChartOfAccountsService(context);

            var accounts = await service.GetAccountsAsync();

            Assert.Collection(accounts, item => Assert.Contains("1020", item.Code));
        }
    }
}
