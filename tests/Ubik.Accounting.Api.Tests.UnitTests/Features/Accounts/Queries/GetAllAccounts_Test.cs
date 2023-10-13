﻿using Bogus;
using FluentAssertions;
using NSubstitute;
using Ubik.Accounting.Api.Features;
using Ubik.Accounting.Api.Features.Accounts.Commands;
using Ubik.Accounting.Api.Features.Accounts.Exceptions;
using Ubik.Accounting.Api.Features.Accounts.Mappers;
using Ubik.Accounting.Api.Features.Accounts.Queries;
using Ubik.Accounting.Api.Models;
using Ubik.ApiService.Common.Exceptions;
using Ubik.ApiService.Common.Validators;
using static Ubik.Accounting.Api.Features.Accounts.Queries.GetAllAccounts;

namespace Ubik.Accounting.Api.Tests.UnitTests.Features.Accounts.Queries
{
    public class GetAllAccounts_Test
    {
        private readonly IServiceManager _serviceManager;
        private readonly GetAllAccountsHandler _handler;
        private readonly GetAllAccountsQuery _query;
        private IEnumerable<Account> _accounts;
        private readonly ValidationPipelineBehavior<GetAllAccountsQuery, IEnumerable<GetAllAccountsResult>> _validationBehavior;

        public GetAllAccounts_Test()
        {
            _serviceManager = Substitute.For<IServiceManager>();
            _handler = new GetAllAccountsHandler(_serviceManager);

            _query = new GetAllAccountsQuery();

            _accounts = new Account[] { new Account() { Code = "TEST", Label = "Test" } };

            _validationBehavior = new ValidationPipelineBehavior<GetAllAccountsQuery, 
                IEnumerable<GetAllAccountsResult>>(new GetAllAccountsValidator());

        }

        [Fact]
        public async Task Get_Accounts_Ok()
        {
            //Arrange
            _serviceManager.AccountService.GetAccountsAsync().Returns(_accounts);

            //Act
            var result = await _handler.Handle(_query, CancellationToken.None);

            //Assert
            result.Should()
                    .NotBeNull()
                    .And.BeEquivalentTo(_accounts.ToGetAllAccountResult())
                    .And.HaveCount(1);
        }
    }
}