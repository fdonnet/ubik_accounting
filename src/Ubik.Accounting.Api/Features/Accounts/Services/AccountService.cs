using LanguageExt;
using LanguageExt.Common;
using Microsoft.EntityFrameworkCore;
using Ubik.Accounting.Api.Data;
using Ubik.Accounting.Api.Features.Accounts.Exceptions;
using Ubik.Accounting.Api.Features.Accounts.Mappers;
using Ubik.Accounting.Api.Models;
using Ubik.ApiService.Common.Exceptions;

namespace Ubik.Accounting.Api.Features.Accounts.Services
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

        public async Task<ResultT<Account>> GetAsync(Guid id)
        {
            var account = await _context.Accounts.FirstOrDefaultAsync(a => a.Id == id);

            return account == null
                ? new ResultT<Account>() {IsSuccess = false, Exception = new AccountNotFoundException(id)}
                : new ResultT<Account>() {IsSuccess = true, Result = account };
        }

        public async Task<bool> IfExistsAsync(string accountCode)
        {
            return await _context.Accounts.AnyAsync(a => a.Code == accountCode);
        }

        public async Task<bool> IfExistsWithDifferentIdAsync(string accountCode, Guid currentId)
        {
            return await _context.Accounts.AnyAsync(a => a.Code == accountCode && a.Id != currentId);
        }

        public async Task<ResultT<Account>> AddAsync(Account account)
        {
            var accountExists = await IfExistsAsync(account.Code);
            if (accountExists)
                return new ResultT<Account>() { IsSuccess = false, Exception = new AccountAlreadyExistsException(account.Code) };

            var curExists = await IfExistsCurrencyAsync(account.CurrencyId);
            if (!curExists)
                return new ResultT<Account>() { IsSuccess = false, Exception = new AccountCurrencyNotFoundException(account.CurrencyId) };

            await _context.Accounts.AddAsync(account);
            _context.SetAuditAndSpecialFields();

            return new ResultT<Account>() { IsSuccess = true, Result = account };
        }

        public async Task<ResultT<Account>> UpdateAsync(Account account)
        {
            //Check if the account is found
            var accountPresent = await GetAsync(account.Id);
            if(!accountPresent.IsSuccess)
                return accountPresent;

            var accountToUpd = accountPresent.Result;

            //check if the account code already exists in other records
            bool exists = await IfExistsWithDifferentIdAsync(account.Code, account.Id);
            if (exists)
                return new ResultT<Account>() { IsSuccess = false, Exception = new AccountAlreadyExistsException(account.Code) };

            //check if the specified currency exists
            var curexists = await IfExistsCurrencyAsync(account.CurrencyId);
            if (!curexists)
                return new ResultT<Account>() { IsSuccess = false, Exception = new AccountCurrencyNotFoundException(account.CurrencyId) };

            accountToUpd = account.ToAccount(accountToUpd);

            _context.Entry(accountToUpd).State = EntityState.Modified;
            _context.SetAuditAndSpecialFields();

            return new ResultT<Account>() { IsSuccess = true, Result=accountToUpd };
        }

        public async Task<ResultT<bool>> ExecuteDeleteAsync(Guid id)
        {
            var account = await GetAsync(id);

            if(account.IsSuccess)
            {
                await _context.Accounts.Where(x => x.Id == id).ExecuteDeleteAsync();
                return new ResultT<bool>() { IsSuccess = true, Result = true };
            }
            else
            {
                return new ResultT<bool>() { IsSuccess = false, Exception = new AccountNotFoundException(id) };
            }
        }

        public async Task<bool> IfExistsCurrencyAsync(Guid currencyId)
        {
            return await _context.Currencies.AnyAsync(c => c.Id == currencyId);
        }
    }
}
