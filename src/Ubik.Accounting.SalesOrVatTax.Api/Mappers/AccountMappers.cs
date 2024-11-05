using Ubik.Accounting.SalesOrVatTax.Api.Models;
using Ubik.Accounting.SalesOrVatTax.Contracts.SalesOrVatTaxRate.Commands;
using Ubik.Accounting.Structure.Contracts.Accounts.Events;

namespace Ubik.Accounting.SalesOrVatTax.Api.Mappers
{
    public static class AccountMappers
    {
        public static Account ToAccount(this AccountAdded current)
        {
            return new Account
            {
                Active = current.Active,
                Code = current.Code,
                Id = current.Id,
                Label = current.Label,
                TenantId = current.TenantId,
                Version = current.Version
            };
        }
    }
}
