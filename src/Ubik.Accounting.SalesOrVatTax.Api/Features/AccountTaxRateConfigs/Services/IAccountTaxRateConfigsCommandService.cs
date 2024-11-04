using LanguageExt;
using Ubik.Accounting.SalesOrVatTax.Api.Models;
using Ubik.Accounting.SalesOrVatTax.Contracts.AccountTaxRateConfigs.Commands;
using Ubik.ApiService.Common.Errors;

namespace Ubik.Accounting.SalesOrVatTax.Api.Features.AccountTaxRateConfigs.Services
{
    public interface IAccountTaxRateConfigsCommandService
    {
        Task<Either<IServiceAndFeatureError, AccountTaxRateConfig>> AttachAsync(AddAccountTaxRateConfigCommand command);
        Task<Either<IServiceAndFeatureError, bool>> DetachAsync(DeleteAccountTaxRateConfigCommand command);
    }
}
