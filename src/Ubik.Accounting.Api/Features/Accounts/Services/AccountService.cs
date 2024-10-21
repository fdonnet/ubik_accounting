using Dapper;
using LanguageExt;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Ubik.Accounting.Api.Data;
using Ubik.Accounting.Api.Features.Accounts.CustomPoco;
using Ubik.Accounting.Api.Features.Accounts.Errors;
using Ubik.Accounting.Api.Features.Accounts.Mappers;
using Ubik.Accounting.Api.Models;
using Ubik.ApiService.Common.Errors;
using Ubik.ApiService.Common.Services;

namespace Ubik.Accounting.Api.Features.Accounts.Services
{
    public class AccountService(AccountingDbContext ctx, ICurrentUser currentUser) : IAccountService
    {
        private readonly AccountingDbContext _context = ctx;
        private readonly ICurrentUser _currentUser = currentUser;

        public async Task<IEnumerable<Account>> GetAllAsync()
        {
            var accounts = await _context.Accounts.ToListAsync();

            return accounts;
        }

        public async Task<Either<IServiceAndFeatureError, Account>> GetAsync(Guid id)
        {
            var account = await _context.Accounts.FindAsync(id);

            return account == null
                ? new ResourceNotFoundError("Account", "Id", id.ToString())
                : account;
        }

        public async Task<Either<IServiceAndFeatureError, Account>> AddAsync(Account account)
        {
            return await ValidateIfNotAlreadyExistsAsync(account).ToAsync()
                .Bind(ac => ValidateIfCurrencyExistsAsync(ac).ToAsync()
                .MapAsync(async ac =>
                {
                    ac.Id = NewId.NextGuid();
                    await _context.Accounts.AddAsync(ac);
                    _context.SetAuditAndSpecialFields();
                    return ac;
                }));
        }

        //public async Task<Either<IServiceAndFeatureError, Account>> UpdateAsync(Account account)
        //{
        //    return await GetAsync(account.Id).ToAsync()
        //        .Map(ac => ac = account.ToAccount(ac))
        //        .Bind(ac => ValidateIfNotAlreadyExistsWithOtherIdAsync(ac).ToAsync())
        //        .Bind(ac => ValidateIfCurrencyExistsAsync(ac).ToAsync())
        //        .Map(ac =>
        //        {
        //            _context.Entry(ac).State = EntityState.Modified;
        //            _context.SetAuditAndSpecialFields();

        //            return ac;
        //        });
        //}

        public async Task<Either<IServiceAndFeatureError, bool>> ExecuteDeleteAsync(Guid id)
        {
            return await GetAsync(id).ToAsync()
                .MapAsync(async ac =>
                {
                    await _context.Accounts.Where(x => x.Id == id).ExecuteDeleteAsync();
                    return true;
                });
        }

        public async Task<Either<IServiceAndFeatureError, AccountAccountGroup>> AddInAccountGroupAsync(AccountAccountGroup accountAccountGroup)
        {
            //Validate and insert
            return await GetAsync(accountAccountGroup.AccountId).ToAsync()
                .Bind(a => ValidateIfExistsAccountGroupIdAsync(accountAccountGroup).ToAsync())
                .Bind(aag => ValidateIfNotExistsInTheClassificationAsync(aag).ToAsync())
                .MapAsync(async aag =>
                {
                    await _context.AccountsAccountGroups.AddAsync(aag);
                    _context.SetAuditAndSpecialFields();
                    return aag;
                });
        }

        public async Task<Either<IServiceAndFeatureError, AccountAccountGroup>> DeleteFromAccountGroupAsync(Guid id, Guid accountGroupId)
        {
            return await GetExistingAccountGroupRelationAsync(id, accountGroupId).ToAsync()
                .Map(aag =>
                {
                    _context.Entry(aag).State = EntityState.Deleted;
                    return aag;
                });
        }

        public async Task<Either<IServiceAndFeatureError, IEnumerable<AccountGroupClassification>>> GetAccountGroupsWithClassificationInfoAsync(Guid id)
        {
            return await GetAsync(id).ToAsync()
                .MapAsync(async ac =>
                {
                    var p = new DynamicParameters();
                    p.Add("@id", id);
                    p.Add("@tenantId", _currentUser.TenantId);

                    var con = _context.Database.GetDbConnection();

                    var sql = """
                SELECT ag.id
                , ag.code
                , ag.label
                , c.id as classification_id
                , c.code as classification_code
                , c.label as classification_label
                FROM account_groups ag
                INNER JOIN classifications c ON ag.classification_id = c.id
                INNER JOIN accounts_account_groups aag ON aag.account_group_id = ag.id
                WHERE ag.tenant_id = @tenantId
                AND aag.account_id = @id
                """;

                    return await con.QueryAsync<AccountGroupClassification>(sql, p);
                });
        }

        private async Task<Either<IServiceAndFeatureError, Account>> ValidateIfNotAlreadyExistsAsync(Account account)
        {
            var exists = await _context.Accounts.AnyAsync(a => a.Code == account.Code);
            return exists
                ? new ResourceAlreadyExistsError("Account","Code",account.Code)
                : account;
        }

        private async Task<Either<IServiceAndFeatureError, Account>> ValidateIfNotAlreadyExistsWithOtherIdAsync(Account account)
        {
            var exists = await _context.Accounts.AnyAsync(a => a.Code == account.Code && a.Id != account.Id);

            return exists
                ? new ResourceAlreadyExistsError("Account", "Code", account.Code)
                : account;
        }

        private async Task<Either<IServiceAndFeatureError, Account>> ValidateIfCurrencyExistsAsync(Account account)
        {
            return await _context.Currencies.AnyAsync(c => c.Id == account.CurrencyId)
                ? account
                : new BadParamExternalResourceNotFound("Account","Currency", "CurrencyId", account.CurrencyId.ToString());
        }

        private async Task<Either<IServiceAndFeatureError, AccountAccountGroup>> GetExistingAccountGroupRelationAsync(Guid id, Guid accountGroupId)
        {
            var accountAccountGroup = await _context.AccountsAccountGroups.FirstOrDefaultAsync(aag =>
                aag.AccountId == id
                && aag.AccountGroupId == accountGroupId);

            return accountAccountGroup == null ?
                new AccountNotExistsInAccountGroupError(id, accountGroupId)
                : accountAccountGroup;
        }

        private async Task<Either<IServiceAndFeatureError, AccountAccountGroup>> ValidateIfNotExistsInTheClassificationAsync(AccountAccountGroup accountAccountGroup)
        {
            var p = new DynamicParameters();
            p.Add("@id", accountAccountGroup.AccountId);
            p.Add("@accountGroupId", accountAccountGroup.AccountGroupId);
            p.Add("@tenantId", _currentUser.TenantId);

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

            return (await con.QueryFirstOrDefaultAsync<Account>(sql, p)) != null
                ? new AccountAlreadyExistsInClassificationError(accountAccountGroup.AccountId, accountAccountGroup.AccountGroupId)
                : accountAccountGroup;

        }

        private async Task<Either<IServiceAndFeatureError, AccountAccountGroup>> ValidateIfExistsAccountGroupIdAsync(AccountAccountGroup accountAccountGroup)
        {
            return await _context.AccountGroups.AnyAsync(ag => ag.Id == accountAccountGroup.AccountGroupId)
                ? accountAccountGroup
                : new BadParamExternalResourceNotFound("Account","AccountGroup", "AccountGroupId", accountAccountGroup.AccountGroupId.ToString());
        }

        public async Task<IEnumerable<AccountAccountGroup>> GetAllAccountGroupLinksAsync()
        {
            var results = await _context.AccountsAccountGroups.ToListAsync();

            return results;
        }
    }
}
