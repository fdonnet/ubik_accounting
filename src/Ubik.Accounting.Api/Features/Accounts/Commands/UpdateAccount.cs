using MediatR;
using Ubik.Accounting.Api.Features.Accounts.Exceptions;
using Ubik.Accounting.Api.Features.Accounts.Mappers;

namespace Ubik.Accounting.Api.Features.Accounts.Commands
{
    public class UpdateAccount
    {
        //Input
        public record UpdateAccountCommand : IRequest<UpdateAccountResult>
        {
            public Guid Id { get; set; }
            public string Code { get; set; } = default!;
            public string Label { get; set; } = default!;
            public string? Description { get; set; }
            public Guid AccountGroupId { get; set; }
            public Guid Version { get; set; }
        }

        //Output
        public record UpdateAccountResult
        {
            public Guid Id { get; set; }
            public string Code { get; set; } = default!;
            public string Label { get; set; } = default!;
            public string? Description { get; set; }
            public Guid AccountGroupId { get; set; }
            public Guid Version { get; set; }
        }


        //TODO: not forget to add group ID exists check
        public class UpdateAccountHandler : IRequestHandler<UpdateAccountCommand, UpdateAccountResult>
        {
            private readonly IServiceManager _serviceManager;

            public UpdateAccountHandler(IServiceManager serviceManager)
            {
                _serviceManager = serviceManager;
            }

            public async Task<UpdateAccountResult> Handle(UpdateAccountCommand request, CancellationToken cancellationToken)
            {
                //Check if the account code already exists in other records
                bool exists = await _serviceManager.AccountService.IfExistsWithDifferentIdAsync(request.Code, request.Id);
                if (exists)
                    throw new AccountAlreadyExistsException(request.Code);

                //Check if the account is found
                var account = await _serviceManager.AccountService.GetAccountAsync(request.Id) 
                                ?? throw new AccountNotFoundException(request.Id);

                //Modify the found account
                account = request.ToAccount(account);

                var result = await _serviceManager.AccountService.UpdateAccountAsync(account);

                return result.ToUpdateAccountResult();
            }
        }
    }
}
