﻿using MassTransit;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;
using Ubik.Accounting.Structure.Api.Models;

namespace Ubik.Accounting.Structure.Api.Data.Init
{
    internal static class CurrenciesData
    {
        internal static async Task LoadAsync(AccountingDbContext context)
        {
            if (!context.Currencies.Any())
            {
                var query = await File.ReadAllTextAsync(@"Data/Init/CurrenciesData.sql");
                await context.Database.ExecuteSqlAsync(FormattableStringFactory.Create(query));
            }
        }
    }
}
