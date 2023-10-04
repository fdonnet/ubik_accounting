using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Ubik.Accounting.Api.Data;
using Ubik.Accounting.Api.Models;

namespace Ubik.Accounting.Api.Service
{
    public class ChartOfAccountsService
    {
        private readonly AccountingContext _context;
        public ChartOfAccountsService(AccountingContext ctx)
        {
            _context = ctx;
        }

        public async Task<IEnumerable<Account>> GetAccountsAsync()
        {
            return await _context.Accounts.ToListAsync();
        }
    }
}
