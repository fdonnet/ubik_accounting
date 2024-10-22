using Microsoft.EntityFrameworkCore;
using Ubik.Accounting.Api.Data;
using Ubik.Accounting.Api.Models;

namespace Ubik.Accounting.Api.Features.Currencies.Services
{
    public class CurrencyQueryService(AccountingDbContext ctx) : ICurrencyQueryService
    {
        public async Task<IEnumerable<Currency>> GetAllAsync()
        {
            return  await ctx.Currencies.ToListAsync();
        }
    }
}
