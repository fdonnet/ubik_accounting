using LanguageExt.Common;
using Microsoft.EntityFrameworkCore;
using Ubik.Accounting.Api.Data;
using Ubik.Accounting.Api.Dto;
using Ubik.Accounting.Api.Dto.Mappers;
using Ubik.Accounting.Api.Models;
using Ubik.Accounting.Api.Features.Accounts.Exceptions;
using Ubik.ApiService.Common.Exceptions;

namespace Ubik.Accounting.Api.Features.Accounts
{
    public class AccountService : IAccountService
    {
        private readonly AccountingContext _context;
        public AccountService(AccountingContext ctx)
        {
            _context = ctx;

        }
        public async Task<IEnumerable<Account>> GetAccountsAsync()
        {
            var accounts = await _context.Accounts.ToListAsync();

            return accounts;
        }

        public async Task<Account?> GetAccountAsync(Guid id)
        {
            var account = await _context.Accounts.FirstOrDefaultAsync(a => a.Id == id);
            return account;
        }

        public async Task<bool> IfExists(string accountCode)
        {
            return await _context.Accounts.AnyAsync(a => a.Code == accountCode);
        }

        public async Task<bool> IfExistsWithDifferentId(string accountCode, Guid id)
        {
            return await _context.Accounts.AnyAsync(a => a.Code == accountCode && a.Id != id);
        }

        public async Task<Account> AddAccountAsync(Account account)
        {
            await _context.Accounts.AddAsync(account);
            await _context.SaveChangesAsync();

            return account;
        }

        /// <summary>
        /// Take care, can raise a Concurrency exception as ServiceAndFeatureException
        /// </summary>
        /// <param name="account"></param>
        /// <returns>a bool or throw AccountUpdateDbConcurrencyException if a DbUpdateConcurrencyException occurs</returns>
        public async Task<bool> UpdateAccountAsync(Account account)
        {
            _context.Entry(account).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                var conflict = new AccountUpdateDbConcurrencyException("DbUpdateConcurrencyException", ex)
                {
                    ErrorCode = "ACCOUNT_CONFLICT",
                    ErrorFriendlyMessage = "You don't have the last version or this account has been removed, refresh your data before updating.",
                    ErrorValueDetails = "Version",
                };
                throw conflict;
            }

            return true;
        }

    }
}
