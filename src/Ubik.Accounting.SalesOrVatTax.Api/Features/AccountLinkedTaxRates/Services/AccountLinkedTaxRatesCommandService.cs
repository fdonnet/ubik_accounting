using LanguageExt;
using MassTransit;
using MassTransit.Transports;
using Microsoft.EntityFrameworkCore;
using Ubik.Accounting.SalesOrVatTax.Api.Data;
using Ubik.Accounting.SalesOrVatTax.Api.Features.AccountLinkedTaxRates.Errors;
using Ubik.Accounting.SalesOrVatTax.Api.Mappers;
using Ubik.Accounting.SalesOrVatTax.Api.Models;
using Ubik.Accounting.SalesOrVatTax.Contracts.AccountLinkedTaxRates.Commands;
using Ubik.ApiService.Common.Errors;

namespace Ubik.Accounting.SalesOrVatTax.Api.Features.AccountLinkedTaxRates.Services
{
    public class AccountLinkedTaxRatesCommandService(AccountingSalesTaxDbContext ctx, IPublishEndpoint publishEndpoint) : IAccountLinkedTaxRatesCommandService
    {
        public async Task<Either<IServiceAndFeatureError, AccountTaxRateConfig>> AttachAsync(AddTaxRateToAccountCommand command)
        {
            return await GetAsync(command.TaxRateId)
                        .BindAsync(t => GetAccountAsync(command.AccountId))
                        .BindAsync(t => GetTaxAccountAsync(command.TaxAccountId))
                        .BindAsync(t => ValidateIfLinkNotAlreadyExistsAsync(command.AccountId, command.TaxRateId)
                        .BindAsync(l => AddTaxRateLinkInDbContextAsync(l, command))
                        .BindAsync(AddTaxRateLinkSaveAndPublishAsync));
        }

        //public async Task<Either<IServiceAndFeatureError, TaxRate>> DetachAsync(AddTaxRateToAccountCommand command)
        //{
            
        //}

        private async Task<Either<IServiceAndFeatureError, AccountTaxRateConfig>> AddTaxRateLinkSaveAndPublishAsync(AccountTaxRateConfig current)
        {
            await publishEndpoint.Publish(current.ToAccountTaxRateConfigAdded(), CancellationToken.None);
            await ctx.SaveChangesAsync();

            return current;
        }

        private async Task<Either<IServiceAndFeatureError, AccountTaxRateConfig>> AddTaxRateLinkInDbContextAsync(AccountTaxRateConfig current
            , AddTaxRateToAccountCommand command)
        {
            current.TaxAccountId = command.TaxAccountId;

            await ctx.AccountTaxRateConfigs.AddAsync(current);
            ctx.SetAuditAndSpecialFields();

            return current;
        }

        private async Task<Either<IServiceAndFeatureError, AccountTaxRateConfig>> ValidateIfLinkNotAlreadyExistsAsync(Guid accountId, Guid taxRateId)
        {
            var result = await ctx.AccountTaxRateConfigs.AnyAsync(x => x.AccountId == accountId
                            && x.TaxRateId == taxRateId );

            return result
                ? new LinkedTaxRateAlreadyExist(accountId,taxRateId)
                : new AccountTaxRateConfig() { AccountId= accountId, TaxRateId=taxRateId};
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
                ? new BadParamExternalResourceNotFound("AccountTaxConfig","TaxAccount", "TaxAccountId", taxAccountId.ToString())
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
