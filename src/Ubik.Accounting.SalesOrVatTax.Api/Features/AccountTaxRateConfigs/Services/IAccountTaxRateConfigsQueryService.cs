using LanguageExt;
using Ubik.Accounting.SalesOrVatTax.Api.Models;
using Ubik.ApiService.Common.Errors;

namespace Ubik.Accounting.SalesOrVatTax.Api.Features.AccountTaxRateConfigs.Services
{
    public interface IAccountTaxRateConfigsQueryService
    {
        Task<Either<IFeatureError, IEnumerable<AccountTaxRateConfig>>> GetAllAsync(Guid accountId);
        Task<Either<IFeatureError, AccountTaxRateConfig>> GetAsync(Guid accountId, Guid taxRateId);
    }
}
