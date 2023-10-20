using MediatR;
using Ubik.Accounting.Api.Features.Accounts.Exceptions;
using Ubik.Accounting.Api.Features.Accounts.Mappers;

namespace Ubik.Accounting.Api.Features.Accounts.Commands
{
    public class AddAccount
    {
        //Input
        public record AddAccountCommand : IRequest<AddAccountResult>
        {
            public string Code { get; set; } = default!;
            public string Label { get; set; } = default!;
            public string? Description { get; set; }
        }

        //Output
        public record AddAccountResult
        {
            public Guid Id { get; set; }
            public string Code { get; set; } = default!;
            public string Label { get; set; } = default!;
            public string? Description { get; set; }
            public Guid Version { get; set; }
        }

        public class AddAccountHandler : IRequestHandler<AddAccountCommand, AddAccountResult>
        {
            private readonly IServiceManager _serviceManager;
            public AddAccountHandler(IServiceManager serviceManager)
            {
                _serviceManager = serviceManager;
            }
            public async Task<AddAccountResult> Handle(AddAccountCommand request, CancellationToken cancellationToken)
            {
                var account = request.ToAccount();

                var accountExists = await _serviceManager.AccountService.IfExistsAsync(account.Code);
                if (accountExists)
                    throw new AccountAlreadyExistsException(request.Code);

                await _serviceManager.AccountService.AddAsync(account);
                await _serviceManager.SaveAsync();

                return account.ToAddAccountResult();
            }
        }


    }
}
