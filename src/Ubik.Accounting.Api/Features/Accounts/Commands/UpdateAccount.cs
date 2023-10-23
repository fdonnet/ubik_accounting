using MediatR;
using System.ComponentModel.DataAnnotations;
using Ubik.Accounting.Api.Features.Accounts.Exceptions;
using Ubik.Accounting.Api.Features.Accounts.Mappers;

namespace Ubik.Accounting.Api.Features.Accounts.Commands
{
    public class UpdateAccount
    {
        //Input
        public record UpdateAccountCommand : IRequest<UpdateAccountResult>
        {
            [Required]
            public Guid Id { get; set; }
            [Required]
            [MaxLength(20)]
            public string Code { get; set; } = default!;
            [Required]
            [MaxLength(100)]
            public string Label { get; set; } = default!;
            [MaxLength(700)]
            public string? Description { get; set; }
            [Required]
            public Guid Version { get; set; }
        }

        //Output
        public record UpdateAccountResult
        {
            public Guid Id { get; set; }
            public string Code { get; set; } = default!;
            public string Label { get; set; } = default!;
            public string? Description { get; set; }
            public Guid Version { get; set; }
        }


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
                var account = await _serviceManager.AccountService.GetAsync(request.Id) 
                                ?? throw new AccountNotFoundException(request.Id);

                //Modify the found account
                account = request.ToAccount(account);

                var result = _serviceManager.AccountService.Update(account);
                await _serviceManager.SaveAsync();

                return result.ToUpdateAccountResult();
            }
        }
    }
}
