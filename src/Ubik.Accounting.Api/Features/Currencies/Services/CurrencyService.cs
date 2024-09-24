using Microsoft.EntityFrameworkCore;
using Ubik.Accounting.Api.Data;
using Ubik.Accounting.Api.Models;

namespace Ubik.Accounting.Api.Features.Currencies.Services
{
    public class CurrencyService : ICurrencyService
    {
        private readonly AccountingDbContext _context;
        public CurrencyService(AccountingDbContext ctx)
        {
            _context = ctx;

        }
        public async Task<IEnumerable<Currency>> GetAllAsync()
        {
            return  await _context.Currencies.ToListAsync();
        }
    }
}
