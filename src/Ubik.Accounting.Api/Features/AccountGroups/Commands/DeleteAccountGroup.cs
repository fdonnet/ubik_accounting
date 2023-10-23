using MediatR;
using System.ComponentModel.DataAnnotations;
using Ubik.Accounting.Api.Features.AccountGroups.Exceptions;

namespace Ubik.Accounting.Api.Features.AccountGroups.Commands
{
    public class DeleteAccountGroup
    {
        //Input
        public record DeleteAccountGroupCommand : IRequest<bool>
        {
            [Required]
            public Guid Id { get; set; }
        }

        public class DeleteAccountGroupHandler : IRequestHandler<DeleteAccountGroupCommand, bool>
        {
            private readonly IServiceManager _serviceManager;

            public DeleteAccountGroupHandler(IServiceManager serviceManager)
            {
                _serviceManager = serviceManager;
            }

            public async Task<bool> Handle(DeleteAccountGroupCommand request, CancellationToken cancellationToken)
            {
                //Check if the accountGroup is found
                var accountGroup = await _serviceManager.AccountGroupService.GetAsync(request.Id) 
                    ?? throw new AccountGroupNotFoundException(request.Id);

                //Has child account groups ?
                var hasChildAccountGroups = await _serviceManager.AccountGroupService.HasAnyChildAccountGroups(request.Id);
                if (hasChildAccountGroups)
                    throw new AccountGroupHasChildAccountGroupsException(request.Id);


                //TODO: Has child accounts ?
                //var hasChildAccounts = await _serviceManager.AccountGroupService.HasAnyChildAccounts(request.Id);
                //if (hasChildAccounts)
                //    throw new AccountGroupHasChildAccountsException(request.Id);


                await _serviceManager.AccountGroupService.ExecuteDeleteAsync(accountGroup.Id);

                return true;
            }
        }
    }
}
