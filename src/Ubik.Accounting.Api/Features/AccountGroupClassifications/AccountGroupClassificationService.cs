using Microsoft.EntityFrameworkCore;
using Ubik.Accounting.Api.Data;
using Ubik.Accounting.Api.Models;

namespace Ubik.Accounting.Api.Features.AccountGroupClassifications
{
    public class AccountGroupClassificationService : IAccountGroupClassificationService
    {
        private readonly AccountingContext _context;
        public AccountGroupClassificationService(AccountingContext ctx)
        {
            _context = ctx;

        }

        public async Task<bool> IfExistsAsync(Guid id)
        {
            return await _context.AccountGroupClassifications.AnyAsync(a => a.Id == id);
        }
    }
}
