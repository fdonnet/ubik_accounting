using MediatR;
using System.ComponentModel.DataAnnotations;
using Ubik.Accounting.Api.Features.AccountGroups.Exceptions;
using Ubik.Accounting.Api.Features.AccountGroups.Mappers;

namespace Ubik.Accounting.Api.Features.AccountGroups.Commands
{
    public class AddAccountGroup
    {

        //Input
        public record AddAccountGroupCommand : IRequest<AddAccountGroupResult>
        {
            [Required]
            [MaxLength(20)]
            public string Code { get; set; } = default!;
            [Required]
            [MaxLength(100)]
            public string Label { get; set; } = default!;
            [MaxLength(700)]
            public string? Description { get; set; }
            public Guid? ParentAccountGroupId { get; set; }
            [Required]
            public Guid AccountGroupClassificationId { get; set; }
        }

        //Output
        public record AddAccountGroupResult
        {
            public Guid Id { get; set; }
            public string Code { get; set; } = default!;
            public string Label { get; set; } = default!;
            public string? Description { get; set; }
            public Guid? ParentAccountGroupId { get; set; }
            public Guid AccountGroupClassificationId { get; set; }
            public Guid Version { get; set; }
        }

        public class AddAccountGroupHandler : IRequestHandler<AddAccountGroupCommand, AddAccountGroupResult>
        {
            private readonly IServiceManager _serviceManager;
            public AddAccountGroupHandler(IServiceManager serviceManager)
            {
                _serviceManager = serviceManager;
            }
            public async Task<AddAccountGroupResult> Handle(AddAccountGroupCommand request, CancellationToken cancellationToken)
            {
                var accountGroup = request.ToAccountGroup();

                var accountGroupExists = await _serviceManager.AccountGroupService
                    .IfExistsAsync(accountGroup.Code, accountGroup.AccountGroupClassificationId);

                if (accountGroupExists)
                    throw new AccountGroupAlreadyExistsException(request.Code,request.AccountGroupClassificationId);
                
                //Check if parent account group exists
                if(accountGroup.ParentAccountGroupId != null)
                {
                    var parentAccountExists = await _serviceManager.AccountGroupService
                        .IfExistsAsync((Guid)accountGroup.ParentAccountGroupId);

                    if (!parentAccountExists)
                        throw new AccountGroupParentNotFoundException((Guid)accountGroup.ParentAccountGroupId);
                }

                await _serviceManager.AccountGroupService.AddAsync(accountGroup);
                await _serviceManager.SaveAsync();

                return accountGroup.ToAddAccountGroupResult();
            }
        }
    }
}
