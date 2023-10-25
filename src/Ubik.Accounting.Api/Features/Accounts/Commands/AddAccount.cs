using MassTransit;
using MediatR;
using System.ComponentModel.DataAnnotations;
using Ubik.Accounting.Api.Features.Accounts.Exceptions;
using Ubik.Accounting.Api.Features.Accounts.Mappers;
using Ubik.Accounting.Contracts;
using Ubik.ApiService.DB.Enums;

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
            [Required]
            public AccountCategory Category { get; set; }
            [Required]
            public AccountDomain Domain { get; set; }
            [Required]
            public Guid CurrencyId { get; set; }
        }

        //Output
        public record AddAccountResult
        {
            public Guid Id { get; set; }
            public string Code { get; set; } = default!;
            public string Label { get; set; } = default!;
            public AccountCategory Category { get; set; }
            public AccountDomain Domain { get; set; }
            public string? Description { get; set; }
            public Guid CurrencyId { get; set; }
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

                //Publish and store
                await _serviceManager.AccountService.AddAsync(account);
                await _publishEndpoint.Publish(account.ToAccountAdded(), CancellationToken.None);

                await _serviceManager.SaveAsync();

                return account.ToAddAccountResult();
            }
        }
    }
}
