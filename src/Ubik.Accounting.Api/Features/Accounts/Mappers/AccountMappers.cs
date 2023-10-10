using Ubik.Accounting.Api.Dto;
using Ubik.Accounting.Api.Models;
using static Ubik.Accounting.Api.Features.Accounts.Commands.AddAccount;
using static Ubik.Accounting.Api.Features.Accounts.Queries.GetAccount;

namespace Ubik.Accounting.Api.Features.Accounts.Mappers
{
    public static class AccountMappers
    {
        public static AddAccountResult ToAddAccountResult(this Account account)
        {
            return new AddAccountResult()
            {
                Id = account.Id,
                Code = account.Code,
                Label = account.Label,
                Description = account.Description,
                AccountGroupId = account.AccountGroupId,
                Version = account.Version
            };
        }

        public static GetAccountResult ToGetAccountResult(this Account account)
        {
            return new GetAccountResult()
            {
                Id = account.Id,
                Code = account.Code,
                Label = account.Label,
                Description = account.Description,
                AccountGroupId = account.AccountGroupId,
                Version = account.Version
            };
        }

        //public static Account ToAccount(this AccountDto accountDto, Account? account = null)
        //{
        //    if (account == null)
        //    {
        //        return new Account()
        //        {
        //            Id = accountDto.Id,
        //            Code = accountDto.Code,
        //            Label = accountDto.Label,
        //            Description = accountDto.Description,
        //            AccountGroupId = accountDto.AccountGroupId
        //        };
        //    }
        //    else
        //    {
        //        account.Id = accountDto.Id;
        //        account.Code = accountDto.Code;
        //        account.Label = accountDto.Label;
        //        account.Description = accountDto.Description;
        //        account.AccountGroupId = accountDto.AccountGroupId;
        //        account.Version = accountDto.Version;
        //        return account;
        //    }
        //}

        public static Account ToAccount(this AddAccountCommand addAccountCommand)
        {
            return new Account()
            {
                Id = Guid.NewGuid(),
                Code = addAccountCommand.Code,
                Label = addAccountCommand.Label,
                Description = addAccountCommand.Description,
                AccountGroupId = addAccountCommand.AccountGroupId
            };
        }
    }
}
