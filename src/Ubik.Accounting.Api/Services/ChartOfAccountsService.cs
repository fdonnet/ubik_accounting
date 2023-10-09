using LanguageExt.Common;
using LanguageExt.Pipes;
using Microsoft.EntityFrameworkCore;
using Ubik.Accounting.Api.Data;
using Ubik.Accounting.Api.Dto;
using Ubik.Accounting.Api.Dto.Mappers;
using Ubik.Accounting.Api.Models;
using Ubik.ApiService.Common.Exceptions;

namespace Ubik.Accounting.Api.Services
{
    public interface IChartOfAccountsService
    {
        public Task<IEnumerable<AccountDto>> GetAccountsAsync();
        public Task<Result<AccountDto>> GetAccountAsync(Guid Id);
        public Task<Result<Account>> AddAccountAsync(AccountDtoForAdd account);
        public Task<Result<bool>> UpdateAccountAsync(Guid currentId, AccountDto account);
    }

    public class ChartOfAccountsService : IChartOfAccountsService
    {
        private readonly AccountingContext _context;
        public ChartOfAccountsService(AccountingContext ctx)
        {
            _context = ctx;
        }

        public async Task<IEnumerable<AccountDto>> GetAccountsAsync()
        {
            var accounts = await _context.Accounts.ToListAsync();

            return accounts.Select(a => AccountMapper.ToAccountDto(a));
        }

        public async Task<Result<AccountDto>> GetAccountAsync(Guid Id)
        {
            var account = await _context.Accounts.SingleOrDefaultAsync(a => a.Id == Id);

            if (account == null)
            {
                var notExistsForUpdate = new ServiceAndFeatureException()
                {
                    ErrorCode = "ACCOUNT_NOT_FOUND",
                    ExceptionType = ServiceAndFeatureExceptionType.NotFound,
                    ErrorFriendlyMessage = "The account doesn't exist. Id not found.",
                    ErrorValueDetails = "Id",
                };
                return new Result<AccountDto>(notExistsForUpdate);
            }
            return account.ToAccountDto();
        }

        public async Task<Result<Account>> AddAccountAsync(AccountDtoForAdd accountDto)
        {
            var account = AccountMapper.ToAccount(accountDto);

            bool exists = await _context.Accounts.AnyAsync(a => a.Code == accountDto.Code);

            if (exists)
            {
                var alreadyExists = new ServiceAndFeatureException()
                {
                    ErrorCode = "ACCOUNT_ALREADY_EXISTS",
                    ExceptionType = ServiceAndFeatureExceptionType.Conflict,
                    ErrorFriendlyMessage = "The account already exists. Code field needs to be unique.",
                    ErrorValueDetails = "Code"
                };
                return new Result<Account>(alreadyExists);
            }

            await _context.Accounts.AddAsync(account);
            await _context.SaveChangesAsync();

            return account;
        }

        public async Task<Result<bool>> UpdateAccountAsync(Guid currentId, AccountDto accountDto)
        {
            //Not correct ID
            if (currentId != accountDto.Id)
            {
                var notSameId = new ServiceAndFeatureException()
                {
                    ErrorCode = "ACCOUNT_ID_NOTMATCH",
                    ExceptionType = ServiceAndFeatureExceptionType.BadParams,
                    ErrorFriendlyMessage = "The account Id provided doesn't match the account Id information as payload sent for update.",
                    ErrorValueDetails = "Id"
                };
                return new Result<bool>(notSameId);
            }

            //Check if the account code already exists in other records
            bool exists = await _context.Accounts.AnyAsync(a => a.Code == accountDto.Code && a.Id != currentId);
            if (exists)
            {
                var alreadyExists = new ServiceAndFeatureException()
                {
                    ErrorCode = "ACCOUNT_ALREADY_EXISTS",
                    ExceptionType = ServiceAndFeatureExceptionType.Conflict,
                    ErrorFriendlyMessage = "The account already exists. Code field needs to be unique.",
                    ErrorValueDetails = "Code",
                };
                return new Result<bool>(alreadyExists);
            }

            //Check if the record exists
            var accountToUpdate = await _context.Accounts.SingleOrDefaultAsync(x => x.Id == currentId);

            if (accountToUpdate == null)
            {
                var notExistsForUpdate = new ServiceAndFeatureException()
                {
                    ErrorCode = "ACCOUNT_NOT_FOUND",
                    ExceptionType = ServiceAndFeatureExceptionType.NotFound,
                    ErrorFriendlyMessage = "The account doesn't exist. Id not found.",
                    ErrorValueDetails = "Id",
                };
                return new Result<bool>(notExistsForUpdate);
            }

            accountToUpdate = accountDto.ToAccount(accountToUpdate);

            _context.Entry(accountToUpdate).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                var conflict = new ServiceAndFeatureException()
                {
                    ErrorCode = "ACCOUNT_MODIFIED",
                    ExceptionType = ServiceAndFeatureExceptionType.Conflict,
                    ErrorFriendlyMessage = "You don't have the last version or this account has been removed, refresh your data before updating.",
                    ErrorValueDetails = "Version",
                };
                return new Result<bool>(conflict);
            }

            return true;
        }
    }
}
