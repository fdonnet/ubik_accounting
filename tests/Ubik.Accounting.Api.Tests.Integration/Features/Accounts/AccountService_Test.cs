using FluentAssertions;
using Ubik.Accounting.Api.Data.Init;
using Ubik.Accounting.Api.Features;
using Ubik.Accounting.Api.Features.Accounts.Exceptions;
using Ubik.Accounting.Api.Models;
using Ubik.ApiService.Common.Exceptions;

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
            var result = (await _serviceManager.AccountService.GetAsync(_testValuesForAccounts.AccountId1)).Result;

            //Assert
            result.Should()
                    .NotBeNull()
                    .And.BeOfType<Account>();
        }

        [Theory, MemberData(nameof(GeneratedGuids))]
        public async Task Get_AccountNotFoundException_IdNotExists(Guid id)
        {
            //Arrange

            //Act
            var result = (await _serviceManager.AccountService.GetAsync(id)).Exception;

            //Assert
            result.Should()
                    .NotBeNull()
                    .And.BeOfType<AccountNotFoundException>()
                    .And.Match<AccountNotFoundException>(a =>
                        a.ErrorType == ServiceAndFeatureExceptionType.NotFound);
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
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        [MemberData(nameof(GetAccounts), parameters: new object[] { 5, "ccfe1b29-6d1b-420c-ac64-fc8f1a6153a1", null })]
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        public async Task Add_Account_Ok(Account account)
        {
            //Arrange

            //Act
            var result = (await _serviceManager.AccountService.AddAsync(account)).Result;

            //Assert
            result.Should()
                    .NotBeNull()
                    .And.BeOfType<Account>();
        }

        [Theory]
        [MemberData(nameof(GetAccounts), parameters: new object[] { 5, "ccfe1b29-6d1b-420c-ac64-fc8f1a6153a1", "1020" })]
        public async Task Add_AccountAlreadyExistsException_AccountCodeAlreadyExists(Account account)
        {
            //Arrange

            //Act
            var result = (await _serviceManager.AccountService.AddAsync(account)).Exception;

            //Assert
            result.Should()
                    .NotBeNull()
                    .And.BeOfType<AccountAlreadyExistsException>()
                    .And.Match<AccountAlreadyExistsException>(a =>
                        a.ErrorType == ServiceAndFeatureExceptionType.Conflict);
        }

        [Theory]
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        [MemberData(nameof(GetAccounts), parameters: new object[] { 5, "ccfe1b29-6d1b-420c-ac64-fc8f1a6153a7", null })]
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        public async Task Add_AccountCurrencyNotFoundException_CurrencyIdNotFound(Account account)
        {
            //Arrange

            //Act
            var result = (await _serviceManager.AccountService.AddAsync(account)).Exception;

            //Assert
            result.Should()
                    .NotBeNull()
                    .And.BeOfType<AccountCurrencyNotFoundException>()
                    .And.Match<AccountCurrencyNotFoundException>(a =>
                        a.ErrorType == ServiceAndFeatureExceptionType.BadParams);
        }


        [Theory]
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        [MemberData(nameof(GetAccounts), parameters: new object[] { 5, "ccfe1b29-6d1b-420c-ac64-fc8f1a6153a1", null })]
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        public async Task Add_AuditFieldsModified_Ok(Account account)
        {
            //Arrange

            //Act
            var result = (await _serviceManager.AccountService.AddAsync(account)).Result;

            //Assert
            result.Should().Match<Account>(x => x.ModifiedBy != null && x.ModifiedAt != null);
        }

        [Fact]
        public async Task Update_UpdatedAccount_Ok()
        {
            //Arrange
            var account = (await _serviceManager.AccountService.GetAsync(_testValuesForAccounts.AccountId1)).Result;

            account!.Label = "Modified";
            account.Description = "Modified";

            //Act
            var result = (await _serviceManager.AccountService.UpdateAsync(account)).Result;

            //Assert
            result.Should()
                    .NotBeNull()
                    .And.BeOfType<Account>()
                    .And.Match<Account>(a => 
                        a.Label == "Modified"
                        && a.Version != account.Version);
        }

        [Fact]
        public async Task Update_AccountNotFoundException_AccountIdNotFound()
        {
            //Arrange
            var account = new Account { Id = Guid.NewGuid(), Code="TEST",Label="TEST", CurrencyId=Guid.NewGuid()};

            account!.Label = "Modified";
            account.Description = "Modified";

            //Act
            var result = (await _serviceManager.AccountService.UpdateAsync(account)).Exception;

            //Assert
            result.Should()
                    .NotBeNull()
                    .And.BeOfType<AccountNotFoundException>()
                    .And.Match<AccountNotFoundException>(a =>
                        a.ErrorType == ServiceAndFeatureExceptionType.NotFound);
        }

        [Fact]
        public async Task Update_AccountAlreadyExistsException_AccountCodeAlreadyExistsForAnotherAccount()
        {
            //Arrange
            var account = new Account { Id = _testValuesForAccounts.AccountId2, Code = _testValuesForAccounts.AccountCode1, Label = "TEST", CurrencyId = Guid.NewGuid() };

            account!.Label = "Modified";
            account.Description = "Modified";

            //Act
            var result = (await _serviceManager.AccountService.UpdateAsync(account)).Exception;

            //Assert
            result.Should()
                    .NotBeNull()
                    .And.BeOfType<AccountAlreadyExistsException>()
                    .And.Match<AccountAlreadyExistsException>(a =>
                        a.ErrorType == ServiceAndFeatureExceptionType.Conflict);
        }

        [Fact]
        public async Task Update_AccountCurrencyNotFoundException_AccountCurrencyNotFound()
        {
            //Arrange
            var account = new Account { Id = _testValuesForAccounts.AccountId1, Code = _testValuesForAccounts.AccountCode1, Label = "TEST", CurrencyId = Guid.NewGuid() };

            account!.Label = "Modified";
            account.Description = "Modified";

            //Act
            var result = (await _serviceManager.AccountService.UpdateAsync(account)).Exception;

            //Assert
            result.Should()
                    .NotBeNull()
                    .And.BeOfType<AccountCurrencyNotFoundException>()
                    .And.Match<AccountCurrencyNotFoundException>(a =>
                        a.ErrorType == ServiceAndFeatureExceptionType.BadParams);
        }

        [Fact]
        public async Task Update_ModifiedAtFieldUpdated_Ok()
        {
            //Arrange
            var account = (await _serviceManager.AccountService.GetAsync(_testValuesForAccounts.AccountId1)).Result;

            account!.Label = "Modified";
            account.Description = "Modified";
            var modifiedAt = account.ModifiedAt;

            //Act
            var result = (await _serviceManager.AccountService.UpdateAsync(account)).Result;

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
            var exist = (await _serviceManager.AccountService.GetAsync(_testValuesForAccounts.AccountIdForDel)).IsSuccess;

            //Assert
            exist.Should()
                .BeFalse();
        }

        [Fact]
        public async Task Delete_AccountNotFoundException_AccountIdNotFound()
        {
            //Arrange

            //Act
            var result = (await _serviceManager.AccountService.ExecuteDeleteAsync(Guid.NewGuid())).Exception;

            //Assert
            result.Should()
                     .NotBeNull()
                     .And.BeOfType<AccountNotFoundException>()
                     .And.Match<AccountNotFoundException>(a =>
                         a.ErrorType == ServiceAndFeatureExceptionType.NotFound);
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

        public static IEnumerable<object[]> GetAccounts(int numTests, string currencyId, string? code = null)
        {
            var accounts = FakeGenerator.GenerateAccounts(numTests, Guid.Parse(currencyId), code: code);

            foreach (var account in accounts)
            {
                yield return new object[] { account };
            }
        }
    }
}

