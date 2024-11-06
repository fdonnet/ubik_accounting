using Ubik.Accounting.Structure.Contracts.Accounts.Events;
using Ubik.Accounting.Transaction.Api.Models;

namespace Ubik.Accounting.Transaction.Api.Mappers
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
