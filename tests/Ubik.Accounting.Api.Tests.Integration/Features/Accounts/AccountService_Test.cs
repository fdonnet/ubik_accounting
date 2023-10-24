using FluentAssertions;
using NSubstitute;
using Ubik.Accounting.Api.Data.Init;
using Ubik.Accounting.Api.Features;
using Ubik.Accounting.Api.Models;
using Ubik.ApiService.Common.Services;

namespace Ubik.Accounting.Api.Tests.Integration.Features.Accounts
{
    public class AccountService_Test : BaseIntegrationTest
    {
        private readonly BaseValuesForAccounts _testValuesForAccounts;
        private readonly IServiceManager _serviceManager;

        public AccountService_Test(IntegrationTestWebAppFactory factory) : base(factory) 
        {
            _testValuesForAccounts = new BaseValuesForAccounts();
           _serviceManager =new ServiceManager(DbContext);
        }

        [Fact]
        public async Task Get_Account_Ok()
        {
            //Arrange
                
            //Act
            var result = await _serviceManager.AccountService.GetAsync(_testValuesForAccounts.AccountId1);

            //Assert
            result.Should()
                    .NotBeNull()
                    .And.BeOfType<Account>();
        }

        [Theory, MemberData(nameof(GeneratedGuids))]
        public async Task Get_Null_IdNotExists(Guid id)
        {
            //Arrange

            //Act
            var result = await _serviceManager.AccountService.GetAsync(id);

            //Assert
            result.Should()
                    .BeNull();
        }

        [Theory]
        [InlineData("1020", true)]
        [InlineData("ZZZZZZZXX", false)]
        public async Task IfExist_TrueOrFalse_Ok(string accountCode, bool resultNeeded)
        {
            //Arrange

            //Act
            var result = await _serviceManager.AccountService.IfExistsAsync(accountCode);

            //Assert
            result.Should().Be(resultNeeded);
        }

        [Theory]
        [InlineData("1020", "7777f11f-20dd-4888-88f8-428e59bbc535", true)]
        [InlineData("zzzz999", "7777f11f-20dd-4888-88f8-428e59bbc535", false)]
        public async Task IfExistWithDifferentId_TrueorFalse_Ok(string accountCode, string currentGuid, bool resultNeeded)
        {
            //Arrange

            //Act
            var result = await _serviceManager.AccountService.IfExistsWithDifferentIdAsync(accountCode, Guid.Parse(currentGuid));

            //Assert
            result.Should().Be(resultNeeded);
        }

        [Fact]
        public async Task GetAll_Accounts_Ok()
        {
            //Arrange

            //Act
            var result = await _serviceManager.AccountService.GetAllAsync();

            //Assert
            result.Should()
                    .NotBeNull()
                    .And.AllBeOfType<Account>();
        }

        [Theory]
        [MemberData(nameof(GetAccounts), parameters: new object[] { 5 })]
        public async Task Add_Account_Ok(Account account)
        {
            //Arrange

            //Act
            var result = await _serviceManager.AccountService.AddAsync(account);

            //Assert
            result.Should()
                    .NotBeNull()
                    .And.BeOfType<Account>();
        }

        [Theory]
        [MemberData(nameof(GetAccounts), parameters: new object[] { 5 })]
        public async Task Add_AuditFieldsModified_Ok(Account account)
        {
            //Arrange

            //Act
            var result = await _serviceManager.AccountService.AddAsync(account);

            //Assert
            result.Should().Match<Account>(x => x.ModifiedBy != null && x.ModifiedAt != null);
        }

        [Fact]
        public async Task Update_UpdatedAccount_Ok()
        {
            //Arrange
            var account = await _serviceManager.AccountService.GetAsync(_testValuesForAccounts.AccountId1);

            account!.Label = "Modified";
            account.Description = "Modified";

            //Act
            var result = _serviceManager.AccountService.Update(account);

            //Assert
            result.Should()
                    .NotBeNull()
                    .And.BeOfType<Account>();
        }

        [Fact]
        public async Task Update_ModifiedAtFieldUpdated_Ok()
        {
            //Arrange
            var account = await _serviceManager.AccountService.GetAsync(_testValuesForAccounts.AccountId1);

            account!.Label = "Modified";
            account.Description = "Modified";
            var modifiedAt = account.ModifiedAt;

            //Act
            var result = _serviceManager.AccountService.Update(account);

            //Assert
            result.Should()
                    .NotBeNull()
                    .And.Match<Account>(x => x.ModifiedAt > modifiedAt);
        }

        [Fact]
        public async Task Delete_True_Ok()
        {
            //Arrange
            
            //Act
            await _serviceManager.AccountService.ExecuteDeleteAsync(_testValuesForAccounts.AccountIdForDel);
            var exist = (await _serviceManager.AccountService.GetAsync(_testValuesForAccounts.AccountIdForDel)) != null;

            //Assert
            exist.Should()
                .BeFalse();
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

        public static IEnumerable<object[]> GetAccounts(int numTests)
        {
            var accounts = FakeGenerator.GenerateAccounts(numTests);

            foreach (var account in accounts)
            {
                yield return new object[] { account };
            }
        }
    }
}

