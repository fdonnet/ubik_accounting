using MediatR;
using static Ubik.Accounting.Api.Features.Accounts.Commands.UpdateAccount;
using Ubik.Accounting.Api.Features.Accounts.Exceptions;
using Ubik.Accounting.Api.Features.AccountGroups.Exceptions;
using Ubik.Accounting.Api.Features.AccountGroups.Mappers;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Ubik.Accounting.Api.Features.AccountGroups.Commands
{
    public class UpdateAccountGroup
    {
        //Input
        public record UpdateAccountGroupCommand : IRequest<UpdateAccountGroupResult>
        {
            [Required]
            [JsonIgnore]
            public Guid Id { get; set; }
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
            [Required]
            public Guid Version { get; set; }
        }

        //Output
        public record UpdateAccountGroupResult
        {
            public Guid Id { get; set; }
            public string Code { get; set; } = default!;
            public string Label { get; set; } = default!;
            public string? Description { get; set; }
            public Guid? ParentAccountGroupId { get; set; }
            public Guid AccountGroupClassificationId { get; set; }
            public Guid Version { get; set; }
        }

        public class UpdateAccountGroupHandler : IRequestHandler<UpdateAccountGroupCommand, UpdateAccountGroupResult>
        {
            private readonly IServiceManager _serviceManager;

            public UpdateAccountGroupHandler(IServiceManager serviceManager)
            {
                _serviceManager = serviceManager;
            }

            public async Task<UpdateAccountGroupResult> Handle(UpdateAccountGroupCommand request, CancellationToken cancellationToken)
            {
                //Check if the account group code already exists in other records
                var alreadyExistsWithOtherId = await _serviceManager.AccountGroupService
                    .IfExistsWithDifferentIdAsync(request.Code, request.AccountGroupClassificationId, request.Id);

                if (alreadyExistsWithOtherId)
                    throw new AccountGroupAlreadyExistsException(request.Code,request.AccountGroupClassificationId);

                //Check if the account group is found
                var accountGroup = await _serviceManager.AccountGroupService.GetAsync(request.Id) 
                    ?? throw new AccountGroupNotFoundException(request.Id);

                //Check if parent account group exists
                if (request.ParentAccountGroupId != null)
                {
                    var parentAccountGroupExists = await _serviceManager.AccountGroupService.IfExistsAsync((Guid)request.ParentAccountGroupId);
                    if (!parentAccountGroupExists)
                    {
                        throw new AccountGroupParentNotFoundException((Guid)request.ParentAccountGroupId);
                    }
                }

                //Modify the found account group
                accountGroup = request.ToAccountGroup(accountGroup);

                var result = _serviceManager.AccountGroupService.Update(accountGroup);
                await _serviceManager.SaveAsync();


                return result.ToUpdateAccountGroupResult();
            }
        }
    }
}
