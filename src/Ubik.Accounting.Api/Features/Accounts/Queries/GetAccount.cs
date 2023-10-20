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
            public Guid Version { get; set; }
        }

        //Handler
        public class GetAccountHandler : IRequestHandler<GetAccountQuery, GetAccountResult>
        {
            private readonly IServiceManager _serviceManager;

            public GetAccountHandler(IServiceManager serviceManager)
            {
                _serviceManager = serviceManager;
            }

            public async Task<GetAccountResult> Handle(GetAccountQuery request, CancellationToken cancellationToken)
            {
                var account = await _serviceManager.AccountService.GetAsync(request.Id);

                return account == null 
                    ? throw new AccountNotFoundException(request.Id) 
                    : account.ToGetAccountResult();
            }
        }
    }
}
