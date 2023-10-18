using MediatR;
using Ubik.Accounting.Api.Features.AccountGroups.Mappers;

namespace Ubik.Accounting.Api.Features.AccountGroups.Queries
{
    public class GetAllAccountGroups
    {
        public record GetAllAccountGroupsQuery : IRequest<IEnumerable<GetAllAccountGroupsResult>> { }

        public record GetAllAccountGroupsResult
        {
            public Guid Id { get; set; }
            public required string Code { get; set; }
            public required string Label { get; set; }
            public string? Description { get; set; }
            public Guid? ParentAccountGroupId { get; set; }
            public Guid Version { get; set; }
        }

        public class GetAllAccountGroupsHandler : IRequestHandler<GetAllAccountGroupsQuery, IEnumerable<GetAllAccountGroupsResult>>
        {
            private readonly IServiceManager _serviceManager;

            public GetAllAccountGroupsHandler(IServiceManager serviceManager)
            {
                _serviceManager = serviceManager;
            }

            public async Task<IEnumerable<GetAllAccountGroupsResult>> Handle(GetAllAccountGroupsQuery request, CancellationToken cancellationToken)
            {
                var accountGroups = await _serviceManager.AccountGroupService.GetAllAsync();
                return accountGroups.ToGetAllAccountGroupsResult();
            }
        }
    }
}
