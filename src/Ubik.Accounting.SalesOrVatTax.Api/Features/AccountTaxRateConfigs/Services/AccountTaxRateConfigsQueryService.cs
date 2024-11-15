using LanguageExt;
using Microsoft.EntityFrameworkCore;
using Ubik.Accounting.SalesOrVatTax.Api.Data;
using Ubik.Accounting.SalesOrVatTax.Api.Features.AccountTaxRateConfigs.Errors;
using Ubik.Accounting.SalesOrVatTax.Api.Models;
using Ubik.ApiService.Common.Errors;

namespace Ubik.Accounting.SalesOrVatTax.Api.Features.AccountTaxRateConfigs.Services
{
    public class AccountTaxRateConfigsQueryService(AccountingSalesTaxDbContext ctx)
        : IAccountTaxRateConfigsQueryService
    {
        public async Task<Either<IFeatureError,IEnumerable<AccountTaxRateConfig>>> GetAllAsync(Guid accountId)
        {
            return await GetAccountAsync(accountId)
                .MapAsync(a => GetAccountTaxConfigs(a.Id));
        }

        public async Task<Either<IFeatureError, AccountTaxRateConfig>> GetAsync(Guid accountId, Guid taxRateId)
        {
            return await GetAccountAsync(accountId)
                .BindAsync(a => GetAccountTaxConfig(a.Id, taxRateId));
        }

        private async Task<IEnumerable<AccountTaxRateConfig>> GetAccountTaxConfigs(Guid accountId)
        {
            return await ctx.AccountTaxRateConfigs.Where(c => c.AccountId == accountId).ToListAsync();
        }

        private async Task<Either<IFeatureError, AccountTaxRateConfig>> GetAccountTaxConfig(Guid accountId, Guid taxRateId)
        {
            var result = await ctx.AccountTaxRateConfigs.Where(c => c.AccountId == accountId && c.TaxRateId == taxRateId).FirstOrDefaultAsync();

            return result == null
                ? new AccountTaxRateConfigNotFoundError(accountId, taxRateId)
                : result;
        }

        private async Task<Either<IFeatureError, Account>> GetAccountAsync(Guid accountId)
        {
            var result = await ctx.Accounts.FindAsync(accountId);

            return result == null
                ? new ResourceNotFoundError("Account", "Id", accountId.ToString())
                : result;
        }
    }

    
}
