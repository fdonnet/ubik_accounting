using LanguageExt;
using Ubik.Accounting.SalesOrVatTax.Api.Models;
using Ubik.ApiService.Common.Errors;

namespace Ubik.Accounting.SalesOrVatTax.Api.Features.AccountLinkedTaxRates.Services
{
    public interface IAccountLinkedTaxRatesQueryService
    {
        Task<Either<IServiceAndFeatureError, IEnumerable<AccountTaxRateConfig>>> GetAllAsync(Guid accountId);
        Task<Either<IServiceAndFeatureError, AccountTaxRateConfig>> GetAsync(Guid accountId, Guid taxRateId);
    }
}
