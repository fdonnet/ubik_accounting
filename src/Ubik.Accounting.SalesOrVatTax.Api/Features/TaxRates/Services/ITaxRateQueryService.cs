using LanguageExt;
using Ubik.Accounting.SalesOrVatTax.Api.Models;
using Ubik.ApiService.Common.Errors;

namespace Ubik.Accounting.SalesOrVatTax.Api.Features.TaxRates.Services
{
    public interface ITaxRateQueryService
    {
        Task<Either<IFeatureError, TaxRate>> GetAsync(Guid id);
        Task<IEnumerable<TaxRate>> GetAllAsync();
    }
}
