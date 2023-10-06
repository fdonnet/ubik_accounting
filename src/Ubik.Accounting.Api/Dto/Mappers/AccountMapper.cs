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
                AccountGroupId = account.AccountGroupId
            };
        }

        public static Account ToAccount(this AccountDto accountDto)
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

        public static AccountWithAccountGroupDto ToAccountWithAccountGroup(this Account account)
        {
            return new AccountWithAccountGroupDto
            {
                Id = account.Id,
                Code = account.Code,
                Label = account.Label,
                Description = account.Description,
                AccountGroupId = account.AccountGroupId,
                Group = account.Group != null ? AccountGroupMapper.ToAccountGroupDto(account.Group) : null
        };
        }
    }
}
