﻿using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace Ubik.Accounting.SalesOrVatTax.Api.Data.Init
{
    internal static class AccountTaxRateConfigsData
    {
        internal static async Task LoadAsync(AccountingSalesTaxDbContext context)
        {
            if (!context.AccountTaxRateConfigs.Any())
            {
                var query = await File.ReadAllTextAsync(@"Data/Init/AccountTaxRateConfigsData.sql");
                await context.Database.ExecuteSqlAsync(FormattableStringFactory.Create(query));
            }
        }
    }
}
