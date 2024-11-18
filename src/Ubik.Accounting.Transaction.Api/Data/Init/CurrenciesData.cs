﻿using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace Ubik.Accounting.Transaction.Api.Data.Init
{
    internal static class CurrenciesData
    {
        internal static async Task LoadAsync(AccountingTxContext context)
        {
            if (!context.Currencies.Any())
            {
                var query = await File.ReadAllTextAsync(@"Data/Init/CurrenciesData.sql");
                await context.Database.ExecuteSqlAsync(FormattableStringFactory.Create(query));
            }
        }
    }
}