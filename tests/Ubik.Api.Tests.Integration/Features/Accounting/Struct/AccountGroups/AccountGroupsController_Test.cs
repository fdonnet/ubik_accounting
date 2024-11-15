using FluentAssertions;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net;
using Ubik.Accounting.Structure.Contracts.AccountGroups.Results;
using Ubik.ApiService.Common.Exceptions;
using Ubik.Accounting.Structure.Contracts.Accounts.Results;
using Ubik.Accounting.Structure.Contracts.AccountGroups.Commands;
using MassTransit;

namespace Ubik.Api.Tests.Integration.Features.Accounting.Struct.AccountGroups
{
    public class AccountGroupsController_Test : BaseIntegrationTestAccountingStruct
    {
        private readonly string _baseUrlForV1;
        private readonly HttpClient _client;
        private readonly static Guid _accountGroupId = new("ec860000-5dd4-0015-93df-08dcda2056e2");
        private readonly static Guid _accountGroupToDel = new("34980000-5dd4-0015-811f-08dcdb084021");

        public AccountGroupsController_Test(IntegrationTestProxyFactory factory) : base(factory)
        {
            _baseUrlForV1 = "/accounting/api/v1/accountgroups";
            _client = Factory.CreateDefaultClient();
        }

        [Fact]
        public async Task Get_AccountGroups_All_WithRW_OK()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.RW);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.GetAsync(_baseUrlForV1);
            var result = await response.Content.ReadFromJsonAsync<List<AccountGroupStandardResult>>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Should()
                .NotBeNull()
                .And.BeOfType<List<AccountGroupStandardResult>>();
        }

        [Fact]
        public async Task Get_AccountGroups_All_WithOtherTenant_OK()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.OtherTenant);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.GetAsync(_baseUrlForV1);
            var result = await response.Content.ReadFromJsonAsync<List<AccountGroupStandardResult>>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Should()
                .NotBeNull()
                .And.BeOfType<List<AccountGroupStandardResult>>()
                .And.Match<List<AccountGroupStandardResult>>(x => x.Count == 0);
        }

        [Fact]
        public async Task Get_AccountGroups_All_WithRO_OK()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.RO);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.GetAsync(_baseUrlForV1);
            var result = await response.Content.ReadFromJsonAsync<List<AccountGroupStandardResult>>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Should()
                .NotBeNull()
                .And.BeOfType<List<AccountGroupStandardResult>>();
        }

        [Fact]
        public async Task Get_AccountGroups_All_WithNoAuth_401()
        {
            //Act
            var response = await _client.GetAsync(_baseUrlForV1);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Get_AccountGroups_All_WithNoRole_403()
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
        public async Task Get_AccountGroups_All_WithAdmin_403()
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
        public async Task Get_AccountGroup_By_Id_WithRW_OK()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.RW);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.GetAsync($"{_baseUrlForV1}/{_accountGroupId}");
            var result = await response.Content.ReadFromJsonAsync<AccountGroupStandardResult>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Should()
                .NotBeNull()
                .And.BeOfType<AccountGroupStandardResult>()
                .And.Match<AccountGroupStandardResult>(x => x.Id == _accountGroupId);
        }

        [Fact]
        public async Task Get_AccountGroup_By_Id_WithRO_OK()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.RO);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.GetAsync($"{_baseUrlForV1}/{_accountGroupId}");
            var result = await response.Content.ReadFromJsonAsync<AccountGroupStandardResult>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Should()
                .NotBeNull()
                .And.BeOfType<AccountGroupStandardResult>()
                .And.Match<AccountGroupStandardResult>(x => x.Id == _accountGroupId);
        }

        [Fact]
        public async Task Get_AccountGroup_By_Id_WithAdmin_403()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.MegaAdmin);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.GetAsync($"{_baseUrlForV1}/{_accountGroupId}");


            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Get_AccountGroup_By_Id_WithNoAuth_401()
        {
            //Act
            var response = await _client.GetAsync($"{_baseUrlForV1}/{_accountGroupId}");

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Get_AccountGroup_By_Id_WithNoRole_403()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.NoRole);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.GetAsync($"{_baseUrlForV1}/{_accountGroupId}");

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Get_AccountGroup_By_Id_WithOtherTenantUser_404()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.OtherTenant);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.GetAsync($"{_baseUrlForV1}/{_accountGroupId}");
            var result = await response.Content.ReadFromJsonAsync<CustomProblemDetails>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            result.Should()
                .NotBeNull()
                .And.BeOfType<CustomProblemDetails>()
                .And.Match<CustomProblemDetails>(x => x.Errors.First().Code == "ACCOUNTGROUP_NOT_FOUND");
        }

        [Fact]
        public async Task Get_AccountGroup_By_Id_WithBadId_404()
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
                .And.Match<CustomProblemDetails>(x => x.Errors.First().Code == "ACCOUNTGROUP_NOT_FOUND");
        }

        [Fact]
        public async Task Get_AccountGroup_ChildAccounts_By_Id_WithRW_OK()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.RW);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.GetAsync($"{_baseUrlForV1}/{_accountGroupId}/accounts");
            var result = await response.Content.ReadFromJsonAsync<List<AccountStandardResult>>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Should()
                .NotBeNull()
                .And.BeOfType<List<AccountStandardResult>>();
        }

        [Fact]
        public async Task Get_AccountGroup_ChildAccounts_By_Id_WithRO_OK()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.RO);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.GetAsync($"{_baseUrlForV1}/{_accountGroupId}/accounts");
            var result = await response.Content.ReadFromJsonAsync<List<AccountStandardResult>>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Should()
                .NotBeNull()
                .And.BeOfType<List<AccountStandardResult>>();
        }

        [Fact]
        public async Task Get_AccountGroup_ChildAccounts_By_Id_WithAdmin_403()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.MegaAdmin);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.GetAsync($"{_baseUrlForV1}/{_accountGroupId}/accounts");

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Get_AccountGroup_ChildAccounts_By_Id_WithNoAuth_401()
        {
            //Act
            var response = await _client.GetAsync($"{_baseUrlForV1}/{_accountGroupId}/accounts");

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Get_AccountGroup_ChildAccounts_By_Id_WithNoRole_403()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.NoRole);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.GetAsync($"{_baseUrlForV1}/{_accountGroupId}/accounts");

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Get_AccountGroup_ChildAccounts_By_Id_WithOtherTenantUser_404()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.OtherTenant);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.GetAsync($"{_baseUrlForV1}/{_accountGroupId}/accounts");
            var result = await response.Content.ReadFromJsonAsync<CustomProblemDetails>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            result.Should()
                .NotBeNull()
                .And.BeOfType<CustomProblemDetails>()
                .And.Match<CustomProblemDetails>(x => x.Errors.First().Code == "ACCOUNTGROUP_NOT_FOUND");
        }

        [Fact]
        public async Task Get_AccountGroup_ChildAccounts_By_Id_WithBadId_404()
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
                .And.Match<CustomProblemDetails>(x => x.Errors.First().Code == "ACCOUNTGROUP_NOT_FOUND");
        }

        [Fact]
        public async Task Add_AccountGroup_WithRW_Created()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.RW);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var command = new AddAccountGroupCommand
            {
                Description = "Test",
                Code = "Test",
                Label = "Test",
                AccountGroupClassificationId = new Guid("cc100000-5dd4-0015-d910-08dcd9665e79"),
            };

            //Act
            var response = await _client.PostAsJsonAsync(_baseUrlForV1, command);
            var result = await response.Content.ReadFromJsonAsync<AccountGroupStandardResult>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            result.Should()
                .NotBeNull()
                .And.BeOfType<AccountGroupStandardResult>()
                .And.Match<AccountGroupStandardResult>(x => x.Code == command.Code);
        }

        [Fact]
        public async Task Add_AccountGroupWithParent_WithRW_Created()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.RW);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var command = new AddAccountGroupCommand
            {
                Description = "Test",
                Code = "ZZZ",
                Label = "Test",
                ParentAccountGroupId = new Guid("b0650000-5dd4-0015-b1d7-08dcda6d3ea4"),
                AccountGroupClassificationId = new Guid("cc100000-5dd4-0015-d910-08dcd9665e79"),
            };

            //Act
            var response = await _client.PostAsJsonAsync(_baseUrlForV1, command);
            var result = await response.Content.ReadFromJsonAsync<AccountGroupStandardResult>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            result.Should()
                .NotBeNull()
                .And.BeOfType<AccountGroupStandardResult>()
                .And.Match<AccountGroupStandardResult>(x => x.Code == command.Code);
        }

        [Fact]
        public async Task Add_AccountGroup_WithRO_403()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.RO);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var command = new AddAccountGroupCommand
            {
                Description = "Test",
                Code = "Test",
                Label = "Test",
                AccountGroupClassificationId = new Guid("cc100000-5dd4-0015-d910-08dcd9665e79"),
            };

            //Act
            var response = await _client.PostAsJsonAsync(_baseUrlForV1, command);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Add_AccountGroup_WithAdmin_403()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.MegaAdmin);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var command = new AddAccountGroupCommand
            {
                Description = "Test",
                Code = "Test",
                Label = "Test",
                AccountGroupClassificationId = new Guid("cc100000-5dd4-0015-d910-08dcd9665e79"),
            };

            //Act
            var response = await _client.PostAsJsonAsync(_baseUrlForV1, command);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Add_AccountGroup_WithNoRole_403()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.NoRole);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var command = new AddAccountGroupCommand
            {
                Description = "Test",
                Code = "Test",
                Label = "Test",
                AccountGroupClassificationId = new Guid("cc100000-5dd4-0015-d910-08dcd9665e79"),
            };

            //Act
            var response = await _client.PostAsJsonAsync(_baseUrlForV1, command);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Add_AccountGroup_WithNoAuth_401()
        {
            //Arrange
            var command = new AddAccountGroupCommand
            {
                Description = "Test",
                Code = "Test",
                Label = "Test",
                AccountGroupClassificationId = new Guid("cc100000-5dd4-0015-d910-08dcd9665e79"),
            };

            //Act
            var response = await _client.PostAsJsonAsync(_baseUrlForV1, command);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Add_AccountGroup_WithAlreadyExists_409()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.RW);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var command = new AddAccountGroupCommand
            {
                Description = "Test",
                Code = "106",
                Label = "Test",
                AccountGroupClassificationId = new Guid("cc100000-5dd4-0015-d910-08dcd9665e79"),
            };

            //Act
            var response = await _client.PostAsJsonAsync(_baseUrlForV1, command);
            var result = await response.Content.ReadFromJsonAsync<CustomProblemDetails>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Conflict);
            result.Should()
                .NotBeNull()
                .And.BeOfType<CustomProblemDetails>()
                .And.Match<CustomProblemDetails>(x => x.Errors.First().Code
                    == "ACCOUNTGROUP_IN_CLASSIFICATION_ALREADY_EXISTS");
        }

        [Fact]
        public async Task Add_AccountGroup_WithParentAccountGroupNotExists_400()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.RW);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var command = new AddAccountGroupCommand
            {
                Description = "Test",
                Code = "Test1",
                Label = "Test",
                ParentAccountGroupId = NewId.NextGuid(),
                AccountGroupClassificationId = new Guid("cc100000-5dd4-0015-d910-08dcd9665e79"),
            };

            //Act
            var response = await _client.PostAsJsonAsync(_baseUrlForV1, command);
            var result = await response.Content.ReadFromJsonAsync<CustomProblemDetails>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            result.Should()
                .NotBeNull()
                .And.BeOfType<CustomProblemDetails>()
                .And.Match<CustomProblemDetails>(x => x.Errors.First().Code == "PARENT_ACCOUNTGROUP_NOTFOUND");
        }

        [Fact]
        public async Task Add_AccountGroup_WithClassificationNotExists_400()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.RW);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var command = new AddAccountGroupCommand
            {
                Description = "Test",
                Code = "Test",
                Label = "Test",
                AccountGroupClassificationId = NewId.NextGuid(),
            };

            //Act
            var response = await _client.PostAsJsonAsync(_baseUrlForV1, command);
            var result = await response.Content.ReadFromJsonAsync<CustomProblemDetails>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            result.Should()
                .NotBeNull()
                .And.BeOfType<CustomProblemDetails>()
                .And.Match<CustomProblemDetails>(x => x.Errors.First().Code == "ACCOUNTGROUP_CLASSIFICATION_NOT_FOUND");
        }

        [Fact]
        public async Task Update_AccountGroup_WithRW_OK()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.RW);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var command = new UpdateAccountGroupCommand
            {
                Id = new Guid("78920000-5dd4-0015-db59-08dcd9a9de05"),
                Description = "Test",
                Code = "Test2",
                Label = "Test",
                AccountGroupClassificationId = new Guid("cc100000-5dd4-0015-d910-08dcd9665e79"),
                Version = new Guid("78920000-5dd4-0015-e1bb-08dcd9a9de05"),
            };

            //Act
            var response = await _client.PutAsJsonAsync($"{_baseUrlForV1}/78920000-5dd4-0015-db59-08dcd9a9de05", command);
            var result = await response.Content.ReadFromJsonAsync<AccountGroupStandardResult>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Should()
                .NotBeNull()
                .And.BeOfType<AccountGroupStandardResult>()
                .And.Match<AccountGroupStandardResult>(x => x.Code == command.Code);
        }

        [Fact]
        public async Task Update_AccountGroup_WithOtherTenant_404()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.OtherTenant);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var command = new UpdateAccountGroupCommand
            {
                Id = new Guid("34980000-5dd4-0015-17d3-08dcdaf4ec4e"),
                Description = "Test",
                Code = "TestA2",
                Label = "Test",
                AccountGroupClassificationId = new Guid("cc100000-5dd4-0015-d910-08dcd9665e79"),
                Version = new Guid("78920000-5dd4-0015-e1bb-08dcd9a9de05"),
            };

            //Act
            var response = await _client.PutAsJsonAsync($"{_baseUrlForV1}/34980000-5dd4-0015-17d3-08dcdaf4ec4e", command);
            var result = await response.Content.ReadFromJsonAsync<CustomProblemDetails>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            result.Should()
                .NotBeNull()
                .And.BeOfType<CustomProblemDetails>();
        }

        [Fact]
        public async Task Update_AccountGroup_WithRO_403()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.RO);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var command = new UpdateAccountGroupCommand
            {
                Id = _accountGroupId,
                Description = "Test",
                Code = "Test",
                Label = "Test",
                AccountGroupClassificationId = new Guid("cc100000-5dd4-0015-d910-08dcd9665e79"),
                Version = new Guid("ec860000-5dd4-0015-9a33-08dcda2056e2"),
            };

            //Act
            var response = await _client.PutAsJsonAsync($"{_baseUrlForV1}/{_accountGroupId}", command);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Update_AccountGroup_WithAdmin_403()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.MegaAdmin);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var command = new UpdateAccountGroupCommand
            {
                Id = _accountGroupId,
                Description = "Test",
                Code = "Test",
                Label = "Test",
                AccountGroupClassificationId = new Guid("cc100000-5dd4-0015-d910-08dcd9665e79"),
                Version = new Guid("ec860000-5dd4-0015-9a33-08dcda2056e2"),
            };

            //Act
            var response = await _client.PutAsJsonAsync($"{_baseUrlForV1}/{_accountGroupId}", command);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Update_AccountGroup_WithNoRole_403()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.NoRole);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var command = new UpdateAccountGroupCommand
            {
                Id = _accountGroupId,
                Description = "Test",
                Code = "Test",
                Label = "Test",
                AccountGroupClassificationId = new Guid("cc100000-5dd4-0015-d910-08dcd9665e79"),
                Version = new Guid("ec860000-5dd4-0015-9a33-08dcda2056e2"),
            };

            //Act
            var response = await _client.PutAsJsonAsync($"{_baseUrlForV1}/{_accountGroupId}", command);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Update_AccountGroup_WithNoAuth_401()
        {
            //Arrange
            var command = new UpdateAccountGroupCommand
            {
                Id = _accountGroupId,
                Description = "Test",
                Code = "Test",
                Label = "Test",
                AccountGroupClassificationId = new Guid("cc100000-5dd4-0015-d910-08dcd9665e79"),
                Version = new Guid("ec860000-5dd4-0015-9a33-08dcda2056e2"),
            };

            //Act
            var response = await _client.PutAsJsonAsync($"{_baseUrlForV1}/{_accountGroupId}", command);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Update_AccountGroup_WithNotMatchId_400()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.RW);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var command = new UpdateAccountGroupCommand
            {
                Id = NewId.NextGuid(),
                Description = "Test",
                Code = "Test",
                Label = "Test",
                AccountGroupClassificationId = new Guid("cc100000-5dd4-0015-d910-08dcd9665e79"),
                Version = new Guid("ec860000-5dd4-0015-9a33-08dcda2056e2"),
            };

            //Act
            var response = await _client.PutAsJsonAsync($"{_baseUrlForV1}/{_accountGroupId}", command);
            var result = await response.Content.ReadFromJsonAsync<CustomProblemDetails>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            result.Should()
                .NotBeNull()
                .And.BeOfType<CustomProblemDetails>()
                .And.Match<CustomProblemDetails>(x => x.Errors.First().Code == "ACCOUNTGROUP_COMMAND_IDS_NOT_MATCH");
        }

        [Fact]
        public async Task Update_AccountGroup_WithBadId_404()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.RW);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var id = NewId.NextGuid();
            var command = new UpdateAccountGroupCommand
            {
                Id = id,
                Description = "Test",
                Code = "Test",
                Label = "Test",
                AccountGroupClassificationId = new Guid("cc100000-5dd4-0015-d910-08dcd9665e79"),
                Version = new Guid("ec860000-5dd4-0015-9a33-08dcda2056e2"),
            };

            //Act
            var response = await _client.PutAsJsonAsync($"{_baseUrlForV1}/{id}", command);
            var result = await response.Content.ReadFromJsonAsync<CustomProblemDetails>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            result.Should()
                .NotBeNull()
                .And.BeOfType<CustomProblemDetails>()
                .And.Match<CustomProblemDetails>(x => x.Errors.First().Code == "ACCOUNTGROUP_NOT_FOUND");
        }

        [Fact]
        public async Task Update_AccountGroup_WithAlreadyExists_409()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.RW);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var command = new UpdateAccountGroupCommand
            {
                Id = _accountGroupId,
                Description = "Test",
                Code = "180",
                Label = "Test",
                AccountGroupClassificationId = new Guid("cc100000-5dd4-0015-d910-08dcd9665e79"),
                Version = new Guid("ec860000-5dd4-0015-9a33-08dcda2056e2"),
            };

            //Act
            var response = await _client.PutAsJsonAsync($"{_baseUrlForV1}/{_accountGroupId}", command);
            var result = await response.Content.ReadFromJsonAsync<CustomProblemDetails>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Conflict);
            result.Should()
                .NotBeNull()
                .And.BeOfType<CustomProblemDetails>()
                .And.Match<CustomProblemDetails>(x => x.Errors.First().Code == "ACCOUNTGROUP_IN_CLASSIFICATION_ALREADY_EXISTS");
        }

        [Fact]
        public async Task Update_AccountGroup_WithParentAccountGroup_OK()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.RW);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var command = new UpdateAccountGroupCommand
            {
                Id = new Guid("ec860000-5dd4-0015-c40a-08dcda311af0"),
                Description = "Test",
                Code = "Test5",
                Label = "Test",
                AccountGroupClassificationId = new Guid("cc100000-5dd4-0015-d910-08dcd9665e79"),
                Version = new Guid("ec860000-5dd4-0015-c6eb-08dcda311af0"),
                ParentAccountGroupId = new Guid("ec860000-5dd4-0015-2d06-08dcda1f7bbb"),
            };

            //Act
            var response = await _client.PutAsJsonAsync($"{_baseUrlForV1}/ec860000-5dd4-0015-c40a-08dcda311af0", command);
            var result = await response.Content.ReadFromJsonAsync<AccountGroupStandardResult>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Should()
                .NotBeNull()
                .And.BeOfType<AccountGroupStandardResult>()
                .And.Match<AccountGroupStandardResult>(x => x.Code == command.Code);
        }

        [Fact]
        public async Task Update_AccountGroup_WithParentAccountGroupNotExists_400()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.RW);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var command = new UpdateAccountGroupCommand
            {
                Id = new Guid("ec860000-5dd4-0015-6596-08dcda313f52"),
                Description = "Test",
                Code = "Test3",
                Label = "Test",
                AccountGroupClassificationId = new Guid("cc100000-5dd4-0015-d910-08dcd9665e79"),
                Version = new Guid("ec860000-5dd4-0015-9509-08dcda31d988"),
                ParentAccountGroupId = NewId.NextGuid(),
            };

            //Act
            var response = await _client.PutAsJsonAsync($"{_baseUrlForV1}/ec860000-5dd4-0015-6596-08dcda313f52", command);
            var result = await response.Content.ReadFromJsonAsync<CustomProblemDetails>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            result.Should()
                .NotBeNull()
                .And.BeOfType<CustomProblemDetails>()
                .And.Match<CustomProblemDetails>(x => x.Errors.First().Code == "PARENT_ACCOUNTGROUP_NOTFOUND");
        }

        [Fact]
        public async Task Update_AccountGroup_WithClassificationNotExists_400()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.RW);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var command = new UpdateAccountGroupCommand
            {
                Id = new Guid("cc100000-5dd4-0015-3bb7-08dcd9780cd7"),
                Description = "Test",
                Code = "Test6",
                Label = "Test",
                AccountGroupClassificationId = NewId.NextGuid(),
                Version = new Guid("ec860000-5dd4-0015-0d47-08dcda322a37"),
            };

            //Act
            var response = await _client.PutAsJsonAsync($"{_baseUrlForV1}/cc100000-5dd4-0015-3bb7-08dcd9780cd7", command);
            var result = await response.Content.ReadFromJsonAsync<CustomProblemDetails>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            result.Should()
                .NotBeNull()
                .And.BeOfType<CustomProblemDetails>()
                .And.Match<CustomProblemDetails>(x => x.Errors.First().Code == "ACCOUNTGROUP_CLASSIFICATION_NOT_FOUND");
        }

        [Fact]
        public async Task Delete_AccountGroup_WithRW_OK()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.RW);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.DeleteAsync($"{_baseUrlForV1}/{_accountGroupToDel}");
            var result = await response.Content.ReadFromJsonAsync<List<AccountGroupStandardResult>>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Should()
                .NotBeNull()
                .And.BeOfType<List<AccountGroupStandardResult>>()
                .And.Match<List<AccountGroupStandardResult>>(x => x.Count == 10);
        }

        [Fact]
        public async Task Delete_AccountGroup_WithRO_403()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.RO);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.DeleteAsync($"{_baseUrlForV1}/{_accountGroupToDel}");

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Delete_AccountGroup_WithAdmin_403()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.MegaAdmin);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.DeleteAsync($"{_baseUrlForV1}/{_accountGroupToDel}");

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Delete_AccountGroup_WithNoRole_403()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.NoRole);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.DeleteAsync($"{_baseUrlForV1}/{_accountGroupToDel}");

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Delete_AccountGroup_WithNoAuth_401()
        {
            //Act
            var response = await _client.DeleteAsync($"{_baseUrlForV1}/{_accountGroupToDel}");

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Delete_AccountGroup_WithBadId_404()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.RW);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var id = NewId.NextGuid();

            //Act
            var response = await _client.DeleteAsync($"{_baseUrlForV1}/{id}");
            var result = await response.Content.ReadFromJsonAsync<CustomProblemDetails>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            result.Should()
                .NotBeNull()
                .And.BeOfType<CustomProblemDetails>()
                .And.Match<CustomProblemDetails>(x => x.Errors.First().Code == "ACCOUNTGROUP_NOT_FOUND");
        }

        [Fact]
        public async Task Delete_AccountGroup_WithOtherTenant_404()
        {
            //Arrange
            var token = await GetAccessTokenAsync(TokenType.OtherTenant);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Act
            var response = await _client.DeleteAsync($"{_baseUrlForV1}/34980000-5dd4-0015-43e3-08dcdaf50cf9");
            var result = await response.Content.ReadFromJsonAsync<CustomProblemDetails>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            result.Should()
                .NotBeNull()
                .And.BeOfType<CustomProblemDetails>();
        }
    }
}
