﻿using Microsoft.EntityFrameworkCore;
using Ubik.Accounting.Structure.Contracts.Accounts.Events;
using Ubik.Accounting.Transaction.Api.Data;
using Ubik.Accounting.Transaction.Api.Mappers;

namespace Ubik.Accounting.Transaction.Api.Features.TaxRates.Services
{
    public class TaxRateCommandService(AccountingTxContext ctx) : ITaxRateCommandService
    {
        public async Task AddAsync(TaxRateAdded accountAdded)
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
