using Dapper;
using LanguageExt;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Ubik.Accounting.Api.Data;
using Ubik.Accounting.Api.Features.Accounts.Exceptions;
using Ubik.Accounting.Api.Features.Accounts.Mappers;
using Ubik.Accounting.Api.Features.Classifications.Exceptions;
using Ubik.Accounting.Api.Models;
using Ubik.ApiService.Common.Exceptions;
using Ubik.ApiService.Common.Services;

namespace Ubik.Accounting.Api.Features.Accounts.Services
{
    public class AccountService : IAccountService
    {
        private readonly AccountingContext _context;
        private readonly ICurrentUserService _userService;
        public AccountService(AccountingContext ctx, ICurrentUserService userService)
        {
            _context = ctx;
            _userService = userService;
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

        public async Task<Either<IServiceAndFeatureException, AccountAccountGroup>> AddInAccountGroupAsync(Guid id, Guid accountGroupId)
        {
            var accountPresent = await GetAsync(id);
            if (accountPresent.IsLeft)
                return new AccountNotFoundException(id);

            if (!(await IfExistAccountGroupAsync(accountGroupId)))
                return new AccountGroupNotFoundForAccountException(accountGroupId);

            if (await IfExistsInTheClassification(id, accountGroupId))
                return new AccountAlreadyExistsInClassificationException(id, accountGroupId);

            var accountAccountGroup = new AccountAccountGroup
            {
                Id = NewId.NextGuid(),
                AccountGroupId = accountGroupId,
                AccountId = id
            };

            await _context.AccountsAccountGroups.AddAsync(accountAccountGroup);
            return accountAccountGroup;
        }

        public async Task<Either<IServiceAndFeatureException, AccountAccountGroup>> DeleteFromAccountGroupAsync(Guid id, Guid accountGroupId)
        {
            var accountAccountGroup = await _context.AccountsAccountGroups.FirstOrDefaultAsync(aag =>
                aag.AccountId == id
                && aag.AccountGroupId == accountGroupId);

            if (accountAccountGroup == null)
                return new AccountNotExistsInAccountGroup(id, accountGroupId);

            _context.Entry(accountAccountGroup).State = EntityState.Deleted;

            return accountAccountGroup;
        }

        private async Task<bool> IfExistsInTheClassification(Guid id, Guid accountGroupId)
        {
            var p = new DynamicParameters();
            p.Add("@id", id);
            p.Add("@accountGroupId", accountGroupId);
            p.Add("@tenantId", _userService.CurrentUser.TenantIds[0]);

            var con = _context.Database.GetDbConnection();
            var sql = """
                SELECT a.id
                FROM account_groups ag 
                INNER JOIN accounts_account_groups aag on aag.account_group_id = ag.id
                INNER JOIN accounts a ON aag.account_id = a.id
                WHERE a.tenant_id = @tenantId
                AND a.id = @id
                AND ag.classification_id = (SELECT c1.id
                						   	FROM classifications c1
                						 	INNER JOIN account_groups ag1 ON ag1.classification_id = c1.id
                						   	WHERE ag1.id = @accountGroupId)
                """;

            return (await con.QueryFirstOrDefaultAsync<Account>(sql, p)) == null ? false : true;
        }

        private async Task<bool> IfExistAccountGroupAsync(Guid accountGroupId)
        {
            return await _context.AccountGroups.AnyAsync(ag => ag.Id == accountGroupId);
        }


    }
}
