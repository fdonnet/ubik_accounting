using LanguageExt;
using MassTransit;
using MassTransit.Transports;
using Microsoft.EntityFrameworkCore;
using Ubik.Accounting.SalesOrVatTax.Api.Data;
using Ubik.Accounting.SalesOrVatTax.Api.Features.AccountTaxRateConfigs.Errors;
using Ubik.Accounting.SalesOrVatTax.Api.Mappers;
using Ubik.Accounting.SalesOrVatTax.Api.Models;
using Ubik.Accounting.SalesOrVatTax.Contracts.AccountTaxRateConfigs.Commands;
using Ubik.Accounting.SalesOrVatTax.Contracts.AccountTaxRateConfigs.Events;
using Ubik.ApiService.Common.Errors;

namespace Ubik.Accounting.SalesOrVatTax.Api.Features.AccountTaxRateConfigs.Services
{
    public class AccountTaxRateConfigsCommandService(AccountingSalesTaxDbContext ctx, IPublishEndpoint publishEndpoint) : IAccountTaxRateConfigsCommandService
    {
        public async Task<Either<IServiceAndFeatureError, AccountTaxRateConfig>> AttachAsync(AddAccountTaxRateConfigCommand command)
        {
            return await GetAsync(command.TaxRateId)
                        .BindAsync(t => GetAccountAsync(command.AccountId))
                        .BindAsync(t => GetTaxAccountAsync(command.TaxAccountId))
                        .BindAsync(t => ValidateIfLinkNotAlreadyExistsAsync(command.AccountId, command.TaxRateId)
                        .BindAsync(l => AddTaxRateLinkInDbContextAsync(command))
                        .BindAsync(AddTaxRateLinkSaveAndPublishAsync));
        }


        public async Task<Either<IServiceAndFeatureError, bool>> DetachAsync(DeleteAccountTaxRateConfigCommand command)
        {
            return await GetAccountTaxRateConfigAsync(command.AccountId, command.TaxRateId)
                        .BindAsync(DeleteInDbContextAsync)
                        .BindAsync(DeletedSaveAndPublishAsync);
        }

        private async Task<Either<IServiceAndFeatureError, AccountTaxRateConfig>> DeleteInDbContextAsync(AccountTaxRateConfig current)
        {
            ctx.Entry(current).State = EntityState.Deleted;

            await Task.CompletedTask;
            return current;
        }

        private async Task<Either<IServiceAndFeatureError, bool>> DeletedSaveAndPublishAsync(AccountTaxRateConfig current)
        {
            await publishEndpoint.Publish(new AccountTaxRateConfigDeleted { Id = current.Id }, CancellationToken.None);
            await ctx.SaveChangesAsync();

            return true;
        }

        private async Task<Either<IServiceAndFeatureError, AccountTaxRateConfig>> GetAccountTaxRateConfigAsync(Guid accountId, Guid taxRateId)
        {
            var result = await ctx.AccountTaxRateConfigs.FirstOrDefaultAsync(x => x.AccountId == accountId && x.TaxRateId == taxRateId);

            return result == null
                ? new ResourceNotFoundError("AccountTaxRateConfig", "AccountId/TaxRateId", $"{accountId.ToString()}/{taxRateId.ToString()}")
                : result;
        }

        private async Task<Either<IServiceAndFeatureError, AccountTaxRateConfig>> AddTaxRateLinkSaveAndPublishAsync(AccountTaxRateConfig current)
        {
            await publishEndpoint.Publish(current.ToAccountTaxRateConfigAdded(), CancellationToken.None);
            await ctx.SaveChangesAsync();

            return current;
        }

        private async Task<Either<IServiceAndFeatureError, AccountTaxRateConfig>> AddTaxRateLinkInDbContextAsync(AddAccountTaxRateConfigCommand command)
        {
            var accountTaxRateConfig = command.ToAccountTaxRateConfig();
            var result = await ctx.AccountTaxRateConfigs.AddAsync(command.ToAccountTaxRateConfig());
            ctx.SetAuditAndSpecialFields();

            return accountTaxRateConfig;
        }

        private async Task<Either<IServiceAndFeatureError, bool>> ValidateIfLinkNotAlreadyExistsAsync(Guid accountId, Guid taxRateId)
        {
            var result = await ctx.AccountTaxRateConfigs.AnyAsync(x => x.AccountId == accountId
                            && x.TaxRateId == taxRateId);

            return result
                ? new LinkedTaxRateAlreadyExist(accountId, taxRateId)
                : true;
        }

        private async Task<Either<IServiceAndFeatureError, Account>> GetAccountAsync(Guid accountId)
        {
            var result = await ctx.Accounts.FindAsync(accountId);

            return result == null
                ? new ResourceNotFoundError("Account", "Id", accountId.ToString())
                : result;
        }

        private async Task<Either<IServiceAndFeatureError, Account>> GetTaxAccountAsync(Guid taxAccountId)
        {
            var result = await ctx.Accounts.FindAsync(taxAccountId);

            return result == null
                ? new BadParamExternalResourceNotFound("AccountTaxConfig", "TaxAccount", "TaxAccountId", taxAccountId.ToString())
                : result;
        }

        private async Task<Either<IServiceAndFeatureError, TaxRate>> GetAsync(Guid taxRateId)
        {
            var result = await ctx.TaxRates.FindAsync(taxRateId);

            return result == null
                ? new ResourceNotFoundError("TaxRate", "Id", taxRateId.ToString())
                : result;
        }
    }
}
