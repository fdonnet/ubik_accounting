using Ubik.Accounting.Contracts.Accounts.Results;

namespace Ubik.Accounting.Webapp.Shared.Facades
{
    public interface IClientContactFacade
    {
        Task<IEnumerable<GetAllAccountsResult>> GetAllAccountsAsync(CancellationToken cancellationToken = default);
    }
}
