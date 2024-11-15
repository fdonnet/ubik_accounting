using Microsoft.EntityFrameworkCore;
using Ubik.Accounting.SalesOrVatTax.Api.Data;
using Ubik.Accounting.SalesOrVatTax.Api.Mappers;
using Ubik.Accounting.Structure.Contracts.Accounts.Events;

namespace Ubik.Accounting.SalesOrVatTax.Api.Features.Accounts.Services
{
    public class AccountCommandService(AccountingSalesTaxDbContext ctx) : IAccountCommandService
    {
        public async Task AddAsync(AccountAdded accountAdded)
        {
            await ctx.Accounts.AddAsync(accountAdded.ToAccount());
            await ctx.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid accountId)
        {
            await ctx.Accounts.Where(a => a.Id == accountId).ExecuteDeleteAsync();
        }

        public async Task UpdateAsync(AccountUpdated accountUpdated)
        {
            await ctx.Accounts.Where(a => a.Id == accountUpdated.Id).ExecuteUpdateAsync(setters => setters
                .SetProperty(b => b.Active, accountUpdated.Active)
                .SetProperty(b => b.Version, accountUpdated.Version)
                .SetProperty(b => b.Code, accountUpdated.Code)
                .SetProperty(b => b.Label, accountUpdated.Label));
        }
    }
}
