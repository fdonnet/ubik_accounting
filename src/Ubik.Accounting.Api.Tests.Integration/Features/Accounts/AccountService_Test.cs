using Bogus;
using FluentAssertions;
using Ubik.Accounting.Api.Data;
using Ubik.Accounting.Api.Features;
using Ubik.Accounting.Api.Models;

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
        public async Task Get_Account_Ok()
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
        public async Task Get_Null_IdNotExists(Guid id)
        {
            //Arrange

            //Act
            var account = await _serviceManager.AccountService.GetAccountAsync(id);

            //Assert
            account.Should()
                    .BeNull();
        }

        [Theory]
        [InlineData("1020",true)]
        [InlineData("ZZZZZZZXX", false)]
        public async Task IfExist_TrueOrFalse_AccountExists(string accountCode, bool result)
        {
            //Arrange

            //Act
            var account = await _serviceManager.AccountService.IfExists(accountCode);

            //Assert
            account.Should().Be(result);
        }

        [Theory]
        [InlineData("1020", "7777f11f-20dd-4888-88f8-428e59bbc535", true)]
        [InlineData("1030", "7777f11f-20dd-4888-88f8-428e59bbc535", false)]
        public async Task IfExistWithDifferentId_TrueorFalse_AccountExists(string accountCode, string currentGuid, bool result)
        {
            //Arrange

            //Act
            var account = await _serviceManager.AccountService.IfExistsWithDifferentId(accountCode,Guid.Parse(currentGuid));

            //Assert
            account.Should().Be(result);
        }

        [Fact]
        public async Task GetAll_Accounts_Ok()
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
        public async Task Add_AddedAccount_Ok(Account account)
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
        public async Task Add_AuditFieldsModified_Ok(Account account)
        {
            //Arrange

            //Act
            var accountResult = await _serviceManager.AccountService.AddAccountAsync(account);

            //Assert
            account.Should().Match<Account>(x => x.ModifiedBy != null && x.ModifiedAt != null);
        }

        [Theory]
        [MemberData(nameof(GetAccounts), parameters: new object[] { 5, "1524f11f-20dd-4888-88f8-428e59bbc22b" })]
        public async Task Add_Exception_AccountGroupIdNotExists(Account account)
        {
            //Arrange

            //Act
            Func<Task> act = async () => await _serviceManager.AccountService.AddAccountAsync(account);

            //Assert
            await act.Should().ThrowAsync<Exception>();
        }

        [Theory]
        [MemberData(nameof(GetAccounts), parameters: new object[] { 5, "1524f11f-20dd-4888-88f8-428e59bbc22b" })]
        public async Task Add_Exception_CrashDb(Account account)
        {
            //Arrange
            account.Code = new string(new Faker("fr_CH").Random.Chars(count: 21));

            //Act
            Func<Task> act = async () => await _serviceManager.AccountService.AddAccountAsync(account);

            //Assert
            await act.Should().ThrowAsync<Exception>();
        }

        [Fact]
        public async Task Update_UpdatedAccount_Ok()
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
        public async Task Update_ModifiedAtFieldUpdated_Ok()
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
