using Microsoft.EntityFrameworkCore;
using Ubik.Accounting.Api.Data;
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
        public async Task<IEnumerable<Account>> GetAllAsync()
        {
            var accounts = await _context.Accounts.ToListAsync();

            return accounts;
        }

        public async Task<Account?> GetAsync(Guid id)
        {
            var account = await _context.Accounts.FirstOrDefaultAsync(a => a.Id == id);
            return account;
        }

        public async Task<bool> IfExistsAsync(string accountCode)
        {
            return await _context.Accounts.AnyAsync(a => a.Code == accountCode);
        }

        public async Task<bool> IfExistsWithDifferentIdAsync(string accountCode, Guid currentId)
        {
            return await _context.Accounts.AnyAsync(a => a.Code == accountCode && a.Id != currentId);
        }

        public async Task<bool> IfExistsAccountGroupAsync(Guid accountGroupId)
        {
            return await _context.AccountGroups.AnyAsync(g=> g.Id == accountGroupId);
        }

        public async Task<Account> AddAsync(Account account)
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
        public async Task<Account> UpdateAsync(Account account)
        {
            _context.Entry(account).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return account;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                var err = new CustomError()
                {
                    ErrorCode = "ACCOUNT_CONFLICT",
                    ErrorFriendlyMessage = "You don't have the last version or this account has been removed, refresh your data before updating.",
                    ErrorValueDetails = "Version",
                };
                var conflict = new AccountUpdateDbConcurrencyException(account.Version, ex)
                {
                    CustomErrors = new List<CustomError> { err }
                };

                throw conflict;
            }
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            _context.Accounts.Where(x => x.Id == id).ExecuteDelete();
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
