using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ubik.Accounting.Api.Data.Init;
using Ubik.Accounting.Api.Features;
using Ubik.Accounting.Api.Features.AccountGroups.Exceptions;
using Ubik.Accounting.Api.Features.Classifications.Exceptions;
using Ubik.Accounting.Api.Models;
using Ubik.ApiService.Common.Exceptions;

namespace Ubik.Accounting.Api.Tests.Integration.Features.Classifications
{
    public  class ClassificationService_Test : BaseIntegrationTest
    {
        private readonly BaseValuesForClassifications _testClassifications;
        private readonly IServiceManager _serviceManager;

        public ClassificationService_Test(IntegrationTestWebAppFactory factory) : base(factory)
        {
            _testClassifications = new BaseValuesForClassifications();
            _serviceManager = new ServiceManager(DbContext);
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
            var result = (await _serviceManager.ClassificationService.GetAsync(_testClassifications.AccountGroupClassificationId1)).Result;

            //Assert
            result.Should()
                    .NotBeNull()
                    .And.BeOfType<Classification>();
        }

        [Fact]
        public async Task Get_ClassificationNotFoundException_IdNotExists()
        {
            //Arrange

            //Act
            var result = (await _serviceManager.ClassificationService.GetAsync(Guid.NewGuid())).Exception;

            //Assert
            result.Should()
           .NotBeNull()
           .And.BeOfType<ClassificationNotFoundException>()
           .And.Match<ClassificationNotFoundException>(a =>
               a.ErrorType == ServiceAndFeatureExceptionType.NotFound);
        }
    }
}
