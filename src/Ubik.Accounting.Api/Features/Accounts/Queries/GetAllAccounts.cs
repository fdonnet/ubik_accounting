using MediatR;
using Ubik.Accounting.Api.Features.Accounts.Mappers;

namespace Ubik.Accounting.Api.Features.Accounts.Queries
{
    public class GetAllAccounts
    {
        public record GetAllAccountsQuery : IRequest<IEnumerable<GetAllAccountsResult>> { }

        public record GetAllAccountsResult
        {
            public Guid Id { get; set; }
            public required string Code { get; set; }
            public required string Label { get; set; }
            public string? Description { get; set; }
            public Guid Version { get; set; }
        }

        public class GetAllAccountsHandler : IRequestHandler<GetAllAccountsQuery, IEnumerable<GetAllAccountsResult>>
        {
            private readonly IServiceManager _serviceManager;

            public GetAllAccountsHandler(IServiceManager serviceManager)
            {
                _serviceManager = serviceManager;
            }

            public async Task<IEnumerable<GetAllAccountsResult>> Handle(GetAllAccountsQuery request, CancellationToken cancellationToken)
            {
                var accounts = await _serviceManager.AccountService.GetAllAsync();
                return accounts.ToGetAllAccountResult();
            }
        }
    }
}
