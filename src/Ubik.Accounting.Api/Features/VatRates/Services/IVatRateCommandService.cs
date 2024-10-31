using LanguageExt;
using Ubik.Accounting.Api.Models;
using Ubik.Accounting.Contracts.VatRate.Commands;
using Ubik.ApiService.Common.Errors;

namespace Ubik.Accounting.Api.Features.VatRates.Services
{
    public interface IVatRateCommandService
    {
        public Task<Either<IServiceAndFeatureError, VatRate>> AddAsync(AddVatRateCommand command);
        public Task<Either<IServiceAndFeatureError, VatRate>> UpdateAsync(UpdateVatRateCommand command);
        public Task<Either<IServiceAndFeatureError, bool>> DeleteAsync(Guid id);
    }
}
