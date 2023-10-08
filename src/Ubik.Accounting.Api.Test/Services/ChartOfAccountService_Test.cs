using FluentAssertions;
using LanguageExt.Common;
using LanguageExt.UnitTesting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ubik.Accounting.Api.Dto;
using Ubik.Accounting.Api.Models;
using Ubik.Accounting.Api.Services;

namespace Ubik.Accounting.Api.Test.Services
{
    public class ChartOfAccountService_Test : IClassFixture<AccountingTestDbFixture>
    {
        public ChartOfAccountService_Test(AccountingTestDbFixture fixture)
                => Fixture = fixture;
        public AccountingTestDbFixture Fixture { get; }

        [Fact]
        public async Task GetAccountsAsync_Success()
        {
            //Arrange
            using var context = AccountingTestDbFixture.CreateContext();
            var service = new ChartOfAccountsService(context);

            //Act
            var accounts = await service.GetAccountsAsync();

            //Assert
            accounts.Should()
                    .NotBeNull()
                    .And.Contain(x=>x.Code =="1020");
        }

        [Fact]
        public async Task AddAccountAsync_Success()
        {
            //Arrange
            using var context = AccountingTestDbFixture.CreateContext();
            var service = new ChartOfAccountsService(context);

            //Act
            var accountDto = new AccountDtoForAdd()
            {
                Code = "1030",
                Label = "Compte de liquidité test",
                Description = "Test account",
                AccountGroupId = Guid.Parse("1524f11f-20dd-4888-88f8-428e59bbc22a")
            };

            var account = await service.AddAccountAsync(accountDto);

            //Assert
            account.Should()
                .BeOfType<Result<Account>>()
                .And.Match<Result<Account>>(a=>a.IsSuccess ==true);
        }
    }
}
