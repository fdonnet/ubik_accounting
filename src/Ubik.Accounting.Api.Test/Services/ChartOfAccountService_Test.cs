using FluentAssertions;
using LanguageExt.Common;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using Ubik.Accounting.Api.Data;
using Ubik.Accounting.Api.Dto;
using Ubik.Accounting.Api.Models;
using Ubik.Accounting.Api.Services;
using Ubik.ApiService.Common.Exceptions;

namespace Ubik.Accounting.Api.Test.Services
{
    public class ChartOfAccountService_Test : IClassFixture<AccountingTestDbFixture>
    {
        public AccountingTestDbFixture Fixture { get; }

        private readonly DbInitializer _testDBValues;

        public ChartOfAccountService_Test(AccountingTestDbFixture fixture)
        {
            Fixture = fixture;
            _testDBValues = new DbInitializer();
        }

        [Fact]
        public async Task GetAccountAsync_Success()
        {
            //Arrange
            var context = AccountingTestDbFixture.CreateContext();
            var service = new ChartOfAccountsService(context);

            //Act
            var account = await service.GetAccountAsync(_testDBValues.AccountId1);

            //Assert
            account.Should()
                    .NotBeNull()
                    .And.BeOfType<Result<AccountDto>>()
                    .And.Match<Result<AccountDto>>(a => a.IsSuccess == true);
        }

        [Fact]
        public async Task GetAccountAsync_Failed_NotFound()
        {
            //Arrange
            var context = AccountingTestDbFixture.CreateContext();
            var service = new ChartOfAccountsService(context);

            //Act
            var account = await service.GetAccountAsync(_testDBValues.UserId1);
            var ok = account.Match(
                Succ: a => new Exception(),
                Fail: a => a
            );

            //Assert
            ok.Should()
                .BeOfType<ServiceException>()
                .And.Match<ServiceException>(a =>
                                    a.ErrorCode == "ACCOUNT_NOT_FOUND" &&
                                    a.ExceptionType == ServiceExceptionType.NotFound);
        }

        [Fact]
        public async Task GetAccountsAsync_Success()
        {
            //Arrange
            var context = AccountingTestDbFixture.CreateContext();
            var service = new ChartOfAccountsService(context);

            //Act
            var accounts = await service.GetAccountsAsync();

            //Assert
            accounts.Should()
                    .NotBeNull()
                    .And.Contain(x => x.Code == "1020");
        }

        [Fact]
        public async Task AddAccountAsync_Success()
        {
            //Arrange
            var context = AccountingTestDbFixture.CreateContext();
            var service = new ChartOfAccountsService(context);

            //Act
            var accountDto = new AccountDtoForAdd()
            {
                Code = "1030",
                Label = "Compte de liquidité test",
                Description = "Test account",
                AccountGroupId = _testDBValues.AccountGroupId1
            };

            var account = await service.AddAccountAsync(accountDto);

            //Assert
            account.Should()
                .BeOfType<Result<Account>>()
                .And.Match<Result<Account>>(a => a.IsSuccess == true);
        }

        [Fact]
        public async Task AddAccountAsync_Success_CheckFields()
        {
            //Arrange
            var context = AccountingTestDbFixture.CreateContext();
            var service = new ChartOfAccountsService(context);

            //Act
            var accountDto = new AccountDtoForAdd()
            {
                Code = "1031",
                Label = "Compte de liquidité test",
                Description = "Test account1",
                AccountGroupId = _testDBValues.AccountGroupId1
            };

            var account = await service.AddAccountAsync(accountDto);

            Account ok = account.Match(
                Succ: a => a,
                Fail: a => new Account() { Code = "-1", Label = "Fail" }
            );

            //Assert
            ok.Should()
                .BeOfType<Account>()
                .And.Match<Account>(a =>
                                    a.Code == "1031" &&
                                    a.Label == "Compte de liquidité test" &&
                                    a.Description == "Test account1" &&
                                    a.AccountGroupId == _testDBValues.AccountGroupId1);
        }

        /// <summary>
        /// Test audit fields (only on this account entity) to be sure the SaveAsync take care of IAuditEntity interface
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task AddAccountAsync_Success_CheckAuditFields()
        {
            //Arrange
            var context = AccountingTestDbFixture.CreateContext();
            var service = new ChartOfAccountsService(context);

            //Act
            var accountDto = new AccountDtoForAdd()
            {
                Code = "1032",
                Label = "Compte de liquidité test2",
                Description = "Test account2",
                AccountGroupId = _testDBValues.AccountGroupId1
            };

            var account = await service.AddAccountAsync(accountDto);

            Account ok = account.Match(
                Succ: a => a,
                Fail: a => new Account() { Code = "-1", Label = "Fail" }
            );

            //Assert
            ok.Should()
                .BeOfType<Account>()
                .And.Match<Account>(a =>
                                    a.CreatedAt > DateTime.UtcNow.AddMinutes(-1) &&
                                    a.ModifiedAt > DateTime.UtcNow.AddMinutes(-1) &&
                                    a.CreatedBy == _testDBValues.UserId1 &&
                                    a.ModifiedBy == _testDBValues.UserId1);
        }

        /// <summary>
        /// Test tenant id field (only on this account entity) to be sure the SaveAsync take care of ITenantEntity interface
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task AddAccountAsync_Success_CheckTenantId()
        {
            //Arrange
            var context = AccountingTestDbFixture.CreateContext();
            var service = new ChartOfAccountsService(context);

            //Act
            var accountDto = new AccountDtoForAdd()
            {
                Code = "1033",
                Label = "Compte de liquidité test2",
                Description = "Test account3",
                AccountGroupId = _testDBValues.AccountGroupId1
            };

            var account = await service.AddAccountAsync(accountDto);

            Account ok = account.Match(
                Succ: a => a,
                Fail: a => new Account() { Code = "-1", Label = "Fail" }
            );

            //Assert
            ok.Should()
                .BeOfType<Account>()
                .And.Match<Account>(a =>
                                    a.TenantId == _testDBValues.TenantId);
        }

        [Fact]
        public async Task AddAccountAsync_Failed()
        {
            //Arrange
            var context = AccountingTestDbFixture.CreateContext();
            var service = new ChartOfAccountsService(context);

            //Act
            var accountDto = new AccountDtoForAdd()
            {
                Code = "1020",
                Label = "Compte de liquidité test4",
                Description = "Test account4",
                AccountGroupId = _testDBValues.AccountGroupId1
            };

            var account = await service.AddAccountAsync(accountDto);

            //Assert
            account.Should()
                .BeOfType<Result<Account>>()
                .And.Match<Result<Account>>(a => a.IsSuccess == false);
        }

        [Fact]
        public async Task AddAccountAsync_Failed_AlreadyExists()
        {
            //Arrange
            var context = AccountingTestDbFixture.CreateContext();
            var service = new ChartOfAccountsService(context);

            //Act
            var accountDto = new AccountDtoForAdd()
            {
                Code = "1020",
                Label = "Compte de liquidité test",
                Description = "Test account5",
                AccountGroupId = _testDBValues.AccountGroupId1
            };

            var account = await service.AddAccountAsync(accountDto);

            var ok = account.Match(
                Succ: a => new Exception(),
                Fail: a => a
            );

            //Assert
            ok.Should()
                .BeOfType<ServiceException>()
                .And.Match<ServiceException>(a =>
                                    a.ErrorCode == "ACCOUNT_ALREADY_EXISTS" &&
                                    a.ExceptionType == ServiceExceptionType.Conflict);
        }

        [Fact]
        public async Task UpdateAccountAsync_Success()
        {
            //Arrange
            var context = AccountingTestDbFixture.CreateContext();
            var service = new ChartOfAccountsService(context);

            //Act
            //Need to get the account from DB to have the correct version field (concurrency)
            var accountToUpdate = await service.GetAccountAsync(_testDBValues.AccountId1);
            var ok1 = accountToUpdate.Match<AccountDto>(Succ: a => a, Fail: a => new AccountDto() { Code = "-1", Label = "-1" });
            ok1.Label = "TEST UPDATE";

            var result = await service.UpdateAccountAsync(_testDBValues.AccountId1, ok1);
            var ok2 = result.Match<bool>(Succ: a => a, Fail: a => false);

            //Assert
            ok1.Should().Match<AccountDto>(a => a.Code != "-1");
            ok2.Should().BeTrue();
        }

    }
}
