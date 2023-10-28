using MassTransit;
using MediatR;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Ubik.Accounting.Api.Features.Accounts.Exceptions;
using Ubik.Accounting.Api.Features.Accounts.Mappers;
using Ubik.ApiService.DB.Enums;

namespace Ubik.Accounting.Api.Features.Accounts.Commands
{
    public class UpdateAccount
    {
        //Input
        public record UpdateAccountCommand : IRequest<UpdateAccountResult>
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
            [JsonRequired]
            [EnumDataType(typeof(AccountCategory))]
            public AccountCategory Category { get; set; }
            [JsonRequired]
            [EnumDataType(typeof(AccountDomain))]
            public AccountDomain Domain { get; set; }
            [MaxLength(700)]
            public string? Description { get; set; }
            [Required]
            public Guid Version { get; set; }
            [Required]
            public Guid CurrencyId { get; set; }
        }

        //Output
        public record UpdateAccountResult
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


        public class UpdateAccountHandler : IRequestHandler<UpdateAccountCommand, UpdateAccountResult>
        {
            private readonly IServiceManager _serviceManager;
            private readonly IPublishEndpoint _publishEndpoint;

            public UpdateAccountHandler(IServiceManager serviceManager, IPublishEndpoint publishEndpoint)
            {
                _serviceManager = serviceManager;
                _publishEndpoint = publishEndpoint;
            }

            public async Task<UpdateAccountResult> Handle(UpdateAccountCommand request, CancellationToken cancellationToken)
            {
                //Check if the account code already exists in other records
                bool exists = await _serviceManager.AccountService.IfExistsWithDifferentIdAsync(request.Code, request.Id);
                //TOCHANGE
                //if (exists)
                //    throw new AccountAlreadyExistsException(request.Code);

                //Check if the account is found
                var account = await _serviceManager.AccountService.GetAsync(request.Id) 
                                ?? throw new AccountNotFoundException(request.Id);

                //Check if the specified currency exists
                var curExists = await _serviceManager.AccountService.IfExistsCurrencyAsync(request.CurrencyId);
                if (!curExists)
                    throw new AccountCurrencyNotFoundException(request.CurrencyId);

                //Modify the found account
                account = request.ToAccount(account);

                //Store and publish
                var result = _serviceManager.AccountService.Update(account);
                await _publishEndpoint.Publish(account.ToAccountUpdated(), CancellationToken.None);
                await _serviceManager.SaveAsync();

                return result.ToUpdateAccountResult();
            }
        }
    }
}
