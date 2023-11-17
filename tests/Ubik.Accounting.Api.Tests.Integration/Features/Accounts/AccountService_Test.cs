using FluentAssertions;
using MassTransit;
using Ubik.Accounting.Api.Data.Init;
using Ubik.Accounting.Api.Features;
using Ubik.Accounting.Api.Features.Accounts.Errors;
using Ubik.Accounting.Api.Features.Accounts.Queries.CustomPoco;
using Ubik.Accounting.Api.Models;
using Ubik.Accounting.Api.Tests.Integration.Fake;
using Ubik.ApiService.Common.Errors;

namespace Ubik.Accounting.Api.Tests.Integration.Features.Accounts
{
    public class AccountService_Test : BaseIntegrationTest
    {
        private readonly BaseValuesForAccounts _testValuesForAccounts;
        private readonly BaseValuesForAccountGroups _testValuesForAccountGroups;
        private readonly BaseValuesForCurrencies _testValuesForCurrencies;

        private readonly IServiceManager _serviceManager;

        public AccountService_Test(IntegrationTestWebAppFactory factory) : base(factory)
        {
            _testValuesForAccounts = new BaseValuesForAccounts();
            _testValuesForAccountGroups = new BaseValuesForAccountGroups();
            _testValuesForCurrencies = new BaseValuesForCurrencies();

            _serviceManager = new ServiceManager(DbContext, new FakeUserService());
        }

        [Fact]
        public async Task Get_Account_Ok()
        {
            //Arrange

            //Act
            var result = (await _serviceManager.AccountService.GetAsync(_testValuesForAccounts.AccountId1)).IfLeft(r => default!);

            //Assert
            result.Should()
                    .NotBeNull()
                    .And.BeOfType<Account>()
                    .And.Match<Account>(a => a.Id == _testValuesForAccounts.AccountId1);
        }

        [Fact]
        public async Task GetAccountGroups_AccountGroupsClassification_Ok()
        {
            //Arrange

            //Act
            var result = (await _serviceManager.AccountService.GetAccountGroupsWithClassificationInfoAsync(_testValuesForAccounts.AccountId1)).IfLeft(r => default!);

            //Assert
            result.Should()
                    .NotBeNull()
                    .And.AllBeOfType<AccountGroupClassification>()
                    .And.Match<IEnumerable<AccountGroupClassification>>(a => a.Any());
        }

        [Theory, MemberData(nameof(GeneratedGuids))]
        public async Task Get_AccountNotFoundException_IdNotExists(Guid id)
        {
            //Arrange

            //Act
            var result = (await _serviceManager.AccountService.GetAsync(id)).IfRight(err => default!);
            var addToAccountGroup = (await _serviceManager.AccountService
                .AddInAccountGroupAsync(id, NewId.NextGuid())).IfRight(err => default!);
            var getAccountGroups = (await _serviceManager.AccountService
                .GetAccountGroupsWithClassificationInfoAsync(NewId.NextGuid())).IfRight(err => default!);

            //Assert
            result.Should()
                    .NotBeNull()
                    .And.BeOfType<AccountNotFoundError>()
                    .And.Match<AccountNotFoundError>(a =>
                        a.ErrorType == ServiceAndFeatureErrorType.NotFound);

            addToAccountGroup.Should()
                    .NotBeNull()
                    .And.BeOfType<AccountNotFoundError>()
                    .And.Match<AccountNotFoundError>(a =>
                        a.ErrorType == ServiceAndFeatureErrorType.NotFound);


            getAccountGroups.Should()
                    .NotBeNull()
                    .And.BeOfType<AccountNotFoundError>()
                    .And.Match<AccountNotFoundError>(a =>
                        a.ErrorType == ServiceAndFeatureErrorType.NotFound);
        }

        //[Theory]
        //[InlineData("1020", true)]
        //[InlineData("ZZZZZZZXX", false)]
        //public async Task IfExist_TrueOrFalse_Ok(string accountCode, bool resultNeeded)
        //{
        //    //Arrange

        //    //Act
        //    var result = await _serviceManager.AccountService.ValidateIfNotAlreadyExists(accountCode);

        //    //Assert
        //    result.Should().Be(resultNeeded);
        //}

        //[Theory]
        //[InlineData("1020", "7777f11f-20dd-4888-88f8-428e59bbc535", true)]
        //[InlineData("zzzz999", "7777f11f-20dd-4888-88f8-428e59bbc535", false)]
        //public async Task IfExistWithDifferentId_TrueorFalse_Ok(string accountCode, string currentGuid, bool resultNeeded)
        //{
        //    //Arrange

        //    //Act
        //    var result = await _serviceManager.AccountService.IfExistsWithDifferentIdAsync(accountCode, Guid.Parse(currentGuid));

        //    //Assert
        //    result.Should().Be(resultNeeded);
        //}

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
            var result = (await _serviceManager.AccountService.AddAsync(account)).IfLeft(r => default!);

            //Assert
            result.Should()
                    .NotBeNull()
                    .And.BeOfType<Account>()
                    .And.Match<Account>(a => a.Code == account.Code);
        }

        [Theory]
        [MemberData(nameof(GetAccounts), parameters: new object[] { 5, "ccfe1b29-6d1b-420c-ac64-fc8f1a6153a1", "1020" })]
        public async Task Add_AccountAlreadyExistsException_AccountCodeAlreadyExists(Account account)
        {
            //Arrange

            //Act
            var result = (await _serviceManager.AccountService.AddAsync(account)).IfRight(err => default!);

            //Assert
            result.Should()
                    .NotBeNull()
                    .And.BeOfType<AccountAlreadyExistsError>()
                    .And.Match<AccountAlreadyExistsError>(a =>
                        a.ErrorType == ServiceAndFeatureErrorType.Conflict);
        }

        [Theory]
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        [MemberData(nameof(GetAccounts), parameters: new object[] { 5, "ccfe1b29-6d1b-420c-ac64-fc8f1a6153a7", null })]
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        public async Task Add_AccountCurrencyNotFoundException_CurrencyIdNotFound(Account account)
        {
            //Arrange

            //Act
            var result = (await _serviceManager.AccountService.AddAsync(account)).IfRight(err => default!);

            //Assert
            result.Should()
                    .NotBeNull()
                    .And.BeOfType<AccountCurrencyNotFoundError>()
                    .And.Match<AccountCurrencyNotFoundError>(a =>
                        a.ErrorType == ServiceAndFeatureErrorType.BadParams);
        }


        [Theory]
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        [MemberData(nameof(GetAccounts), parameters: new object[] { 5, "ccfe1b29-6d1b-420c-ac64-fc8f1a6153a1", null })]
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        public async Task Add_AuditFieldsModified_Ok(Account account)
        {
            //Arrange

            //Act
            var result = (await _serviceManager.AccountService.AddAsync(account)).IfLeft(r => default!);

            //Assert
            result.Should().Match<Account>(x => x.ModifiedBy != null
                    && x.ModifiedAt != null
                    && x.Code == account.Code);
        }

        [Fact]
        public async Task Update_UpdatedAccount_Ok()
        {
            //Arrange
            var account = (await _serviceManager.AccountService.GetAsync(_testValuesForAccounts.AccountId1)).IfLeft(r => default!);

            account!.Label = "Modified";
            account.Description = "Modified";

            //Act
            var result = (await _serviceManager.AccountService.UpdateAsync(account)).IfLeft(r => default!);

            //Assert
            result.Should()
                    .NotBeNull()
                    .And.BeOfType<Account>()
                    .And.Match<Account>(a =>
                        a.Label == "Modified"
                        && a.Version != account.Version
                        && a.Id == account.Id);
        }

        [Fact]
        public async Task Update_AccountNotFoundException_AccountIdNotFound()
        {
            //Arrange
            var account = new Account { Id = Guid.NewGuid(), Code = "TEST", Label = "TEST", CurrencyId =_testValuesForCurrencies.CurrencyId1 };

            account!.Label = "Modified";
            account.Description = "Modified";

            //Act
            var result = (await _serviceManager.AccountService.UpdateAsync(account)).IfRight(err => default!);

            //Assert
            result.Should()
                    .NotBeNull()
                    .And.BeOfType<AccountNotFoundError>()
                    .And.Match<AccountNotFoundError>(a =>
                        a.ErrorType == ServiceAndFeatureErrorType.NotFound);
        }

        [Fact]
        public async Task Update_AccountAlreadyExistsException_AccountCodeAlreadyExistsForAnotherAccount()
        {
            //Arrange
            var account = new Account { Id = _testValuesForAccounts.AccountId2, Code = _testValuesForAccounts.AccountCode1, Label = "TEST", CurrencyId = Guid.NewGuid() };

            account!.Label = "Modified";
            account.Description = "Modified";

            //Act
            var result = (await _serviceManager.AccountService.UpdateAsync(account)).IfRight(err => default!);

            //Assert
            result.Should()
                    .NotBeNull()
                    .And.BeOfType<AccountAlreadyExistsError>()
                    .And.Match<AccountAlreadyExistsError>(a =>
                        a.ErrorType == ServiceAndFeatureErrorType.Conflict);
        }

        [Fact]
        public async Task Update_AccountCurrencyNotFoundException_AccountCurrencyNotFound()
        {
            //Arrange
            var account = new Account { Id = _testValuesForAccounts.AccountId1, Code = _testValuesForAccounts.AccountCode1, Label = "TEST", CurrencyId = Guid.NewGuid() };

            account!.Label = "Modified";
            account.Description = "Modified";

            //Act
            var result = (await _serviceManager.AccountService.UpdateAsync(account)).IfRight(err => default!);

            //Assert
            result.Should()
                    .NotBeNull()
                    .And.BeOfType<AccountCurrencyNotFoundError>()
                    .And.Match<AccountCurrencyNotFoundError>(a =>
                        a.ErrorType == ServiceAndFeatureErrorType.BadParams);
        }

        [Fact]
        public async Task Update_ModifiedAtFieldUpdated_Ok()
        {
            //Arrange
            var account = (await _serviceManager.AccountService.GetAsync(_testValuesForAccounts.AccountId1)).IfLeft(r => default!);

            account!.Label = "Modified";
            account.Description = "Modified";
            var modifiedAt = account.ModifiedAt;

            //Act
            var result = (await _serviceManager.AccountService.UpdateAsync(account)).IfLeft(r => default!);

            //Assert
            result.Should()
                    .NotBeNull()
                    .And.Match<Account>(x => x.ModifiedAt > modifiedAt && x.Id == _testValuesForAccounts.AccountId1);
        }

        [Fact]
        public async Task Delete_True_Ok()
        {
            //Arrange

            //Act
            await _serviceManager.AccountService.ExecuteDeleteAsync(_testValuesForAccounts.AccountIdForDel);
            var exist = (await _serviceManager.AccountService.GetAsync(_testValuesForAccounts.AccountIdForDel)).IsRight;

            //Assert
            exist.Should()
                .BeFalse();
        }

        [Fact]
        public async Task Delete_AccountNotFoundException_AccountIdNotFound()
        {
            //Arrange

            //Act
            var result = (await _serviceManager.AccountService.ExecuteDeleteAsync(Guid.NewGuid())).IfRight(err => default!);

            //Assert
            result.Should()
                     .NotBeNull()
                     .And.BeOfType<AccountNotFoundError>()
                     .And.Match<AccountNotFoundError>(a =>
                         a.ErrorType == ServiceAndFeatureErrorType.NotFound);
        }

        [Fact]
        public async Task AddInAccountGroup_AccountAccountGroup_Ok()
        {
            //Arrange

            //Act
            var result = (await _serviceManager.AccountService
                .AddInAccountGroupAsync(_testValuesForAccounts.AccountId2,
                _testValuesForAccountGroups.AccountGroupId1)).IfLeft(r => default!);

            //Assert
            result.Should()
                    .NotBeNull()
                    .And.BeOfType<AccountAccountGroup>()
                    .And.Match<AccountAccountGroup>(a =>
                        a.AccountGroupId == _testValuesForAccountGroups.AccountGroupId1
                        && a.AccountId == _testValuesForAccounts.AccountId2);
        }


        [Fact]
        public async Task AddInAccountGroup_AccountGroupNotFoundForAccountException_AccountGroupIdNotFound()
        {
            //Arrange

            //Act
            var result = (await _serviceManager.AccountService
                .AddInAccountGroupAsync(_testValuesForAccounts.AccountId2,
               Guid.NewGuid())).IfRight(r => default!);

            //Assert
            result.Should()
                    .NotBeNull()
                    .And.BeOfType<AccountGroupNotFoundForAccountError>()
                    .And.Match<AccountGroupNotFoundForAccountError>(a =>
                        a.ErrorType == ServiceAndFeatureErrorType.BadParams);
        }

        [Fact]
        public async Task AddInAccountGroup_AccountAlreadyExistsInClassificationException_AccountAlreadyAttached()
        {
            //Arrange

            //Act
            var result = (await _serviceManager.AccountService
                .AddInAccountGroupAsync(_testValuesForAccounts.AccountId1,
                _testValuesForAccountGroups.AccountGroupId2)).IfRight(r => default!);

            //Assert
            result.Should()
                    .NotBeNull()
                    .And.BeOfType<AccountAlreadyExistsInClassificationError>()
                    .And.Match<AccountAlreadyExistsInClassificationError>(a =>
                        a.ErrorType == ServiceAndFeatureErrorType.Conflict);
        }

        [Fact]
        public async Task DeleteInAccountGroup_AccountAccountGroup_Ok()
        {
            //Arrange

            //Act
            var result = (await _serviceManager.AccountService
                .DeleteFromAccountGroupAsync(_testValuesForAccounts.AccountId3,
                _testValuesForAccountGroups.AccountGroupId2)).IfLeft(r => default!);

            //Assert
            result.Should()
                    .NotBeNull()
                    .And.BeOfType<AccountAccountGroup>()
                    .And.Match<AccountAccountGroup>(a =>
                        a.AccountGroupId == _testValuesForAccountGroups.AccountGroupId2
                        && a.AccountId == _testValuesForAccounts.AccountId3);
        }

        [Fact]
        public async Task DeleteInAccountGroup_AccountAlreadyExistsInClassificationException_AccountAlreadyAttached()
        {
            //Arrange

            //Act
            var result = (await _serviceManager.AccountService
                .DeleteFromAccountGroupAsync(NewId.NextGuid(),
                NewId.NextGuid())).IfRight(r => default!);

            //Assert
            result.Should()
                    .NotBeNull()
                    .And.BeOfType<AccountNotExistsInAccountGroupError>()
                    .And.Match<AccountNotExistsInAccountGroupError>(a =>
                        a.ErrorType == ServiceAndFeatureErrorType.NotFound);
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

