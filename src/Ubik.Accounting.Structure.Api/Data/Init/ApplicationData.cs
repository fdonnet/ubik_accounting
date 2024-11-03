using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;
using Ubik.Accounting.Structure.Api.Models;

namespace Ubik.Accounting.Structure.Api.Data.Init
{
    internal static class ApplicationData
    {
        internal static async Task LoadAsync(AccountingDbContext context)
        {
            if (!context.Applications.Any())
            {
                var app = new Application() { IsReady= false };
                await context.Applications.AddAsync(app);
                await context.SaveChangesAsync();
            }
        }
    }
}
