using MediatR;
using static Ubik.Accounting.Api.Features.Accounts.Commands.DeleteAccount;
using Ubik.Accounting.Api.Features.Accounts.Exceptions;
using Ubik.Accounting.Api.Features.AccountGroups.Exceptions;

namespace Ubik.Accounting.Api.Features.AccountGroups.Commands
{
    public class DeleteAccountGroup
    {
        //Input
        public record DeleteAccountGroupCommand : IRequest<bool>
        {
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

                await _serviceManager.AccountGroupService.DeleteAsync(accountGroup.Id);

                return true;
            }
        }
    }
}
