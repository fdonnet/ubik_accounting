using Microsoft.EntityFrameworkCore;
using Ubik.Accounting.Api.Data;
using Ubik.Accounting.Api.Models;

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

        public async Task<Account> AddAsync(Account account)
        {
            await _context.Accounts.AddAsync(account);
            _context.SetAuditAndSpecialFields();

            return account;
        }

        /// <summary>
        /// Take care, can raise a Concurrency exception as ServiceAndFeatureException
        /// </summary>
        /// <param name="account"></param>
        /// <returns>a bool or throw AccountUpdateDbConcurrencyException if a DbUpdateConcurrencyException occurs</returns>
        public Account Update(Account account)
        {
            _context.Entry(account).State = EntityState.Modified;
            _context.SetAuditAndSpecialFields();

            return account;
        }

        public async Task<bool> ExecuteDeleteAsync(Guid id)
        {
            await _context.Accounts.Where(x => x.Id == id).ExecuteDeleteAsync();
            return true;
        }

        public async Task<bool> IfExistsCurrencyAsync(Guid currencyId)
        {
            return await _context.Currencies.AnyAsync(c => c.Id == currencyId);
        }
    }
}
