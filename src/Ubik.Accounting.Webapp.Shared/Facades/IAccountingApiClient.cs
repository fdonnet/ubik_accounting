namespace Ubik.Accounting.Webapp.Shared.Facades
{
    public interface IAccountingApiClient
    {
        Task<HttpResponseMessage> GetAllAccountsAsync(CancellationToken cancellationToken = default);
        Task<HttpResponseMessage> GetAllCurrenciesAsync(CancellationToken cancellationToken = default);
        Task<HttpResponseMessage> GetAccountAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
