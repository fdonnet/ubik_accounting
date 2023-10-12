using Bogus;
using Bogus.Extensions;
using FluentAssertions;
using MediatR.NotificationPublishers;
using Ubik.Accounting.Api.Data;
using Ubik.Accounting.Api.Features;
using Ubik.Accounting.Api.Features.Accounts.Exceptions;
using Ubik.Accounting.Api.Models;
using Ubik.ApiService.Common.Exceptions;

namespace Ubik.Accounting.Api.Tests.Integration.Features.Accounts
{
    public class AccountService_Test : IClassFixture<AccountingTestDbFixture>
    {
        public AccountingTestDbFixture Fixture { get; }
        private readonly IServiceManager _serviceManager;
        private readonly DbInitializer _testDBValues;

        public AccountService_Test(AccountingTestDbFixture fixture)
        {
            Fixture = fixture;
            _testDBValues = new DbInitializer();
            _serviceManager = new ServiceManager(AccountingTestDbFixture.CreateContext());
        }

        [Fact]
        public async Task Get_Success_Account()
        {
            //Arrange

            //Act
            var account = await _serviceManager.AccountService.GetAccountAsync(_testDBValues.AccountId1);

            //Assert
            account.Should()
                    .NotBeNull()
                    .And.BeOfType<Account>();
        }

        [Theory, MemberData(nameof(GeneratedGuids))]
        public async Task Get_IdNotExists_Null(Guid id)
        {
            //Arrange

            //Act
            var account = await _serviceManager.AccountService.GetAccountAsync(id);

            //Assert
            account.Should()
                    .BeNull();
        }

        [Fact]
        public async Task IfExist_Success_True()
        {
            //Arrange

            //Act
            var account = await _serviceManager.AccountService.IfExists(_testDBValues.AccountCode1);

            //Assert
            account.Should().BeTrue();
        }

        [Fact]
        public async Task IfExist_Success_False()
        {
            //Arrange

            //Act
            var account = await _serviceManager.AccountService.IfExists("ZZZZZZZXX");

            //Assert
            account.Should().BeFalse();
        }

        [Fact]
        public async Task GetAll_Success_Accounts()
        {
            //Arrange

            //Act
            var accounts = await _serviceManager.AccountService.GetAccountsAsync();

            //Assert
            accounts.Should()
                    .NotBeNull()
                    .And.AllBeOfType<Account>();
        }

        [Theory]
        [MemberData(nameof(GetAccounts), parameters: new object[] { 5, "1524f11f-20dd-4888-88f8-428e59bbc22a"})]
        public async Task Add_Success_AddedAccount(Account account)
        {
            //Arrange

            //Act
            var accountResult = await _serviceManager.AccountService.AddAccountAsync(account);

            //Assert
            account.Should()
                    .NotBeNull()
                    .And.BeOfType<Account>();
        }

        [Theory]
        [MemberData(nameof(GetAccounts), parameters: new object[] { 5, "1524f11f-20dd-4888-88f8-428e59bbc22a" })]
        public async Task Add_Success_AuditModifiedFieldsDefined(Account account)
        {
            //Arrange

            //Act
            var accountResult = await _serviceManager.AccountService.AddAccountAsync(account);

            //Assert
            account.Should().Match<Account>(x => x.ModifiedBy != null && x.ModifiedAt != null);
        }

        [Theory]
        [MemberData(nameof(GetAccounts), parameters: new object[] { 5, "1524f11f-20dd-4888-88f8-428e59bbc22b" })]
        public async Task Add_AccountGroupIdNotExists_Exception(Account account)
        {
            //Arrange

            //Act
            Func<Task> act = async () => await _serviceManager.AccountService.AddAccountAsync(account);

            //Assert
            await act.Should().ThrowAsync<Exception>();
        }

        [Theory]
        [MemberData(nameof(GetAccounts), parameters: new object[] { 5, "1524f11f-20dd-4888-88f8-428e59bbc22b" })]
        public async Task Add_CrashDb_Exception(Account account)
        {
            //Arrange
            account.Code = new string(new Faker("fr_CH").Random.Chars(count: 21));

            //Act
            Func<Task> act = async () => await _serviceManager.AccountService.AddAccountAsync(account);

            //Assert
            await act.Should().ThrowAsync<Exception>();
        }

        [Fact]
        public async Task Update_Success_UpdatedAccount()
        {
            //Arrange
            var account = await _serviceManager.AccountService.GetAccountAsync(_testDBValues.AccountId1);

            account!.Label = "Modified";
            account.Description = "Modified";

            //Act
            var accountResult = await _serviceManager.AccountService.UpdateAccountAsync(account);

            //Assert
            account.Should()
                    .NotBeNull()
                    .And.BeOfType<Account>();
        }

        [Fact]
        public async Task Update_Success_ModifiedAtFieldUpdated()
        {
            //Arrange
            var account = await _serviceManager.AccountService.GetAccountAsync(_testDBValues.AccountId1);

            account!.Label = "Modified";
            account.Description = "Modified";
            var modifiedAt = account.ModifiedAt;

            //Act
            var accountResult = await _serviceManager.AccountService.UpdateAccountAsync(account);

            //Assert
            account.Should()
                    .NotBeNull()
                    .And.Match<Account>(x=>x.ModifiedAt > modifiedAt);
        }


        public static IEnumerable<object[]> GeneratedGuids
        {
            get
            {
                yield return new object[] { Guid.NewGuid() };
                yield return new object[] { Guid.NewGuid() };
                yield return new object[] { Guid.NewGuid() };
            }
        }

        public static IEnumerable<object[]> GetAccounts(int numTests, string accountGroupId)
        {
            var accounts = FakeGenerator.GenerateAccounts(numTests, Guid.Parse(accountGroupId));

            var data = new List<Object[]>();

            foreach (var account in accounts)
            {
                yield return new object[] { account };
            }
        }
    }
}
