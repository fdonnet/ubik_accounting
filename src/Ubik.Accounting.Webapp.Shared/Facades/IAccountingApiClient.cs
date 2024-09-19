using Ubik.Accounting.Contracts.AccountGroups.Commands;
using Ubik.Accounting.Contracts.Accounts.Commands;
using Ubik.Accounting.Contracts.Classifications.Commands;

namespace Ubik.Accounting.Webapp.Shared.Facades
{
    public interface IAccountingApiClient
    {
        Task<HttpResponseMessage> GetAllAccountsAsync(CancellationToken cancellationToken = default);
        Task<HttpResponseMessage> GetAccountAsync(Guid id, CancellationToken cancellationToken = default);
        Task<HttpResponseMessage> AddAccountAsync(AddAccountCommand account, CancellationToken cancellationToken = default);
        Task<HttpResponseMessage> AddAccountInAccountGroupAsync(AddAccountInAccountGroupCommand accountInAccountGrp, CancellationToken cancellationToken = default);
        Task<HttpResponseMessage> DeleteAccountInAccountGroupAsync(DeleteAccountInAccountGroupCommand accountInAccountGrp, CancellationToken cancellationToken = default);
        Task<HttpResponseMessage> UpdateAccountAsync(Guid id, UpdateAccountCommand account, CancellationToken cancellationToken = default);
        Task<HttpResponseMessage> DeleteAccountAsync(Guid id, CancellationToken cancellationToken = default);
        Task<HttpResponseMessage> GetAllCurrenciesAsync(CancellationToken cancellationToken = default);

        Task<HttpResponseMessage> GetAllClassificationsAsync(CancellationToken cancellationToken = default);
        Task<HttpResponseMessage> DeleteClassificationAsync(Guid id, CancellationToken cancellationToken = default);
        Task<HttpResponseMessage> AddClassificationAsync(AddClassificationCommand classification, CancellationToken cancellationToken = default);
        Task<HttpResponseMessage> UpdateClassificationAsync(Guid id, UpdateClassificationCommand classification, CancellationToken cancellationToken = default);
        Task<HttpResponseMessage> GetClassificationMissingAccountsAsync(Guid id,CancellationToken cancellationToken = default);

        Task<HttpResponseMessage> GetAllAccountGroupsAsync(CancellationToken cancellationToken = default);
        Task<HttpResponseMessage> AddAccountGroupAsync(AddAccountGroupCommand classification, CancellationToken cancellationToken = default);
        Task<HttpResponseMessage> UpdateAccountGroupAsync(Guid id, UpdateAccountGroupCommand classification, CancellationToken cancellationToken = default);
        Task<HttpResponseMessage> DeleteAccountGroupAsync(Guid id, CancellationToken cancellationToken = default);
        Task<HttpResponseMessage> GetAllAccountsLinksAsync(CancellationToken cancellationToken = default);
    }
}
