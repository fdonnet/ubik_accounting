using Ubik.Accounting.SalesOrVatTax.Contracts.TaxRates.Events;
using Ubik.Accounting.Structure.Contracts.Accounts.Events;

namespace Ubik.Accounting.Transaction.Api.Features.TaxRates.Services
{
    public interface ITaxRateCommandService
    {
        public Task AddAsync(TaxRateAdded toAdd);
        public Task UpdateAsync(TaxRateUpdated toUpd);
        public Task DeleteAsync(Guid accountId);
    }
}
