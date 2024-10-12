using FluentAssertions;
using MassTransit;
using Ubik.Accounting.Api.Data.Init;
using Ubik.Accounting.Api.Features;
using Ubik.Accounting.Api.Features.Accounts.Errors;
using Ubik.Accounting.Api.Features.Accounts.Queries.CustomPoco;
using Ubik.Accounting.Api.Models;
using Ubik.Api.Tests.Integration.Fake;
using Ubik.ApiService.Common.Errors;

namespace Ubik.Api.Tests.Integration.Features.Accounting.Accounts
{
    public class AccountService_Test : BaseIntegrationTestOld
    {
        private readonly BaseValuesForAccounts _testValuesForAccounts;
        private readonly BaseValuesForAccountGroups _testValuesForAccountGroups;
        private readonly BaseValuesForCurrencies _testValuesForCurrencies;

        private readonly IServiceManager _serviceManager;

        public AccountService_Test(IntegrationTestAccoutingFactory factory) : base(factory)
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
                .AddInAccountGroupAsync(new AccountAccountGroup() { AccountId = id, AccountGroupId = NewId.NextGuid() })).IfRight(err => default!);
            var getAccountGroups = (await _serviceManager.AccountService
                .GetAccountGroupsWithClassificationInfoAsync(NewId.NextGuid())).IfRight(err => default!);

            //Assert
            result.Should()
                    .NotBeNull()
                    .And.BeOfType<ResourceNotFoundError>();

            addToAccountGroup.Should()
                    .NotBeNull()
                    .And.BeOfType<ResourceNotFoundError>();


            getAccountGroups.Should()
                    .NotBeNull()
                    .And.BeOfType<ResourceNotFoundError>();
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

        [Fact]
        public async Task GetAllAccountGroupLinks_AccountGroupLinks_Ok()
        {
            //Arrange

            //Act
            var result = await _serviceManager.AccountService.GetAllAccountGroupLinksAsync();

            //Assert
            result.Should()
                    .NotBeNull()
                    .And.AllBeOfType<AccountAccountGroup>();
        }

        [Theory]
        [MemberData(nameof(GetAccounts), parameters: [5, "248e0000-5dd4-0015-38c5-08dcd98e5b2d", null])]
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
        [MemberData(nameof(GetAccounts), parameters: [5, "248e0000-5dd4-0015-38c5-08dcd98e5b2d", "1020"])]
        public async Task Add_AccountAlreadyExistsException_AccountCodeAlreadyExists(Account account)
        {
            //Arrange

            //Act
            var result = (await _serviceManager.AccountService.AddAsync(account)).IfRight(err => default!);

            //Assert
            result.Should()
                    .NotBeNull()
                    .And.BeOfType<ResourceAlreadyExistsError>();
        }

        [Theory]
        [MemberData(nameof(GetAccounts), parameters: [5, "ccfe1b29-6d1b-420c-ac64-fc8f1a6153a7", null])]
        public async Task Add_AccountCurrencyNotFoundException_CurrencyIdNotFound(Account account)
        {
            //Arrange

            //Act
            var result = (await _serviceManager.AccountService.AddAsync(account)).IfRight(err => default!);

            //Assert
            result.Should()
                    .NotBeNull()
                    .And.BeOfType<BadParamExternalResourceNotFound>();
        }


        [Theory]
        [MemberData(nameof(GetAccounts), parameters: [5, "248e0000-5dd4-0015-38c5-08dcd98e5b2d", null])]
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
            var account = new Account { Id = Guid.NewGuid(), Code = "TEST", Label = "TEST", CurrencyId = _testValuesForCurrencies.CurrencyId1 };

            account!.Label = "Modified";
            account.Description = "Modified";

            //Act
            var result = (await _serviceManager.AccountService.UpdateAsync(account)).IfRight(err => default!);

            //Assert
            result.Should()
                    .NotBeNull()
                    .And.BeOfType<ResourceNotFoundError>();
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
                    .And.BeOfType<ResourceAlreadyExistsError>();
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
                    .And.BeOfType<BadParamExternalResourceNotFound>();
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
                     .And.BeOfType<ResourceNotFoundError>();
        }

        [Fact]
        public async Task AddInAccountGroup_AccountAccountGroup_Ok()
        {
            //Arrange

            //Act
            var result = (await _serviceManager.AccountService
                .AddInAccountGroupAsync(new AccountAccountGroup()
                {
                    AccountId = _testValuesForAccounts.AccountForAttach2,
                    AccountGroupId =
                _testValuesForAccountGroups.AccountGroupForAttach2
                })).IfLeft(r => default!);

            //Assert
            result.Should()
                    .NotBeNull()
                    .And.BeOfType<AccountAccountGroup>()
                    .And.Match<AccountAccountGroup>(a =>
                        a.AccountGroupId == _testValuesForAccountGroups.AccountGroupForAttach2
                        && a.AccountId == _testValuesForAccounts.AccountForAttach2);
        }


        [Fact]
        public async Task AddInAccountGroup_AccountGroupNotFoundForAccountException_AccountGroupIdNotFound()
        {
            //Arrange

            //Act
            var result = (await _serviceManager.AccountService
                .AddInAccountGroupAsync(new()
                {
                    AccountId = _testValuesForAccounts.AccountId2,
                    AccountGroupId = Guid.NewGuid()
                })).IfRight(r => default!);

            //Assert
            result.Should()
                    .NotBeNull()
                    .And.BeOfType<BadParamExternalResourceNotFound>();
        }

        [Fact]
        public async Task AddInAccountGroup_AccountAlreadyExistsInClassificationException_AccountAlreadyAttached()
        {
            //Arrange

            //Act
            var result = (await _serviceManager.AccountService
                .AddInAccountGroupAsync(new()
                {
                    AccountId = _testValuesForAccounts.AccountId1,
                    AccountGroupId = _testValuesForAccountGroups.AccountGroupIdFirstLvl1
                })).IfRight(r => default!);

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

