using Dapper;
using LanguageExt;
using LanguageExt.ClassInstances.Pred;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;
using Ubik.Accounting.Api.Data;
using Ubik.Accounting.Api.Features.Accounts.Exceptions;
using Ubik.Accounting.Api.Features.Accounts.Mappers;
using Ubik.Accounting.Api.Features.Accounts.Queries.CustomPoco;
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

        public async Task<Either<IServiceAndFeatureException, Account>> AddAsync(Account account)
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

        public async Task<Either<IServiceAndFeatureException, Account>> UpdateAsync(Account account)
        {
            return await GetAsync(account.Id).ToAsync()
                .Map(ac => ac = account.ToAccount(ac))
                .Bind(ac => ValidateIfNotAlreadyExistsWithOtherIdAsync(ac).ToAsync())
                .Bind(ac => ValidateIfCurrencyExistsAsync(ac).ToAsync())
                .Map(ac =>
                {
                    _context.Entry(ac).State = EntityState.Modified;
                    _context.SetAuditAndSpecialFields();

                    return ac;
                });
        }

        public async Task<Either<IServiceAndFeatureException, bool>> ExecuteDeleteAsync(Guid id)
        {
            return await GetAsync(id).ToAsync()
                .MapAsync(async ac =>
                {
                    await _context.Accounts.Where(x => x.Id == id).ExecuteDeleteAsync();
                    return true;
                });
        }

        public async Task<Either<IServiceAndFeatureException, AccountAccountGroup>> AddInAccountGroupAsync(Guid id, Guid accountGroupId)
        {
            //Create new relation
            var accountAccountGroup = new AccountAccountGroup
            {
                Id = NewId.NextGuid(),
                AccountGroupId = accountGroupId,
                AccountId = id
            };

            //Validate and insert
            return await GetAsync(id).ToAsync()
                .Bind(a => ValidateIfExistsAccountGroupIdAsync(accountAccountGroup).ToAsync())
                .Bind(aag => ValidateIfNotExistsInTheClassificationAsync(aag).ToAsync())
                .MapAsync(async aag =>
                {
                    await _context.AccountsAccountGroups.AddAsync(aag);
                    return aag;
                });
        }

        public async Task<Either<IServiceAndFeatureException, AccountAccountGroup>> DeleteFromAccountGroupAsync(Guid id, Guid accountGroupId)
        {
            return await GetExistingAccountGroupRelationAsync(id, accountGroupId).ToAsync()
                .Map(aag =>
                {
                    _context.Entry(aag).State = EntityState.Deleted;
                    return aag;
                });
        }

        public async Task<Either<IServiceAndFeatureException, IEnumerable<AccountGroupClassification>>> GetAccountGroupsWithClassificationInfoAsync(Guid id)
        {
            return await GetAsync(id).ToAsync()
                .MapAsync(async ac =>
                {
                    var p = new DynamicParameters();
                    p.Add("@id", id);
                    p.Add("@tenantId", _userService.CurrentUser.TenantIds[0]);

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

        private async Task<Either<IServiceAndFeatureException, Account>> ValidateIfNotAlreadyExistsAsync(Account account)
        {
            var exists = await _context.Accounts.AnyAsync(a => a.Code == account.Code);
            return exists
                ? new AccountAlreadyExistsException(account.Code)
                : account;
        }

        private async Task<Either<IServiceAndFeatureException, Account>> ValidateIfNotAlreadyExistsWithOtherIdAsync(Account account)
        {
            var exists = await _context.Accounts.AnyAsync(a => a.Code == account.Code && a.Id != account.Id);

            return exists
                ? new AccountAlreadyExistsException(account.Code)
                : account;
        }

        private async Task<Either<IServiceAndFeatureException, Account>> ValidateIfCurrencyExistsAsync(Account account)
        {
            return await _context.Currencies.AnyAsync(c => c.Id == account.CurrencyId)
                ? account
                : new AccountCurrencyNotFoundException(account.CurrencyId);
        }

        private async Task<Either<IServiceAndFeatureException, AccountAccountGroup>> GetExistingAccountGroupRelationAsync(Guid id, Guid accountGroupId)
        {
            var accountAccountGroup = await _context.AccountsAccountGroups.FirstOrDefaultAsync(aag =>
                aag.AccountId == id
                && aag.AccountGroupId == accountGroupId);

            return accountAccountGroup == null ? (Either<IServiceAndFeatureException, AccountAccountGroup>)new AccountNotExistsInAccountGroupException(id, accountGroupId) : (Either<IServiceAndFeatureException, AccountAccountGroup>)accountAccountGroup;
        }

        private async Task<Either<IServiceAndFeatureException, AccountAccountGroup>> ValidateIfNotExistsInTheClassificationAsync(AccountAccountGroup accountAccountGroup)
        {
            var p = new DynamicParameters();
            p.Add("@id", accountAccountGroup.AccountId);
            p.Add("@accountGroupId", accountAccountGroup.AccountGroupId);
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

            return (await con.QueryFirstOrDefaultAsync<Account>(sql, p)) != null
                ? new AccountAlreadyExistsInClassificationException(accountAccountGroup.AccountId, accountAccountGroup.AccountGroupId)
                : accountAccountGroup;

        }

        private async Task<Either<IServiceAndFeatureException, AccountAccountGroup>> ValidateIfExistsAccountGroupIdAsync(AccountAccountGroup accountAccountGroup)
        {
            return await _context.AccountGroups.AnyAsync(ag => ag.Id == accountAccountGroup.AccountGroupId)
                ? accountAccountGroup
                : new AccountGroupNotFoundForAccountException(accountAccountGroup.AccountGroupId);
        }
    }
}
