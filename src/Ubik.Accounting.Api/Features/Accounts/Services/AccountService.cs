using LanguageExt;
using MassTransit;
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

        public async Task<Either<IServiceAndFeatureException, Account>> GetAsync(Guid id)
        {
            var account = await _context.Accounts.FirstOrDefaultAsync(a => a.Id == id);

            return account == null
                ? new AccountNotFoundException(id)
                : account;
        }

        public async Task<bool> IfExistsAsync(string accountCode)
        {
            return await _context.Accounts.AnyAsync(a => a.Code == accountCode);
        }

        public async Task<bool> IfExistsWithDifferentIdAsync(string accountCode, Guid currentId)
        {
            return await _context.Accounts.AnyAsync(a => a.Code == accountCode && a.Id != currentId);
        }

        public async Task<Either<IServiceAndFeatureException, Account>> AddAsync(Account account)
        {
            var accountExists = await IfExistsAsync(account.Code);
            if (accountExists)
                return new AccountAlreadyExistsException(account.Code);

            var curExists = await IfExistsCurrencyAsync(account.CurrencyId);
            if (!curExists)
                return new AccountCurrencyNotFoundException(account.CurrencyId);

            account.Id = NewId.NextGuid();
            await _context.Accounts.AddAsync(account);
            _context.SetAuditAndSpecialFields();

            return account;
        }

        public async Task<Either<IServiceAndFeatureException, Account>> UpdateAsync(Account account)
        {
            //Check if the account is found
            var accountPresent = await GetAsync(account.Id);
            if(accountPresent.IsLeft)
                return accountPresent;

            var accountToUpd = accountPresent.IfLeft(ac => default!);

            //check if the account code already exists in other records
            bool exists = await IfExistsWithDifferentIdAsync(account.Code, account.Id);
            if (exists)
                return new AccountAlreadyExistsException(account.Code);

            //check if the specified currency exists
            var curexists = await IfExistsCurrencyAsync(account.CurrencyId);
            if (!curexists)
                return new AccountCurrencyNotFoundException(account.CurrencyId);

            accountToUpd = account.ToAccount(accountToUpd);

            _context.Entry(accountToUpd).State = EntityState.Modified;
            _context.SetAuditAndSpecialFields();

            return accountToUpd;
        }

        public async Task<Either<IServiceAndFeatureException, bool>> ExecuteDeleteAsync(Guid id)
        {
            var account = await GetAsync(id);

            if(account.IsRight)
            {
                await _context.Accounts.Where(x => x.Id == id).ExecuteDeleteAsync();
                return true;
            }
            else
                return new AccountNotFoundException(id);
        }

        public async Task<bool> IfExistsCurrencyAsync(Guid currencyId)
        {
            return await _context.Currencies.AnyAsync(c => c.Id == currencyId);
        }

        //public async Task<Either<IServiceAndFeatureException, AccountAccountGroup>> AddToAccountGroupAsync(Guid id, Guid accountGroupId)
        //{
        //    var accountPresent = await GetAsync(id);
        //    if (accountPresent.IsLeft)
        //        return new AccountNotFoundException(id);

        //    if (!(await IfExistAccountGroupAsync(accountGroupId)))
        //        return new AccountGroupNotFoundForAccountException(accountGroupId);


        //}

        //private async Task<bool> IfExistsInTheClassification(Guid id, Guid classificationId)
        //{

        //}

        //private async Task<bool> IfExistAccountGroupAsync(Guid accountGroupId)
        //{
        //    return  await _context.AccountGroups.AnyAsync(ag => ag.Id == accountGroupId);
        //}
    }
}
