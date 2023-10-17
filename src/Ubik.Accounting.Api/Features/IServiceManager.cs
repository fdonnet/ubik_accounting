using Ubik.Accounting.Api.Features.AccountGroups;
using Ubik.Accounting.Api.Features.Accounts;

namespace Ubik.Accounting.Api.Features
{
    public interface IServiceManager
    {
        IAccountService AccountService { get; }
        IAccountGroupService AccountGroupService { get; }
        Task SaveAsync();
    }
}
