using Dapper;
using LanguageExt;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Ubik.Accounting.Structure.Api.Data;
using Ubik.Accounting.Structure.Api.Features.Accounts.Errors;
using Ubik.Accounting.Structure.Api.Mappers;
using Ubik.Accounting.Structure.Api.Models;
using Ubik.Accounting.Structure.Contracts.Accounts.Commands;
using Ubik.Accounting.Structure.Contracts.Accounts.Events;
using Ubik.ApiService.Common.Errors;
using Ubik.ApiService.Common.Exceptions;
using Ubik.ApiService.Common.Services;

namespace Ubik.Accounting.Structure.Api.Features.Accounts.Services
{
    public class AccountCommandService(AccountingDbContext ctx, ICurrentUser currentUser, IPublishEndpoint publishEndpoint) : IAccountCommandService
    {
        public async Task<Either<IServiceAndFeatureError, Account>> AddAsync(AddAccountCommand command)
        {
            return await ValidateIfNotAlreadyExistsAsync(command.ToAccount())
                .BindAsync(ValidateIfCurrencyExistsAsync)
                .BindAsync(AddInDbContextAsync)
                .BindAsync(AddSaveAndPublishAsync);
        }

        public async Task<Either<IServiceAndFeatureError, Account>> UpdateAsync(UpdateAccountCommand command)
        {
            var model = command.ToAccount();

            return await GetAsync(model.Id)
                .BindAsync(ac => MapInDbContextAsync(ac, model))
                .BindAsync(ValidateIfNotAlreadyExistsWithOtherIdAsync)
                .BindAsync(ValidateIfCurrencyExistsAsync)
                .BindAsync(UpdateInDbContextAsync)
                .BindAsync(UpdateSaveAndPublishAsync);
        }

        public async Task<Either<IServiceAndFeatureError, bool>> DeleteAsync(Guid id)
        {
            return await GetAsync(id)
                .BindAsync(DeleteInDbContextAsync)
                .BindAsync(DeletedSaveAndPublishAsync);
        }

        public async Task<Either<IServiceAndFeatureError, AccountAccountGroup>> AddInAccountGroupAsync(AddAccountInAccountGroupCommand command)
        {
            var model = command.ToAccountAccountGroup();
            return await GetAsync(model.AccountId)
                .BindAsync(a => ValidateIfExistsAccountGroupIdAsync(model))
                .BindAsync(ValidateIfNotExistsInTheClassificationAsync)
                .BindAsync(AddAccountGroupLinkInDbContextAsync)
                .BindAsync(AddAccountGroupLinkSaveAndPublishAsync);
        }

        public async Task<Either<IServiceAndFeatureError, AccountAccountGroup>> DeleteFromAccountGroupAsync(DeleteAccountInAccountGroupCommand command)
        {
            return await GetExistingAccountGroupRelationAsync(command.AccountId, command.AccountGroupId)
                .BindAsync(DeleteAccountGroupLinkInDbContextAsync)
                .BindAsync(DeleteAccountGroupLinkSaveAndPublishAsync);
        }

        private async Task<Either<IServiceAndFeatureError, AccountAccountGroup>> DeleteAccountGroupLinkSaveAndPublishAsync(AccountAccountGroup current)
        {
            await publishEndpoint.Publish(current.ToAccountDeletedInAccountGroup(), CancellationToken.None);
            await ctx.SaveChangesAsync();

            return current;
        }

        private async Task<Either<IServiceAndFeatureError, AccountAccountGroup>> GetExistingAccountGroupRelationAsync(Guid id, Guid accountGroupId)
        {
            var accountAccountGroup = await ctx.AccountsAccountGroups.FirstOrDefaultAsync(aag =>
                aag.AccountId == id
                && aag.AccountGroupId == accountGroupId);

            return accountAccountGroup == null ?
                new AccountNotExistsInAccountGroupError(id, accountGroupId)
                : accountAccountGroup;
        }

        private async Task<Either<IServiceAndFeatureError, AccountAccountGroup>> DeleteAccountGroupLinkInDbContextAsync(AccountAccountGroup current)
        {
            ctx.Entry(current).State = EntityState.Deleted;

            await Task.CompletedTask;
            return current;
        }

        private async Task<Either<IServiceAndFeatureError, AccountAccountGroup>> AddAccountGroupLinkSaveAndPublishAsync(AccountAccountGroup current)
        {
            await publishEndpoint.Publish(current.ToAccountAddedInAccountGroup(), CancellationToken.None);
            await ctx.SaveChangesAsync();

            return current;
        }

        private async Task<Either<IServiceAndFeatureError, AccountAccountGroup>> AddAccountGroupLinkInDbContextAsync(AccountAccountGroup current)
        {
            await ctx.AccountsAccountGroups.AddAsync(current);
            ctx.SetAuditAndSpecialFields();

            return current;
        }

        private async Task<Either<IServiceAndFeatureError, AccountAccountGroup>> ValidateIfNotExistsInTheClassificationAsync(AccountAccountGroup accountAccountGroup)
        {
            var p = new DynamicParameters();
            p.Add("@id", accountAccountGroup.AccountId);
            p.Add("@accountGroupId", accountAccountGroup.AccountGroupId);
            p.Add("@tenantId", currentUser.TenantId);

            var con = ctx.Database.GetDbConnection();
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
            return await ctx.AccountGroups.AnyAsync(ag => ag.Id == accountAccountGroup.AccountGroupId)
                ? accountAccountGroup
                : new BadParamExternalResourceNotFound("Account", "AccountGroup", "AccountGroupId", accountAccountGroup.AccountGroupId.ToString());
        }

        private async Task<Either<IServiceAndFeatureError, bool>> DeletedSaveAndPublishAsync(Account current)
        {
            await publishEndpoint.Publish(new AccountDeleted { Id = current.Id }, CancellationToken.None);
            await ctx.SaveChangesAsync();

            return true;
        }

        private async Task<Either<IServiceAndFeatureError, Account>> DeleteInDbContextAsync(Account current)
        {
            ctx.Entry(current).State = EntityState.Deleted;

            await Task.CompletedTask;
            return current;
        }

        private async Task<Either<IServiceAndFeatureError, Account>> UpdateSaveAndPublishAsync(Account current)
        {
            try
            {
                await publishEndpoint.Publish(current.ToAccountUpdated(), CancellationToken.None);
                await ctx.SaveChangesAsync();

                return current;
            }
            catch (UpdateDbConcurrencyException)
            {
                return new ResourceUpdateConcurrencyError("Account", current.Version.ToString());
            }
        }

        private async Task<Either<IServiceAndFeatureError, Account>> GetAsync(Guid id)
        {
            var account = await ctx.Accounts.FindAsync(id);

            return account == null
                ? new ResourceNotFoundError("Account", "Id", id.ToString())
                : account;
        }

        private async Task<Either<IServiceAndFeatureError, Account>> UpdateInDbContextAsync(Account current)
        {
            ctx.Entry(current).State = EntityState.Modified;
            ctx.SetAuditAndSpecialFields();

            await Task.CompletedTask;
            return current;
        }

        private async Task<Either<IServiceAndFeatureError, Account>> ValidateIfNotAlreadyExistsWithOtherIdAsync(Account account)
        {
            var exists = await ctx.Accounts.AnyAsync(a => a.Code == account.Code && a.Id != account.Id);

            return exists
                ? new ResourceAlreadyExistsError("Account", "Code", account.Code)
                : account;
        }

        //private async Task<Either<IServiceAndFeatureError, Account>> ValidateIfNotLinkedToExistingEntryAsync(Account current)
        //{
        //    var exists = await ctx.Entries.AnyAsync(e => e.AccountId == current.Id);

        //    return exists
        //        ? new AccountLinkedToExistingEntriesError(current.Id)
        //        : current;
        //}

        private static async Task<Either<IServiceAndFeatureError, Account>> MapInDbContextAsync
            (Account current, Account forUpdate)
        {
            current = forUpdate.ToAccount(current);
            await Task.CompletedTask;
            return current;
        }

        private async Task<Either<IServiceAndFeatureError, Account>> AddSaveAndPublishAsync(Account account)
        {
            await publishEndpoint.Publish(account.ToAccountAdded(), CancellationToken.None);
            await ctx.SaveChangesAsync();
            return account;
        }

        private async Task<Either<IServiceAndFeatureError, Account>> AddInDbContextAsync(Account account)
        {
            account.Id = NewId.NextGuid();
            await ctx.Accounts.AddAsync(account);
            ctx.SetAuditAndSpecialFields();
            return account;
        }

        private async Task<Either<IServiceAndFeatureError, Account>> ValidateIfNotAlreadyExistsAsync(Account account)
        {
            var exists = await ctx.Accounts.AnyAsync(a => a.Code == account.Code);
            return exists
                ? new ResourceAlreadyExistsError("Account", "Code", account.Code)
                : account;
        }

        private async Task<Either<IServiceAndFeatureError, Account>> ValidateIfCurrencyExistsAsync(Account account)
        {
            return await ctx.Currencies.AnyAsync(c => c.Id == account.CurrencyId)
                ? account
                : new BadParamExternalResourceNotFound("Account", "Currency", "CurrencyId", account.CurrencyId.ToString());
        }
    }
}
