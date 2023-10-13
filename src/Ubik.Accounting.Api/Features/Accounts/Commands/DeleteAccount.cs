using MediatR;
using Ubik.Accounting.Api.Features.Accounts.Exceptions;
using Ubik.Accounting.Api.Features.Accounts.Mappers;

namespace Ubik.Accounting.Api.Features.Accounts.Commands;
public class DeleteAccount
{
    //Input
    public record DeleteAccountCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
    }


    public class DeleteAccountHandler : IRequestHandler<DeleteAccountCommand,bool>
    {
        private readonly IServiceManager _serviceManager;

        public DeleteAccountHandler(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }

        public async Task<bool> Handle(DeleteAccountCommand request, CancellationToken cancellationToken)
        {
            //Check if the account is found
            var account = await _serviceManager.AccountService.GetAccountAsync(request.Id)
                            ?? throw new AccountNotFoundException(request.Id);

            await _serviceManager.AccountService.DeleteAccountAsync(account.Id);

            return true;
        }
    }
}

