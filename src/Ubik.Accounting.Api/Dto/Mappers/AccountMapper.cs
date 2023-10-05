using System.ComponentModel.DataAnnotations;
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
                AccountGroupId  = account.AccountGroupId
            };
        }
    }
}
