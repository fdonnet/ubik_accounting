using LanguageExt;
using Microsoft.EntityFrameworkCore;
using Ubik.Accounting.SalesOrVatTax.Api.Data;
using Ubik.Accounting.SalesOrVatTax.Api.Models;
using Ubik.ApiService.Common.Errors;

namespace Ubik.Accounting.SalesOrVatTax.Api.Features.TaxRates.Services
{
    public class TaxRateQueryService(AccountingSalesTaxDbContext ctx) : ITaxRateQueryService
    {
        public async Task<IEnumerable<TaxRate>> GetAllAsync()
        {
            var result = await ctx.SalesOrVatTaxRates.ToListAsync();

            return result;
        }

        public async Task<Either<IServiceAndFeatureError, TaxRate>> GetAsync(Guid id)
        {
            var result = await ctx.SalesOrVatTaxRates.FindAsync(id);

            return result == null
                ? new ResourceNotFoundError("VatRate", "Id", id.ToString())
                : result;
        }
    }
}
