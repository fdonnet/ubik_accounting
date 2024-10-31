using Microsoft.EntityFrameworkCore;
using Ubik.Accounting.Structure.Api.Data;
using Ubik.Accounting.Structure.Api.Models;

namespace Ubik.Accounting.Structure.Api.Features.Currencies.Services
{
    public class CurrencyQueryService(AccountingDbContext ctx) : ICurrencyQueryService
    {
        public async Task<IEnumerable<Currency>> GetAllAsync()
        {
            return  await ctx.Currencies.ToListAsync();
        }
    }
}
