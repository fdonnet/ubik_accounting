using Bogus;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Ubik.Accounting.Api.Features;
using Ubik.Accounting.Api.Features.Accounts.Commands;
using Ubik.Accounting.Api.Features.Accounts.Exceptions;
using Ubik.Accounting.Api.Features.Accounts.Mappers;
using Ubik.Accounting.Api.Features.Accounts.Queries;
using Ubik.Accounting.Api.Models;
using Ubik.ApiService.Common.Exceptions;
using Ubik.ApiService.Common.Validators;
using static Ubik.Accounting.Api.Features.Accounts.Queries.GetAccount;

namespace Ubik.Accounting.Api.Tests.UnitTests.Features.Accounts.Queries
{
    public class GetAccount_Test
    {
        private readonly IServiceManager _serviceManager;
        private readonly ILogger<IRequestHandler<GetAccountQuery, GetAccountResult>> _logger;
        private readonly GetAccountHandler _handler;
        private readonly GetAccountQuery _query;
        private readonly Account _account;
        private readonly ValidationPipelineBehavior<GetAccountQuery, GetAccountResult> _validationBehavior;

        public GetAccount_Test()
        {
            _serviceManager = Substitute.For<IServiceManager>();
            _logger = Substitute.For<ILogger<IRequestHandler<GetAccountQuery, GetAccountResult>>>();
            _handler = new GetAccountHandler(_serviceManager,_logger);

            _query = new GetAccountQuery()
            {
                Id = Guid.NewGuid()
            };

            _account = new Account() { Code = "TEST", Label = "Test" };

            _validationBehavior = new ValidationPipelineBehavior<GetAccountQuery, GetAccountResult>(new GetAccountValidator());
        }

        [Fact]
        public async Task Get_Account_Ok()
        {
            //Arrange
            _serviceManager.AccountService.GetAccountAsync(_query.Id).Returns(_account);

            //Act
            var result = await _handler.Handle(_query, CancellationToken.None);

            //Assert
            result.Should()
                    .NotBeNull()
                    .And.BeOfType<GetAccountResult>();
        }

        [Fact]
        public async Task GetAccount_AccountNotFoundException_AccountIdNotFound()
        {
            //Arrange
            _serviceManager.AccountService.GetAccountAsync(_query.Id).Returns(Task.FromResult<Account?>(null));

            //Act
            Func<Task> act = async () => await _handler.Handle(_query, CancellationToken.None);

            //Assert
            await act.Should().ThrowAsync<AccountNotFoundException>()
                .Where(e => e.ErrorType == ServiceAndFeatureExceptionType.NotFound);
        }
    }
}
