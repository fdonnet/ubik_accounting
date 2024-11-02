﻿using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace Ubik.Accounting.SalesOrVatTax.Api.Data.Init
{
    internal static class TaxRatesData
    {
        internal static async Task LoadAsync(AccountingSalesTaxDbContext context)
        {
            if (!context.TaxRates.Any())
            {
                var query = await File.ReadAllTextAsync(@"Data/Init/TaxRatesData.sql");
                await context.Database.ExecuteSqlAsync(FormattableStringFactory.Create(query));
            }
        }
    }
}