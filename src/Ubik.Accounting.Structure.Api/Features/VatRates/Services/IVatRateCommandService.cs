using LanguageExt;
using Ubik.Accounting.Structure.Api.Models;
using Ubik.Accounting.Structure.Contracts.VatRate.Commands;
using Ubik.ApiService.Common.Errors;

namespace Ubik.Accounting.Structure.Api.Features.VatRates.Services
{
    public interface IVatRateCommandService
    {
        public Task<Either<IServiceAndFeatureError, VatRate>> AddAsync(AddVatRateCommand command);
        public Task<Either<IServiceAndFeatureError, VatRate>> UpdateAsync(UpdateVatRateCommand command);
        public Task<Either<IServiceAndFeatureError, bool>> DeleteAsync(Guid id);
    }
}
