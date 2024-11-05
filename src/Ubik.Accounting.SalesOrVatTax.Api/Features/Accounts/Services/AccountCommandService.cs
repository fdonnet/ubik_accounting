using Ubik.Accounting.SalesOrVatTax.Api.Data;
using Ubik.Accounting.SalesOrVatTax.Api.Mappers;
using Ubik.Accounting.Structure.Contracts.Accounts.Events;

namespace Ubik.Accounting.SalesOrVatTax.Api.Features.Accounts.Services
{
    public class AccountCommandService(AccountingSalesTaxDbContext ctx) : IAccountCommandService
    {
        public async Task AddAsync(AccountAdded account)
        {
            await ctx.Accounts.AddAsync(account.ToAccount());
            await ctx.SaveChangesAsync();
        }
    }
}
