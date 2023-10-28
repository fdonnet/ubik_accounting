using MediatR;
using Ubik.Accounting.Api.Features.AccountGroups.Mappers;
using Ubik.Accounting.Api.Features.AccountGroups.Exceptions;
using System.ComponentModel.DataAnnotations;

namespace Ubik.Accounting.Api.Features.AccountGroups.Queries
{
    public class GetChildAccounts
    {
        public record GetChildAccountsQuery : IRequest<IEnumerable<GetChildAccountsResult>>
        {
            [Required]
            public Guid AccountGroupId { get; set; }
        }

        public record GetChildAccountsResult
        {
            public Guid Id { get; set; }
            public required string Code { get; set; }
            public required string Label { get; set; }
            public string? Description { get; set; }
            public Guid Version { get; set; }
        }

        public class GetChildAccountsHandler : IRequestHandler<GetChildAccountsQuery, IEnumerable<GetChildAccountsResult>>
        {
            private readonly IServiceManager _serviceManager;

            public GetChildAccountsHandler(IServiceManager serviceManager)
            {
                _serviceManager = serviceManager;
            }

            public async Task<IEnumerable<GetChildAccountsResult>> Handle(GetChildAccountsQuery query, CancellationToken cancellationToken)
            {
                var accountGroup = await _serviceManager.AccountGroupService.GetWithChildAccountsAsync(query.AccountGroupId);

                return accountGroup == null
                    ? throw new AccountGroupNotFoundException(query.AccountGroupId)
                    : accountGroup.Accounts == null
                    ? new List<GetChildAccountsResult>()
                    : accountGroup.Accounts.ToGetChildAccountsResult();
            }
        }
    }
}
