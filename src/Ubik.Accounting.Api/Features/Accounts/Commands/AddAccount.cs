using Microsoft.AspNetCore.Mvc.RazorPages;
using MediatR;
using System.ComponentModel.DataAnnotations;
using Ubik.Accounting.Api.Features.Accounts.Exceptions;
using Ubik.Accounting.Api.Features.Accounts.Mappers;

namespace Ubik.Accounting.Api.Features.Accounts.Commands
{
    public class AddAccount
    {
        //Input
        public record AddAccountCommand : IRequest<AddAccountResult>
        {
            public required string Code { get; set; }
            public required string Label { get; set; }
            public string? Description { get; set; }
            public Guid AccountGroupId { get; set; }
        }

        //Output
        public record AddAccountResult
        {
            public Guid Id { get; set; }
            public required string Code { get; set; }
            public required string Label { get; set; }
            public string? Description { get; set; }
            public Guid AccountGroupId { get; set; }
            public Guid Version { get; set; }
        }

        public class Handler : IRequestHandler<AddAccountCommand, AddAccountResult>
        {
            private readonly IServiceManager _serviceManager;
            public Handler(IServiceManager serviceManager)
            {
                _serviceManager = serviceManager;
            }
            public async Task<AddAccountResult> Handle(AddAccountCommand request, CancellationToken cancellationToken)
            {
                var account = request.ToAccount();
                var accountExists = await _serviceManager.AccountService.IfExists(account.Code);
                
                if (accountExists)
                    throw new AccountAlreadyExistsException(request.Code);

                await _serviceManager.AccountService.AddAccountAsync(account);
                await _serviceManager.SaveAsync();

                return account.ToAddAccountResult();
            }
        }


    }
}
