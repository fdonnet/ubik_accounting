using LanguageExt;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Ubik.Accounting.Api.Data;
using Ubik.Accounting.Api.Features.Mappers;
using Ubik.Accounting.Api.Models;
using Ubik.ApiService.Common.Errors;
using Ubik.ApiService.Common.Exceptions;

namespace Ubik.Accounting.Api.Features.Accounts.Services
{
    public class AccountCommandService(AccountingDbContext ctx, IPublishEndpoint publishEndpoint) : IAccountCommandService
    {
        public async Task<Either<IServiceAndFeatureError, Account>> AddAsync(Account account)
        {
            return await ValidateIfNotAlreadyExistsAsync(account)
                .BindAsync(ValidateIfCurrencyExistsAsync)
                .BindAsync(AddInDbContextAsync)
                .BindAsync(AddSaveAndPublishAsync);
        }

        public async Task<Either<IServiceAndFeatureError, Account>> UpdateAsync(Account account)
        {
            return await GetAsync(account.Id)
                .BindAsync(ac => MapInDbContextAsync(ac, account))
                .BindAsync(ValidateIfNotAlreadyExistsWithOtherIdAsync)
                .BindAsync(ValidateIfCurrencyExistsAsync)
                .BindAsync(UpdateInDbContext)
                .BindAsync(UpdateSaveAndPublishAsync);
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

        private async Task<Either<IServiceAndFeatureError, Account>> UpdateInDbContext(Account current)
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

        private async Task<Either<IServiceAndFeatureError, Account>> MapInDbContextAsync
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
