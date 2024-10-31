using LanguageExt;
using Ubik.Accounting.Structure.Api.Models;
using Ubik.ApiService.Common.Errors;

namespace Ubik.Accounting.Structure.Api.Features.VatRates.Services
{
    public interface IVatRateQueryService
    {
        Task<Either<IServiceAndFeatureError, VatRate>> GetAsync(Guid id);
        Task<IEnumerable<VatRate>> GetAllAsync();
    }
}
