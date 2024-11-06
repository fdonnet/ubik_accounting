using LanguageExt;
using Microsoft.EntityFrameworkCore;
using Ubik.Accounting.SalesOrVatTax.Contracts.TaxRates.Events;
using Ubik.Accounting.Transaction.Api.Data;
using Ubik.Accounting.Transaction.Api.Mappers;

namespace Ubik.Accounting.Transaction.Api.Features.TaxRates.Services
{
    public class TaxRateCommandService(AccountingTxContext ctx) : ITaxRateCommandService
    {
        public async Task AddAsync(TaxRateAdded toAdd)
        {
            await ctx.TaxRates.AddAsync(toAdd.ToTaxRate());
            await ctx.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid Id)
        {
            await ctx.TaxRates.Where(a => a.Id == Id).ExecuteDeleteAsync();
        }

        public async Task UpdateAsync(TaxRateUpdated toUpd)
        {
            await ctx.TaxRates.Where(a => a.Id == toUpd.Id).ExecuteUpdateAsync(setters => setters
                .SetProperty(t => t.Id, toUpd.Id)
                .SetProperty(t => t.TenantId, toUpd.TenantId)
                .SetProperty(t => t.Code, toUpd.Code)
                .SetProperty(t => t.Version, toUpd.Version)
                .SetProperty(t => t.Rate, toUpd.Rate));
        }
    }
}
