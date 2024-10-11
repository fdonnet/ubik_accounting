using FluentAssertions;
using Ubik.Accounting.Api.Data.Init;
using Ubik.Accounting.Api.Features;
using Ubik.Accounting.Api.Features.AccountGroups.Errors;
using Ubik.Accounting.Api.Models;
using Ubik.Api.Tests.Integration.Fake;
using Ubik.ApiService.Common.Errors;

namespace Ubik.Api.Tests.Integration.Features.Accounting.AccountGroups
{
    public class AccountGroupService_Test : BaseIntegrationTest
    {
        private readonly BaseValuesForAccountGroups _testAccountGroupValues;
        private readonly BaseValuesForClassifications _testClassifications;
        private readonly IServiceManager _serviceManager;


        public AccountGroupService_Test(IntegrationTestAccoutingFactory factory) : base(factory)
        {
            _testAccountGroupValues = new BaseValuesForAccountGroups();
            _testClassifications = new BaseValuesForClassifications();
            _serviceManager = new ServiceManager(DbContext, new FakeUserService());
        }

        [Fact]
        public async Task GetAll_AccountGroups_Ok()
        {
            //Arrange

            //Act
            var result = await _serviceManager.AccountGroupService.GetAllAsync();

            //Assert
            result.Should()
                    .NotBeNull()
                    .And.AllBeOfType<AccountGroup>();
        }

        [Fact]
        public async Task Get_AccountGroup_Ok()
        {
            //Arrange

            //Act
            var result = (await _serviceManager.AccountGroupService.GetAsync(_testAccountGroupValues.AccountGroupId1)).IfLeft(a => default!);

            //Assert
            result.Should()
                    .NotBeNull()
                    .And.BeOfType<AccountGroup>()
                    .And.Match<AccountGroup>(a => a.Id == _testAccountGroupValues.AccountGroupId1);
        }

        [Theory, MemberData(nameof(GeneratedGuids))]
        public async Task Get_AccountGroupNotFoundException_IdNotExists(Guid id)
        {
            //Arrange

            //Act
            var result = (await _serviceManager.AccountGroupService.GetAsync(id)).IfRight(a => default!);

            //Assert
            result.Should()
           .NotBeNull()
           .And.BeOfType<ResourceNotFoundError>();
        }

        [Fact]
        public async Task Get_Accounts_OkGetChildAccounts()
        {
            //Arrange

            //Act
            var result = (await _serviceManager.AccountGroupService
                .GetChildAccountsAsync(_testAccountGroupValues.AccountGroupId1)).IfLeft(err => default!);

            //Assert
            result.Should()
                .NotBeNull()
                .And.AllBeOfType<Account>();
        }

        [Theory]
        [MemberData(nameof(GetAccountGroups), parameters: [5])]
        public async Task Add_AccountGroup_Ok(AccountGroup accountGroup)
        {
            //Arrange

            //Act
            var result = (await _serviceManager.AccountGroupService.AddAsync(accountGroup)).IfLeft(err => default!);

            //Assert
            result.Should()
                    .NotBeNull()
                    .And.BeOfType<AccountGroup>()
                    .And.Match<AccountGroup>(ag => ag.Code == accountGroup.Code);
        }

        [Fact]
        public async Task Add_AccountGroupAlreadyExistsException_AccountGroupCodeAlreadyExists()
        {
            //Arrange
            var accountGroup = new AccountGroup
            {
                Code = "10",
                Label = "Test",
                ClassificationId = _testClassifications.ClassificationId1
            };

            //Act
            var result = (await _serviceManager.AccountGroupService.AddAsync(accountGroup)).IfRight(a => default!);

            //Assert
            result.Should()
                    .NotBeNull()
                    .And.BeOfType<ResourceAlreadyExistsError>();
        }

        [Fact]
        public async Task Add_AccountGroupParentNotFoundException_AccountGroupCodeAlreadyExists()
        {
            //Arrange
            var accountGroup = new AccountGroup
            {
                Code = "102",
                Label = "Test",
                ParentAccountGroupId = Guid.NewGuid()
            };

            //Act
            var result = (await _serviceManager.AccountGroupService.AddAsync(accountGroup)).IfRight(err => default!);

            //Assert
            result.Should()
                    .NotBeNull()
                    .And.BeOfType<AccountGroupParentNotFoundError>()
                    .And.Match<AccountGroupParentNotFoundError>(a =>
                        a.ErrorType == ServiceAndFeatureErrorType.BadParams);
        }

        [Fact]
        public async Task Add_AccountGroupClassificationNotFound_AccountGroupClassificationIdNotFound()
        {
            //Arrange
            var accountGroup = new AccountGroup
            {
                Code = "zzz",
                Label = "Test",
                ParentAccountGroupId = null,
                ClassificationId = Guid.NewGuid()
            };

            //Act
            var result = (await _serviceManager.AccountGroupService.AddAsync(accountGroup)).IfRight(err => default!);

            //Assert
            result.Should()
                    .NotBeNull()
                    .And.BeOfType<BadParamExternalResourceNotFound>();
        }

        [Theory]
        [MemberData(nameof(GetAccountGroups), parameters: [5])]
        public async Task Add_AuditFieldsModified_Ok(AccountGroup accountGroup)
        {
            //Arrange

            //Act
            var result = (await _serviceManager.AccountGroupService.AddAsync(accountGroup)).IfLeft(r => default!);

            //Assert
            result.Should().Match<AccountGroup>(x => x.ModifiedBy != null && x.ModifiedAt != null);
        }

        [Fact]
        public async Task Delete_True_Ok()
        {
            //Arrange

            //Act
            await _serviceManager.AccountGroupService.DeleteAsync(_testAccountGroupValues.AccountGroupIdForDel);
            var exist = (await _serviceManager.AccountGroupService.GetAsync(_testAccountGroupValues.AccountGroupIdForDel)).IsRight;

            //Assert
            exist.Should()
                .BeFalse();
        }

        [Fact]
        public async Task Delete_AccountGroupNotFoundException_AccountGroupIdNotFound()
        {
            //Arrange

            //Act
            var result = (await _serviceManager.AccountGroupService.DeleteAsync(Guid.NewGuid())).IfRight(err => default!);

            //Assert
            result.Should()
                     .NotBeNull()
                     .And.BeOfType<ResourceNotFoundError>();
        }

        [Fact]
        public async Task Update_ModifiedAtFieldUpdated_Ok()
        {
            //Arrange
            var accountGroup = (await _serviceManager.AccountGroupService
                .GetAsync(_testAccountGroupValues.AccountGroupId1)).IfLeft(ag => default!);

            accountGroup!.Label = "Modified";
            accountGroup.Description = "Modified";

            var modifiedAt = accountGroup.ModifiedAt;

            //Act
            var result = (await _serviceManager.AccountGroupService.UpdateAsync(accountGroup)).IfLeft(r => default!);

            //Assert
            result.Should()
                    .NotBeNull()
                    .And.Match<AccountGroup>(x => x.ModifiedAt > modifiedAt && x.Id == accountGroup.Id);
        }

        [Fact]
        public async Task Update_UpdatedAccountGroup_Ok()
        {
            //Arrange
            var accountGroup = (await _serviceManager.AccountGroupService
                .GetAsync(_testAccountGroupValues.AccountGroupId1)).IfLeft(ag => default!);

            accountGroup!.Label = "Modified";
            accountGroup.Description = "Modified";

            //Act
            var result = (await _serviceManager.AccountGroupService.UpdateAsync(accountGroup)).IfLeft(r => default!);

            //Assert
            result.Should()
                    .NotBeNull()
                    .And.BeOfType<AccountGroup>()
                    .And.Match<AccountGroup>(a =>
                        a.Label == "Modified"
                        && a.Version != accountGroup.Version
                        && a.Id == accountGroup.Id);
        }

        [Fact]
        public async Task Update_AccountGroupNotFoundException_AccountGroupIdNotFound()
        {
            //Arrange
            var accountGroup = (await _serviceManager.AccountGroupService
                .GetAsync(_testAccountGroupValues.AccountGroupId1)).IfLeft(ag => default!);

            accountGroup.Id = Guid.NewGuid();
            accountGroup.Label = "Modified";
            accountGroup.Description = "Modified";

            //Act
            var result = (await _serviceManager.AccountGroupService.UpdateAsync(accountGroup)).IfRight(err => default!);

            //Assert
            result.Should()
                    .NotBeNull()
                    .And.BeOfType<ResourceNotFoundError>();
        }

        [Fact]
        public async Task Update_AccountGroupParentNotFoundException_AccountGroupParentIdNotFound()
        {
            //Arrange
            var accountGroup = (await _serviceManager.AccountGroupService
                .GetAsync(_testAccountGroupValues.AccountGroupId1)).IfLeft(ag => default!);

            accountGroup!.Label = "Modified";
            accountGroup.Description = "Modified";
            accountGroup.ParentAccountGroupId = Guid.NewGuid();


            //Act
            var result = (await _serviceManager.AccountGroupService.UpdateAsync(accountGroup)).IfRight(err => default!);

            //Assert
            result.Should()
                    .NotBeNull()
                    .And.BeOfType<AccountGroupParentNotFoundError>()
                    .And.Match<AccountGroupParentNotFoundError>(a =>
                        a.ErrorType == ServiceAndFeatureErrorType.BadParams);
        }

        [Fact]
        public async Task Update_AccountGroupClassificationNotFound_AccountGroupClassificationIdNotFound()
        {
            //Arrange
            var accountGroup = (await _serviceManager.AccountGroupService
                .GetAsync(_testAccountGroupValues.AccountGroupId1)).IfLeft(ag => default!);

            accountGroup!.Label = "Modified";
            accountGroup.Description = "Modified";
            accountGroup.ClassificationId = Guid.NewGuid();


            //Act
            var result = (await _serviceManager.AccountGroupService.UpdateAsync(accountGroup)).IfRight(err => default!);

            //Assert
            result.Should()
                    .NotBeNull()
                    .And.BeOfType<BadParamExternalResourceNotFound>();
        }


        [Fact]
        public async Task Update_AccountGroupAlreadyExistsException_AccountGroupCodeAlreadyExistsInClassification()
        {
            //Arrange
            var accountGroup = (await _serviceManager.AccountGroupService
                .GetAsync(_testAccountGroupValues.AccountGroupId1)).IfLeft(ag => default!);

            accountGroup!.Label = "Modified";
            accountGroup.Description = "Modified";
            accountGroup.Code = "10";


            //Act
            var result = (await _serviceManager.AccountGroupService.UpdateAsync(accountGroup)).IfRight(err => default!);

            //Assert
            result.Should()
                    .NotBeNull()
                    .And.BeOfType<ResourceAlreadyExistsError>();
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

        public static IEnumerable<object[]> GetAccountGroups(int numTests)
        {
            var accountGroups = FakeGenerator.GenerateAccountGroups(numTests);

            foreach (var accountGrp in accountGroups)
            {
                yield return new object[] { accountGrp };
            }
        }
    }
}
