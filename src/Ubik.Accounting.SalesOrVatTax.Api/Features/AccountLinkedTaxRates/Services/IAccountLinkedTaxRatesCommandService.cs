using LanguageExt;
using Ubik.Accounting.SalesOrVatTax.Api.Models;
using Ubik.Accounting.SalesOrVatTax.Contracts.AccountLinkedTaxRates.Commands;
using Ubik.ApiService.Common.Errors;

namespace Ubik.Accounting.SalesOrVatTax.Api.Features.AccountLinkedTaxRates.Services
{
    public interface IAccountLinkedTaxRatesCommandService
    {
        Task<Either<IServiceAndFeatureError, AccountTaxRateConfig>> AttachAsync(AddAccountTaxRateConfigCommand command);
        Task<Either<IServiceAndFeatureError, bool>> DetachAsync(DeleteAccountTaxRateConfigCommand command);
    }
}
