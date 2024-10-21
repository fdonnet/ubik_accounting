using Ubik.Accounting.Api.Features.Accounts.Services;
using Ubik.Accounting.Api.Features.AccountGroups.Services;
using Ubik.Accounting.Api.Features.Classifications.Services;
using Ubik.Accounting.Api.Features.Currencies.Services;

namespace Ubik.Accounting.Api.Features
{
    public interface IServiceManager
    {
        IClassificationService ClassificationService { get; }
        ICurrencyService CurrencyService { get; }
        //TODO: pass the cancellation token
        Task SaveAsync();
    }
}
