using Ubik.Accounting.Api.Models;

namespace Ubik.Accounting.Api.Dto.Mappers
{
    public static class AccountMapper
    {
        public static AccountDto ToAccountDto(this Account account)
        {
            return new AccountDto()
            {
                Id = account.Id,
                Code = account.Code,
                Label = account.Label,
                Description = account.Description,
                AccountGroupId = account.AccountGroupId,
                Version = account.Version
            };
        }

        public static Account ToAccount(this AccountDto accountDto,  Account? account = null )
        {
            if(account == null )
            {
                return new Account()
                {
                    Id = accountDto.Id,
                    Code = accountDto.Code,
                    Label = accountDto.Label,
                    Description = accountDto.Description,
                    AccountGroupId = accountDto.AccountGroupId
                };
            }
            else
            {
                account.Id = accountDto.Id;
                account.Code = accountDto.Code;
                account.Label = accountDto.Label;
                account.Description = accountDto.Description;
                account.AccountGroupId = accountDto.AccountGroupId;
                account.Version = accountDto.Version;
                return account;
            }
        }

        public static Account ToAccount(this AccountDtoForAdd accountDtoForAdd)
        {
            return new Account()
            {
                Id = Guid.NewGuid(),
                Code = accountDtoForAdd.Code,
                Label = accountDtoForAdd.Label,
                Description = accountDtoForAdd.Description,
                AccountGroupId = accountDtoForAdd.AccountGroupId
            };
        }
    }
}
