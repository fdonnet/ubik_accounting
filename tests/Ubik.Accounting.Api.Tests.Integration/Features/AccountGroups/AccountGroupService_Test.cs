using FluentAssertions;
using Ubik.Accounting.Api.Data.Init;
using Ubik.Accounting.Api.Features;
using Ubik.Accounting.Api.Models;

namespace Ubik.Accounting.Api.Tests.Integration.Features.AccountGroups
{
    public class AccountGroupService_Test : BaseIntegrationTest
    {
        private readonly BaseValuesForAccountGroups _testAccountGroupValues;
        private readonly IServiceManager _serviceManager;

        public AccountGroupService_Test(IntegrationTestWebAppFactory factory) : base(factory)
        {
            _testAccountGroupValues = new BaseValuesForAccountGroups();
            _serviceManager = new ServiceManager(DbContext);
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
            var result = await _serviceManager.AccountGroupService.GetAsync(_testAccountGroupValues.AccountGroupId1);

            //Assert
            result.Should()
                    .NotBeNull()
                    .And.BeOfType<AccountGroup>();
        }

        [Fact]
        public async Task Get_AccountGroup_OkWithChildAccounts()
        {
            //Arrange

            //Act
            var result = await _serviceManager.AccountGroupService.GetWithChildAccountsAsync(_testAccountGroupValues.AccountGroupId1);

            //Assert
            result.Should()
                    .NotBeNull()
                    .And.BeOfType<AccountGroup>()
                    .And.Match<AccountGroup>(g=>g.Accounts != null);
        }


        [Theory, MemberData(nameof(GeneratedGuids))]
        public async Task Get_Null_IdNotExists(Guid id)
        {
            //Arrange

            //Act
            var result = await _serviceManager.AccountGroupService.GetAsync(id);

            //Assert
            result.Should()
            .BeNull();
        }

        [Theory]
        [MemberData(nameof(GetAccountGroups), parameters: new object[] { 5 })]
        public async Task Add_AccountGroup_Ok(AccountGroup accountGroup)
        {
            //Arrange

            //Act
            var result = await _serviceManager.AccountGroupService.AddAsync(accountGroup);

            //Assert
            result.Should()
                    .NotBeNull()
                    .And.BeOfType<AccountGroup>();
        }

        [Theory]
        [MemberData(nameof(GetAccountGroups), parameters: new object[] { 5 })]
        public async Task Add_AuditFieldsModified_Ok(AccountGroup accountGroup)
        {
            //Arrange

            //Act
            var result = await _serviceManager.AccountGroupService.AddAsync(accountGroup);

            //Assert
            result.Should().Match<AccountGroup>(x => x.ModifiedBy != null && x.ModifiedAt != null);
        }

        [Fact]
        public async Task Delete_True_Ok()
        {
            //Arrange

            //Act
            await _serviceManager.AccountGroupService.ExecuteDeleteAsync(_testAccountGroupValues.AccountGroupIdForDel);
            var exist = (await _serviceManager.AccountGroupService.GetAsync(_testAccountGroupValues.AccountGroupIdForDel)) != null;

            //Assert
            exist.Should()
                .BeFalse();
        }

        [Theory]
        [InlineData("102", "1524f188-20dd-4888-88f8-428e59bbc22a", true)]
        [InlineData("102", "7777f11f-20dd-4888-88f8-428e59bbc535", false)]
        public async Task IfExist_TrueOrFalse_Ok(string accountGroupCode
            , Guid accountGroupClassificationId, bool resultNeeded)
        {
            //Arrange

            //Act
            var result = await _serviceManager.AccountGroupService.IfExistsAsync(accountGroupCode, accountGroupClassificationId);

            //Assert
            result.Should().Be(resultNeeded);
        }

        [Theory]
        [InlineData("102", "1524f188-20dd-4888-88f8-428e59bbc22a","7777f11f-20dd-4888-88f8-428e59bbc535", true)]
        [InlineData("102", "7777f11f-20dd-4888-88f8-428e59bbc535", "7777f11f-20dd-4888-88f8-428e59bbc535", false)]
        public async Task IfExistWithDifferentId_TrueorFalse_Ok(string accountGroupCode, 
            Guid accountGroupClassificationId, Guid currentGuid, bool resultNeeded)

        {
            //Arrange

            //Act
            var result = await _serviceManager.AccountGroupService
                .IfExistsWithDifferentIdAsync(accountGroupCode, accountGroupClassificationId, currentGuid);

            //Assert
            result.Should().Be(resultNeeded);
        }

        [Fact]
        public async Task Update_ModifiedAtFieldUpdated_Ok()
        {
            //Arrange
            var accountGroup = await _serviceManager.AccountGroupService.GetAsync(_testAccountGroupValues.AccountGroupId1);

            accountGroup!.Label = "Modified";
            accountGroup.Description = "Modified";
            var modifiedAt = accountGroup.ModifiedAt;

            //Act
            var result = _serviceManager.AccountGroupService.Update(accountGroup);

            //Assert
            result.Should()
                    .NotBeNull()
                    .And.Match<AccountGroup>(x => x.ModifiedAt > modifiedAt);
        }

        [Fact]
        public async Task Update_UpdatedAccountGroup_Ok()
        {
            //Arrange
            var accountGroup = await _serviceManager.AccountGroupService.GetAsync(_testAccountGroupValues.AccountGroupId1);

            accountGroup!.Label = "Modified";
            accountGroup.Description = "Modified";

            //Act
            var result = _serviceManager.AccountGroupService.Update(accountGroup);

            //Assert
            result.Should()
                    .NotBeNull()
                    .And.BeOfType<AccountGroup>();
        }

        [Theory]
        [InlineData("1529991f-20dd-4888-88f8-428e59bbc22a", true)]
        [InlineData("1524f11f-20dd-4888-88f8-428e59bbc22a", false)]
        public async Task HasAnyChildAccountGroups_TrueOrFalse_Ok(Guid id,bool neededResult)
        {
            //Act
            var result = await _serviceManager.AccountGroupService.HasAnyChildAccountGroups(id);

            //Assert
            result.Should().Be(neededResult);
        }

        [Theory]
        [InlineData("1524f11f-20dd-4888-88f8-428e59bbc22a", true)]
        [InlineData("1529991f-20dd-4888-88f8-428e59bbc22a", false)]
        public async Task HasAnyChildAccounts_TrueOrFalse_OK(Guid id, bool neededResult)
        {
            //Act
            var result = await _serviceManager.AccountGroupService.HasAnyChildAccounts(id);

            //Assert
            result.Should().Be(neededResult);
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
