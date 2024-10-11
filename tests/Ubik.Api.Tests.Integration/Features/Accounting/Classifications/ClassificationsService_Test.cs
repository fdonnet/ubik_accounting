using FluentAssertions;
using Ubik.Accounting.Api.Data.Init;
using Ubik.Accounting.Api.Features;
using Ubik.Accounting.Api.Features.Classifications.Queries.CustomPoco;
using Ubik.Accounting.Api.Models;
using Ubik.Api.Tests.Integration.Fake;
using Ubik.ApiService.Common.Errors;

namespace Ubik.Api.Tests.Integration.Features.Accounting.Classifications
{
    public class ClassificationsService_Test : BaseIntegrationTest
    {
        private readonly BaseValuesForClassifications _testClassifications;
        private readonly IServiceManager _serviceManager;

        public ClassificationsService_Test(IntegrationTestAccoutingFactory factory) : base(factory)
        {
            _testClassifications = new BaseValuesForClassifications();
            _serviceManager = new ServiceManager(DbContext, new FakeUserService());
        }

        [Fact]
        public async Task GetAll_Classifications_Ok()
        {
            //Arrange

            //Act
            var result = await _serviceManager.ClassificationService.GetAllAsync();

            //Assert
            result.Should()
                    .NotBeNull()
                    .And.AllBeOfType<Classification>();
        }

        [Fact]
        public async Task Get_Classification_Ok()
        {
            //Arrange

            //Act
            var result = (await _serviceManager.ClassificationService.GetAsync(_testClassifications.ClassificationId1)).IfLeft(x => default!);

            //Assert
            result.Should()
                    .NotBeNull()
                    .And.BeOfType<Classification>()
                    .And.Match<Classification>(c => c.Id == _testClassifications.ClassificationId1);
        }

        [Fact]
        public async Task Get_ClassificationNotFoundException_IdNotExists()
        {
            //Arrange

            //Act
            var response = await _serviceManager.ClassificationService.GetAsync(Guid.NewGuid());
            var forUpd = await _serviceManager.ClassificationService.UpdateAsync(
                new Classification { Id = Guid.NewGuid(), Code = "test", Label = "test" });

            var forDel = await _serviceManager.ClassificationService.DeleteAsync(Guid.NewGuid());

            var result = response.IfRight(x => default!);
            var resultForUpd = forUpd.IfRight(x => default!);
            var resultForDel = forDel.IfRight(x => default!);

            //Assert
            result.Should()
           .NotBeNull()
           .And.BeOfType<ResourceNotFoundError>();

            resultForUpd.Should()
            .NotBeNull()
            .And.BeOfType<ResourceNotFoundError>();

            resultForDel.Should()
            .NotBeNull()
            .And.BeOfType<ResourceNotFoundError>();
        }

        [Fact]
        public async Task GetAccounts_Accounts_Ok()
        {
            //Arrange

            //Act
            var result = (await _serviceManager.ClassificationService
                .GetClassificationAccountsAsync(_testClassifications.ClassificationId1)).IfLeft(x => default!);

            //Assert
            result.Should()
                    .NotBeNull()
                    .And.NotBeEmpty()
                    .And.AllBeOfType<Account>();
        }

        [Fact]
        public async Task GetAccountsMissing_Accounts_Ok()
        {
            //Arrange

            //Act
            var result = (await _serviceManager.ClassificationService
                .GetClassificationAccountsMissingAsync(_testClassifications.ClassificationId1)).IfLeft(err => default!);

            //Assert
            result.Should()
                    .NotBeNull()
                    .And.NotBeEmpty()
                    .And.AllBeOfType<Account>();
        }

        [Fact]
        public async Task GetStatus_ClassificationStatus_Ok()
        {
            //Arrange

            //Act
            var result = (await _serviceManager.ClassificationService
                .GetClassificationStatusAsync(_testClassifications.ClassificationId1)).IfLeft(x => default!);

            //Assert
            result.Should()
                    .NotBeNull()
                    .And.BeOfType<ClassificationStatus>();
        }

        [Fact]
        public async Task Add_Classification_Ok()
        {
            //Arrange
            var classification = new Classification
            {
                Code = "Test",
                Label = "Test",
            };

            //Act
            var result = (await _serviceManager.ClassificationService.AddAsync(classification)).IfLeft(err => default!);

            //Assert
            result.Should()
                    .NotBeNull()
                    .And.BeOfType<Classification>()
                    .And.Match<Classification>(c => c.Code == classification.Code);
        }

        [Fact]
        public async Task Add_ClassificationAlreadyExistsException_ClassificationCodeAlreadyExists()
        {
            //Arrange
            var classification = new Classification
            {
                Code = "SWISSPLAN-FULL",
                Label = "Test"
            };

            //Act
            var result = (await _serviceManager.ClassificationService.AddAsync(classification)).IfRight(a => default!);

            //Assert
            result.Should()
                    .NotBeNull()
                    .And.BeOfType<ResourceAlreadyExistsError>();
        }

        [Fact]
        public async Task Update_UpdatedClassification_Ok()
        {
            //Arrange
            var classification = (await _serviceManager.ClassificationService
                .GetAsync(_testClassifications.ClassificationId2)).IfLeft(c => default!);

            classification!.Label = "Modified";
            classification.Description = "Modified";

            //Act
            var result = (await _serviceManager.ClassificationService.UpdateAsync(classification)).IfLeft(r => default!);

            //Assert
            result.Should()
                    .NotBeNull()
                    .And.BeOfType<Classification>()
                    .And.Match<Classification>(x =>
                        x.Label == "Modified"
                        && x.Version != classification.Version
                        && x.Id == classification.Id);
        }

        [Fact]
        public async Task Update_ClassificationAlreadyExistsException_ClassificationCodeAlreadyExists()
        {
            //Arrange
            var classification = (await _serviceManager.ClassificationService
                .GetAsync(_testClassifications.ClassificationId2)).IfLeft(c => default!);

            classification!.Label = "Modified";
            classification.Description = "Modified";
            classification.Code = "SWISSPLAN-FULL";

            //Act
            var result = (await _serviceManager.ClassificationService.UpdateAsync(classification)).IfRight(r => default!);

            //Assert
            result.Should()
                    .NotBeNull()
                    .And.BeOfType<ResourceAlreadyExistsError>();
        }

        [Fact]
        public async Task Delete_True_Ok()
        {
            //Arrange

            //Act
            await _serviceManager.AccountGroupService.DeleteAsync(_testClassifications.ClassificationIdForDel);
            var exist = (await _serviceManager.AccountGroupService.GetAsync(_testClassifications.ClassificationIdForDel)).IsRight;

            //Assert
            exist.Should()
                .BeFalse();
        }
    }
}
