using Ubik.Accounting.Api.Features.Accounts.Services;
using Ubik.Accounting.Api.Features.AccountGroups.Services;
using Ubik.Accounting.Api.Features.Classifications.Services;

namespace Ubik.Accounting.Api.Features
{
    public interface IServiceManager
    {
        IAccountService AccountService { get; }
        IAccountGroupService AccountGroupService { get; }
        IClassificationService ClassificationService { get; }
        //TODO: pass the cancellation token
        Task SaveAsync();
    }
}
