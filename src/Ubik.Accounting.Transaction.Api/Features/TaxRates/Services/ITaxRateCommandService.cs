using Ubik.Accounting.Structure.Contracts.Accounts.Events;

namespace Ubik.Accounting.Transaction.Api.Features.TaxRates.Services
{
    public interface ITaxRateCommandService
    {
        public Task AddAsync(AccountAdded accountAdded);
        public Task UpdateAsync(AccountUpdated accountUpdated);
        public Task DeleteAsync(Guid accountId);
    }
}
