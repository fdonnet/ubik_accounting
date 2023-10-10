using MediatR;
using Ubik.Accounting.Api.Features.Accounts.Mappers;

namespace Ubik.Accounting.Api.Features.Accounts.Queries
{
    public class GetAllAccounts
    {
        public record GetAllAccountQuery : IRequest<IEnumerable<GetAllAccountResult>> { }

        public record GetAllAccountResult
        {
            public Guid Id { get; set; }
            public required string Code { get; set; }
            public required string Label { get; set; }
            public string? Description { get; set; }
            public Guid AccountGroupId { get; set; }
            public Guid Version { get; set; }
        }

        public class Handler : IRequestHandler<GetAllAccountQuery, IEnumerable<GetAllAccountResult>>
        {
            private readonly IServiceManager _serviceManager;

            public Handler(IServiceManager serviceManager)
            {
                _serviceManager = serviceManager;
            }

            public async Task<IEnumerable<GetAllAccountResult>> Handle(GetAllAccountQuery request, CancellationToken cancellationToken)
            {
                var accounts = await _serviceManager.AccountService.GetAccountsAsync();
                return accounts.ToGetAllAccountResult();
            }
        }
    }
}
