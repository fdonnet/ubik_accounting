using MassTransit;
using MediatR;
using System.ComponentModel.DataAnnotations;
using Ubik.Accounting.Api.Features.Accounts.Exceptions;
using Ubik.Accounting.Api.Features.Accounts.Mappers;
using Ubik.Accounting.Contracts;

namespace Ubik.Accounting.Api.Features.Accounts.Commands
{
    public class AddAccount
    {
        //Input
        public record AddAccountCommand : IRequest<AddAccountResult>
        {
            [Required]
            [MaxLength(20)]
            public string Code { get; set; } = default!;
            [Required]
            [MaxLength(100)]
            public string Label { get; set; } = default!;
            [MaxLength(700)]
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
            private readonly IPublishEndpoint _publishEndpoint;
            public AddAccountHandler(IServiceManager serviceManager, IPublishEndpoint publishEndpoint)
            {
                _serviceManager = serviceManager;
                _publishEndpoint = publishEndpoint;
            }
            public async Task<AddAccountResult> Handle(AddAccountCommand request, CancellationToken cancellationToken)
            {
                var account = request.ToAccount();

                var accountExists = await _serviceManager.AccountService.IfExistsAsync(account.Code);
                if (accountExists)
                    throw new AccountAlreadyExistsException(request.Code);

                await _serviceManager.AccountService.AddAsync(account);
                await _serviceManager.SaveAsync();

                await _publishEndpoint.Publish(new AccountCreated 
                    { 
                        Code = account.Code,
                        Label = account.Label,
                        Description = account.Description,
                        Version = account.Version,
                        Id = account.Id,
                        TenantId = account.TenantId
                    });

                return account.ToAddAccountResult();
            }
        }


    }
}
