using LanguageExt;
using Ubik.Accounting.Api.Models;
using Ubik.ApiService.Common.Errors;

namespace Ubik.Accounting.Api.Features.VatRates.Services
{
    public interface IVatRateQueryService
    {
        Task<Either<IServiceAndFeatureError, VatRate>> GetAsync(Guid id);
        Task<IEnumerable<VatRate>> GetAllAsync();
    }
}
