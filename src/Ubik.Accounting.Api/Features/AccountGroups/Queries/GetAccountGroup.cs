using MediatR;
using System.ComponentModel.DataAnnotations;
using Ubik.Accounting.Api.Features.AccountGroups.Exceptions;
using Ubik.Accounting.Api.Features.AccountGroups.Mappers;

namespace Ubik.Accounting.Api.Features.AccountGroups.Queries
{
    public class GetAccountGroup
    {
        //Input
        public record GetAccountGroupQuery : IRequest<GetAccountGroupResult>
        {
            [Required]
            public Guid Id { get; set; }
        }

        //Output
        public record GetAccountGroupResult
        {
            public Guid Id { get; set; }
            public required string Code { get; set; }
            public required string Label { get; set; }
            public string? Description { get; set; }
            public Guid? ParentAccountGroupId { get; set; }
            public Guid AccountGroupClassificationId { get; set; }
            public Guid Version { get; set; }
        }

        //Handler
        public class GetAccountGroupHandler : IRequestHandler<GetAccountGroupQuery, GetAccountGroupResult>
        {
            private readonly IServiceManager _serviceManager;

            public GetAccountGroupHandler(IServiceManager serviceManager)
            {
                _serviceManager = serviceManager;
            }

            public async Task<GetAccountGroupResult> Handle(GetAccountGroupQuery request, CancellationToken cancellationToken)
            {
                var accountGroup = await _serviceManager.AccountGroupService.GetAsync(request.Id);

                return accountGroup == null 
                    ? throw new AccountGroupNotFoundException(request.Id) 
                    : accountGroup.ToGetAccountGroupResult();
            }
        }
    }
}
