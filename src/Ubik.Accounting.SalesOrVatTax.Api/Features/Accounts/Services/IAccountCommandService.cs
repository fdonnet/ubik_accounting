using Ubik.Accounting.Structure.Contracts.Accounts.Events;

namespace Ubik.Accounting.SalesOrVatTax.Api.Features.Accounts.Services
{
    public interface IAccountCommandService
    {
        public Task AddAsync(AccountAdded account);
    }
}
