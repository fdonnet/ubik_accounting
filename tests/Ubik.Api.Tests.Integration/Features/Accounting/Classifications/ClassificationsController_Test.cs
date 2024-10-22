using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Ubik.Accounting.Contracts.Accounts.Results;
using Ubik.Accounting.Contracts.Classifications.Results;
using Ubik.ApiService.Common.Exceptions;
using Ubik.Accounting.Contracts.Classifications.Commands;
using MassTransit;

namespace Ubik.Api.Tests.Integration.Features.Accounting.Classifications
{
    public class ClassificationsController_Test : BaseIntegrationTestAccounting
    {
        private readonly string _baseUrlForV1;
        private readonly HttpClient _client;
        private readonly static Guid _id = new("cc100000-5dd4-0015-d910-08dcd9665e79");
        private readonly static Guid _idToDel = new("1524f190-20dd-4888-88f8-428e59bbc22c");

        public ClassificationsController_Test(IntegrationTestProxyFactory factory) : base(factory)
        {
            _baseUrlForV1 = "/accounting/api/v1/classifications";
            _client = Factory.CreateDefaultClient();
        }

        [Fact]
        public async Task Get_Classifications_All_WithRW_OK()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.RW);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.GetAsync(_baseUrlForV1);
            var result = await response.Content.ReadFromJsonAsync<List<ClassificationStandardResult>>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Should()
                .NotBeNull()
                .And.BeOfType<List<ClassificationStandardResult>>();
        }

        [Fact]
        public async Task Get_Classifications_All_WithRO_OK()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.RO);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.GetAsync(_baseUrlForV1);
            var result = await response.Content.ReadFromJsonAsync<List<ClassificationStandardResult>>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Should()
                .NotBeNull()
                .And.BeOfType<List<ClassificationStandardResult>>();
        }

        [Fact]
        public async Task Get_Classifications_All_WithNoAuth_401()
        {
            //Act
            var response = await _client.GetAsync(_baseUrlForV1);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Get_All_Classifications_WithNoRole_403()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.NoRole);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.GetAsync(_baseUrlForV1);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Get_All_Classifications_WithAdmin_403()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.MegaAdmin);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.GetAsync(_baseUrlForV1);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Get_All_Classifications_WithOtherTenant_OK()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.OtherTenant);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.GetAsync(_baseUrlForV1);
            var result = await response.Content.ReadFromJsonAsync<List<ClassificationStandardResult>>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Should()
                .NotBeNull()
                .And.BeOfType<List<ClassificationStandardResult>>()
                .And.HaveCount(1);
        }

        [Fact]
        public async Task Get_Classification_By_Id_WithRW_OK()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.RW);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.GetAsync($"{_baseUrlForV1}/{_id}");
            var result = await response.Content.ReadFromJsonAsync<ClassificationStandardResult>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Should()
                .NotBeNull()
                .And.BeOfType<ClassificationStandardResult>();
        }

        [Fact]
        public async Task Get_Classification_By_Id_WithRO_OK()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.RO);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.GetAsync($"{_baseUrlForV1}/{_id}");
            var result = await response.Content.ReadFromJsonAsync<ClassificationStandardResult>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Should()
                .NotBeNull()
                .And.BeOfType<ClassificationStandardResult>();
        }

        [Fact]
        public async Task Get_Classification_By_Id_WithNoAuth_401()
        {
            //Act
            var response = await _client.GetAsync($"{_baseUrlForV1}/{_id}");

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Get_Classification_By_Id_WithNoRole_403()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.NoRole);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.GetAsync($"{_baseUrlForV1}/{_id}");

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Get_Classification_By_Id_WithAdmin_403()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.MegaAdmin);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.GetAsync($"{_baseUrlForV1}/{_id}");

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Get_Classification_By_Id_WithOtherTenant_404()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.OtherTenant);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.GetAsync($"{_baseUrlForV1}/{_id}");
            var result = await response.Content.ReadFromJsonAsync<CustomProblemDetails>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            result.Should()
                .NotBeNull()
                .And.BeOfType<CustomProblemDetails>()
                .And.Match<CustomProblemDetails>(x => x.Errors.First().Code == "CLASSIFICATION_NOT_FOUND");
        }

        [Fact]
        public async Task Get_Classification_By_Id_WithBadId_404()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.RW);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.GetAsync($"{_baseUrlForV1}/{Guid.NewGuid()}");
            var result = await response.Content.ReadFromJsonAsync<CustomProblemDetails>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            result.Should()
                .NotBeNull()
                .And.BeOfType<CustomProblemDetails>()
                .And.Match<CustomProblemDetails>(x => x.Errors.First().Code == "CLASSIFICATION_NOT_FOUND");
        }

        [Fact]
        public async Task Get_Classification_AttachedAccount_WithRW_OK()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.RW);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.GetAsync($"{_baseUrlForV1}/{_id}/accounts");
            var result = await response.Content.ReadFromJsonAsync<List<AccountStandardResult>>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Should()
                .NotBeNull()
                .And.BeOfType<List<AccountStandardResult>>();
        }

        [Fact]
        public async Task Get_Classification_AttachedAccount_WithRO_OK()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.RO);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.GetAsync($"{_baseUrlForV1}/{_id}/accounts");
            var result = await response.Content.ReadFromJsonAsync<List<AccountStandardResult>>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Should()
                .NotBeNull()
                .And.BeOfType<List<AccountStandardResult>>();
        }

        [Fact]
        public async Task Get_Classification_AttachedAccount_WithNoAuth_401()
        {
            //Act
            var response = await _client.GetAsync($"{_baseUrlForV1}/{_id}/accounts");

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Get_Classification_AttachedAccount_WithNoRole_403()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.NoRole);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.GetAsync($"{_baseUrlForV1}/{_id}/accounts");

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Get_Classification_AttachedAccount_WithAdmin_403()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.MegaAdmin);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.GetAsync($"{_baseUrlForV1}/{_id}/accounts");

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Get_Classification_AttachedAccount_WithOtherTenant_404()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.OtherTenant);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.GetAsync($"{_baseUrlForV1}/{_id}/accounts");
            var result = await response.Content.ReadFromJsonAsync<CustomProblemDetails>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            result.Should()
                .NotBeNull()
                .And.BeOfType<CustomProblemDetails>()
                .And.Match<CustomProblemDetails>(x => x.Errors.First().Code == "CLASSIFICATION_NOT_FOUND");
        }

        [Fact]
        public async Task Get_Classification_AttachedAccount_WithBadId_404()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.RW);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.GetAsync($"{_baseUrlForV1}/{Guid.NewGuid()}/accounts");
            var result = await response.Content.ReadFromJsonAsync<CustomProblemDetails>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            result.Should()
                .NotBeNull()
                .And.BeOfType<CustomProblemDetails>()
                .And.Match<CustomProblemDetails>(x => x.Errors.First().Code == "CLASSIFICATION_NOT_FOUND");
        }

        [Fact]
        public async Task Get_Classification_MissingAccounts_WithRW_OK()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.RW);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.GetAsync($"{_baseUrlForV1}/{_id}/missingaccounts");
            var result = await response.Content.ReadFromJsonAsync<List<AccountStandardResult>>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Should()
                .NotBeNull()
                .And.BeOfType<List<AccountStandardResult>>();
        }

        [Fact]
        public async Task Get_Classification_MissingAccounts_WithRO_OK()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.RO);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.GetAsync($"{_baseUrlForV1}/{_id}/missingaccounts");
            var result = await response.Content.ReadFromJsonAsync<List<AccountStandardResult>>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Should()
                .NotBeNull()
                .And.BeOfType<List<AccountStandardResult>>();
        }

        [Fact]
        public async Task Get_Classification_MissingAccounts_WithNoAuth_401()
        {
            //Act
            var response = await _client.GetAsync($"{_baseUrlForV1}/{_id}/missingaccounts");

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Get_Classification_MissingAccounts_WithNoRole_403()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.NoRole);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.GetAsync($"{_baseUrlForV1}/{_id}/missingaccounts");

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Get_Classification_MissingAccounts_WithAdmin_403()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.MegaAdmin);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.GetAsync($"{_baseUrlForV1}/{_id}/missingaccounts");

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Get_Classification_MissingAccounts_WithOtherTenant_404()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.OtherTenant);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.GetAsync($"{_baseUrlForV1}/{_id}/missingaccounts");
            var result = await response.Content.ReadFromJsonAsync<CustomProblemDetails>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            result.Should()
                .NotBeNull()
                .And.BeOfType<CustomProblemDetails>()
                .And.Match<CustomProblemDetails>(x => x.Errors.First().Code == "CLASSIFICATION_NOT_FOUND");
        }

        [Fact]
        public async Task Get_Classification_MissingAccounts_WithBadId_404()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.RW);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.GetAsync($"{_baseUrlForV1}/{Guid.NewGuid()}/missingaccounts");
            var result = await response.Content.ReadFromJsonAsync<CustomProblemDetails>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            result.Should()
                .NotBeNull()
                .And.BeOfType<CustomProblemDetails>()
                .And.Match<CustomProblemDetails>(x => x.Errors.First().Code == "CLASSIFICATION_NOT_FOUND");
        }

        [Fact]
        public async Task Get_Classification_Status_WithRW_OK()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.RW);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.GetAsync($"{_baseUrlForV1}/{_id}/status");
            var result = await response.Content.ReadFromJsonAsync<ClassificationStatusResult>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Should()
                .NotBeNull()
                .And.BeOfType<ClassificationStatusResult>()
                .And.Match<ClassificationStatusResult>(x => !x.IsReady);
        }

        [Fact]
        public async Task Get_Classification_Status_WithRO_OK()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.RO);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.GetAsync($"{_baseUrlForV1}/{_id}/status");
            var result = await response.Content.ReadFromJsonAsync<ClassificationStatusResult>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Should()
                .NotBeNull()
                .And.BeOfType<ClassificationStatusResult>()
                .And.Match<ClassificationStatusResult>(x => !x.IsReady);
        }

        [Fact]
        public async Task Get_Classification_Status_WithNoAuth_401()
        {
            //Act
            var response = await _client.GetAsync($"{_baseUrlForV1}/{_id}/status");

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Get_Classification_Status_WithNoRole_403()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.NoRole);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.GetAsync($"{_baseUrlForV1}/{_id}/status");

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Get_Classification_Status_WithAdmin_403()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.MegaAdmin);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.GetAsync($"{_baseUrlForV1}/{_id}/status");

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Get_Classification_Status_WithOtherTenant_404()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.OtherTenant);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.GetAsync($"{_baseUrlForV1}/{_id}/status");
            var result = await response.Content.ReadFromJsonAsync<CustomProblemDetails>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            result.Should()
                .NotBeNull()
                .And.BeOfType<CustomProblemDetails>()
                .And.Match<CustomProblemDetails>(x => x.Errors.First().Code == "CLASSIFICATION_NOT_FOUND");
        }

        [Fact]
        public async Task Get_Classification_Status_WithBadId_404()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.RW);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.GetAsync($"{_baseUrlForV1}/{Guid.NewGuid()}/status");
            var result = await response.Content.ReadFromJsonAsync<CustomProblemDetails>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            result.Should()
                .NotBeNull()
                .And.BeOfType<CustomProblemDetails>()
                .And.Match<CustomProblemDetails>(x => x.Errors.First().Code == "CLASSIFICATION_NOT_FOUND");
        }

        [Fact]
        public async Task Add_Classification_WithRW_OK()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.RW);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var classification = new AddClassificationCommand
            {
                Code = "TestCode2",
                Description = "TestDescription",
                Label = "TestLabel",
            };

            //Act
            var response = await _client.PostAsJsonAsync(_baseUrlForV1, classification);
            var result = await response.Content.ReadFromJsonAsync<ClassificationStandardResult>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            result.Should()
                .NotBeNull()
                .And.BeOfType<ClassificationStandardResult>()
                .And.Match<ClassificationStandardResult>(x => x.Code == classification.Code);
        }

        [Fact]
        public async Task Add_Classification_WithRO_403()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.RO);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var classification = new AddClassificationCommand
            {
                Code = "TestCode",
                Description = "TestDescription",
                Label = "TestLabel",
            };

            //Act
            var response = await _client.PostAsJsonAsync(_baseUrlForV1, classification);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Add_Classification_WithNoAuth_401()
        {
            //Arrange
            var classification = new AddClassificationCommand
            {
                Code = "TestCode",
                Description = "TestDescription",
                Label = "TestLabel",
            };

            //Act
            var response = await _client.PostAsJsonAsync(_baseUrlForV1, classification);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Add_Classification_WithAdmin_403()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.MegaAdmin);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var classification = new AddClassificationCommand
            {
                Code = "TestCode",
                Description = "TestDescription",
                Label = "TestLabel",
            };

            //Act
            var response = await _client.PostAsJsonAsync(_baseUrlForV1, classification);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Add_Classification_WithOtherTenant_OK()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.OtherTenant);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var classification = new AddClassificationCommand
            {
                Code = "TestCode",
                Description = "TestDescription",
                Label = "TestLabel",
            };

            //Act
            var response = await _client.PostAsJsonAsync(_baseUrlForV1, classification);
            var result = await response.Content.ReadFromJsonAsync<ClassificationStandardResult>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            result.Should()
                .NotBeNull()
                .And.BeOfType<ClassificationStandardResult>()
                .And.Match<ClassificationStandardResult>(x => x.Code == classification.Code);
        }

        [Fact]
        public async Task Add_Classification_WithNoRole_403()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.NoRole);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var classification = new AddClassificationCommand
            {
                Code = "TestCode",
                Description = "TestDescription",
                Label = "TestLabel",
            };

            //Act
            var response = await _client.PostAsJsonAsync(_baseUrlForV1, classification);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Update_Classification_WithRW_OK()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.RW);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var classification = new UpdateClassificationCommand
            {
                Id = new Guid("1524f189-20dd-4888-88f8-428e59bbcddd"),
                Code = "TestCode",
                Description = "TestDescription",
                Label = "TestLabel",
                Version = new Guid("1524f189-20dd-4888-88f8-428e59bbcddd")
            };

            //Act
            var response = await _client.PutAsJsonAsync($"{_baseUrlForV1}/1524f189-20dd-4888-88f8-428e59bbcddd", classification);
            var result = await response.Content.ReadFromJsonAsync<ClassificationStandardResult>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Should()
                .NotBeNull()
                .And.BeOfType<ClassificationStandardResult>()
                .And.Match<ClassificationStandardResult>(x => x.Code == classification.Code);
        }

        [Fact]
        public async Task Update_Classification_WithBadVersion_OK()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.RW);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var classification = new UpdateClassificationCommand
            {
                Id = new Guid("1524f189-20dd-4888-88f8-428e59bbcddd"),
                Code = "TestCode",
                Description = "TestDescription",
                Label = "TestLabel",
                Version = new Guid("1514f189-20dd-4888-88f8-428e59bbcddd")
            };

            //Act
            var response = await _client.PutAsJsonAsync($"{_baseUrlForV1}/1524f189-20dd-4888-88f8-428e59bbcddd", classification);
            var result = await response.Content.ReadFromJsonAsync<CustomProblemDetails>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Conflict);
            result.Should()
                .NotBeNull()
                .And.BeOfType<CustomProblemDetails>()
                .And.Match<CustomProblemDetails>(CustomProblemDetails =>
                    CustomProblemDetails.Errors.First().Code == "CLASSIFICATION_UPDATE_CONCURRENCY");
        }

        [Fact]
        public async Task Update_Classification_WithRO_403()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.RO);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var classification = new UpdateClassificationCommand
            {
                Id = new Guid("1524f189-20dd-4888-88f8-428e59bbcddd"),
                Code = "TestCode",
                Description = "TestDescription",
                Label = "TestLabel",
                Version = new Guid("1524f189-20dd-4888-88f8-428e59bbcddd")
            };

            //Act
            var response = await _client.PutAsJsonAsync($"{_baseUrlForV1}/1524f189-20dd-4888-88f8-428e59bbcddd", classification);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Update_Classification_WithNoAuth_401()
        {
            //Arrange
            var classification = new UpdateClassificationCommand
            {
                Id = new Guid("1524f189-20dd-4888-88f8-428e59bbcddd"),
                Code = "TestCode",
                Description = "TestDescription",
                Label = "TestLabel",
                Version = new Guid("1524f189-20dd-4888-88f8-428e59bbcddd")
            };

            //Act
            var response = await _client.PutAsJsonAsync($"{_baseUrlForV1}/1524f189-20dd-4888-88f8-428e59bbcddd", classification);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Update_Classification_WithAdmin_403()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.MegaAdmin);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var classification = new UpdateClassificationCommand
            {
                Id = new Guid("1524f189-20dd-4888-88f8-428e59bbcddd"),
                Code = "TestCode",
                Description = "TestDescription",
                Label = "TestLabel",
                Version = new Guid("1524f189-20dd-4888-88f8-428e59bbcddd")
            };

            //Act
            var response = await _client.PutAsJsonAsync($"{_baseUrlForV1}/1524f189-20dd-4888-88f8-428e59bbcddd", classification);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Update_Classification_WithOtherTenant_404()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.OtherTenant);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var classification = new UpdateClassificationCommand
            {
                Id = new Guid("1524f189-20dd-4888-88f8-428e59bbcddd"),
                Code = "TestCode",
                Description = "TestDescription",
                Label = "TestLabel",
                Version = new Guid("1524f189-20dd-4888-88f8-428e59bbcddd")
            };

            //Act
            var response = await _client.PutAsJsonAsync($"{_baseUrlForV1}/1524f189-20dd-4888-88f8-428e59bbcddd", classification);
            var result = await response.Content.ReadFromJsonAsync<CustomProblemDetails>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            result.Should()
                .NotBeNull()
                .And.BeOfType<CustomProblemDetails>()
                .And.Match<CustomProblemDetails>(x => x.Errors.First().Code == "CLASSIFICATION_NOT_FOUND");
        }

        [Fact]
        public async Task Update_Classification_WithBadId_404()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.RW);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var id = Guid.NewGuid();
            var classification = new UpdateClassificationCommand
            {
                Id = id,
                Code = "TestCode",
                Description = "TestDescription",
                Label = "TestLabel",
                Version = new Guid("1524f189-20dd-4888-88f8-428e59bbcddd")
            };

            //Act
            var response = await _client.PutAsJsonAsync($"{_baseUrlForV1}/{id}", classification);
            var result = await response.Content.ReadFromJsonAsync<CustomProblemDetails>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            result.Should()
                .NotBeNull()
                .And.BeOfType<CustomProblemDetails>()
                .And.Match<CustomProblemDetails>(x => x.Errors.First().Code == "CLASSIFICATION_NOT_FOUND");
        }

        [Fact]
        public async Task Update_Classification_WithNoRole_403()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.NoRole);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var classification = new UpdateClassificationCommand
            {
                Id = new Guid("1524f189-20dd-4888-88f8-428e59bbcddd"),
                Code = "TestCode",
                Description = "TestDescription",
                Label = "TestLabel",
                Version = new Guid("1524f189-20dd-4888-88f8-428e59bbcddd")
            };

            //Act
            var response = await _client.PutAsJsonAsync($"{_baseUrlForV1}/1524f189-20dd-4888-88f8-428e59bbcddd", classification);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Update_Classification_WithNotMatchId_400()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.RW);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var classification = new UpdateClassificationCommand
            {
                Id = new Guid("1524f189-20dd-4888-88f8-428e59bbcddd"),
                Code = "TestCode",
                Description = "TestDescription",
                Label = "TestLabel",
                Version = new Guid("1524f189-20dd-4888-88f8-428e59bbcddd")
            };

            //Act
            var response = await _client.PutAsJsonAsync($"{_baseUrlForV1}/{NewId.NextGuid()}", classification);
            var result = await response.Content.ReadFromJsonAsync<CustomProblemDetails>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            result.Should()
                .NotBeNull()
                .And.BeOfType<CustomProblemDetails>()
                .And.Match<CustomProblemDetails>(x => x.Errors.First().Code == "CLASSIFICATION_UPDATE_IDS_NOT_MATCH");
        }

        [Fact]
        public async Task Update_Classification_WithAlreadyExists_409()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.RW);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var classification = new UpdateClassificationCommand
            {
                Id = new Guid("4c470000-5dd4-0015-f70f-08dcdb1e6d00"),
                Code = "SWISSPLAN-FULL",
                Description = "TestDescription",
                Label = "TestLabel",
                Version = new Guid("1524f189-20dd-4888-88f8-428e59bbcddd")
            };

            //Act
            var response = await _client.PutAsJsonAsync($"{_baseUrlForV1}/4c470000-5dd4-0015-f70f-08dcdb1e6d00", classification);
            var result = await response.Content.ReadFromJsonAsync<CustomProblemDetails>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Conflict);
            result.Should()
                .NotBeNull()
                .And.BeOfType<CustomProblemDetails>()
                .And.Match<CustomProblemDetails>(x => x.Errors.First().Code == "CLASSIFICATION_ALREADY_EXISTS");
        }


    }
}
