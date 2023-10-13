using MediatR;
using Ubik.Accounting.Api.Features.Accounts.Exceptions;
using Ubik.Accounting.Api.Features.Accounts.Mappers;

namespace Ubik.Accounting.Api.Features.Accounts.Queries
{
    public class GetAccount
    {
        //Input
        public record GetAccountQuery : IRequest<GetAccountResult> 
        {
            public Guid Id { get; set; }
        }

        //Output
        public record GetAccountResult
        {
            public Guid Id { get; set; }
            public required string Code { get; set; }
            public required string Label { get; set; }
            public string? Description { get; set; }
            public Guid AccountGroupId { get; set; }
            public Guid Version { get; set; }
        }

        //Handler
        public class GetAccountHandler : IRequestHandler<GetAccountQuery, GetAccountResult>
        {
            private readonly IServiceManager _serviceManager;
            private readonly ILogger<IRequestHandler<GetAccountQuery, GetAccountResult>> _logger;

            public GetAccountHandler(IServiceManager serviceManager, ILogger<IRequestHandler<GetAccountQuery, GetAccountResult>> logger)
            {
                _serviceManager = serviceManager;
                _logger = logger;
            }

            public async Task<GetAccountResult> Handle(GetAccountQuery request, CancellationToken cancellationToken)
            {
                var account = await _serviceManager.AccountService.GetAccountAsync(request.Id);

                if (account == null)
                {
                    _logger.LogInformation("This account Id {id} doesn't exist.", request.Id);
                    throw new AccountNotFoundException(request.Id);
                }
                else
                    return account.ToGetAccountResult();
            }
        }
    }
}
