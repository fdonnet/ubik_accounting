using LanguageExt;
using Ubik.Accounting.SalesOrVatTax.Api.Models;
using Ubik.Accounting.SalesOrVatTax.Contracts.SalesOrVatTaxRate.Commands;
using Ubik.ApiService.Common.Errors;

namespace Ubik.Accounting.SalesOrVatTax.Api.Features.TaxRates.Services
{
    public interface ITaxRateCommandService
    {
        public Task<Either<IFeatureError, TaxRate>> AddAsync(TaxRateCommand command);
        public Task<Either<IFeatureError, TaxRate>> UpdateAsync(UpdateSalesOrVatTaxRateCommand command);
        public Task<Either<IFeatureError, bool>> DeleteAsync(Guid id);
    }
}
