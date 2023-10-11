using Bogus;
using Bogus.Extensions;
using FluentAssertions;
using MediatR.NotificationPublishers;
using Ubik.Accounting.Api.Data;
using Ubik.Accounting.Api.Features;
using Ubik.Accounting.Api.Features.Accounts.Exceptions;
using Ubik.Accounting.Api.Models;
using Ubik.ApiService.Common.Exceptions;

namespace Ubik.Accounting.Api.Test.Features.Accounts
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
        public async Task GetAccountAsync_Success()
        {
            //Arrange

            //Act
            var account = await _serviceManager.AccountService.GetAccountAsync(_testDBValues.AccountId1);

            //Assert
            account.Should()
                    .NotBeNull()
                    .And.Match<Account>(x => x.Code == _testDBValues.AccountCode1);
        }

        [Theory, MemberData(nameof(GeneratedGuids))]
        public async Task GetAccountAsync_WhenIdNotExists_ThenReturnNull(Guid id)
        {
            //Arrange

            //Act
            var account = await _serviceManager.AccountService.GetAccountAsync(id);

            //Assert
            account.Should()
                    .BeNull();
        }

        [Fact]
        public async Task GetAccountsAsync_Success()
        {
            //Arrange
            //Act
            var accounts = await _serviceManager.AccountService.GetAccountsAsync();

            //Assert
            accounts.Should()
                    .NotBeNull()
                    .And.Contain(x => x.Code == _testDBValues.AccountCode1);
        }

        [Theory]
        [MemberData(nameof(GetAccounts), parameters: new object[] { 5, "1524f11f-20dd-4888-88f8-428e59bbc22a"})]
        public async Task AddAccountAsync_Success(Account account)
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
        [MemberData(nameof(GetAccounts), parameters: new object[] { 5, "1524f11f-20dd-4888-88f8-428e59bbc22b" })]
        public async Task AddAccountAsync_WhenAccountGroupIdNotExists_ThenReturnException(Account account)
        {
            //Arrange

            //Act
            Func<Task> act = async () => await _serviceManager.AccountService.AddAccountAsync(account);

            //Assert
            await act.Should().ThrowAsync<Exception>();
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
