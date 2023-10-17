using Microsoft.EntityFrameworkCore;
using Ubik.Accounting.Api.Data;
using Ubik.Accounting.Api.Models;

namespace Ubik.Accounting.Api.Features.AccountGroups
{
    public class AccountGroupService : IAccountGroupService
    {
        private readonly AccountingContext _context;
        public AccountGroupService(AccountingContext ctx)
        {
            _context = ctx;

        }
        public Task<AccountGroup> AddAsync(AccountGroup account)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<AccountGroup?> GetAsync(Guid id)
        {
            var accountGroup = await _context.AccountGroups.FirstOrDefaultAsync(a => a.Id == id);
            return accountGroup;
        }

        public async Task<IEnumerable<AccountGroup>> GetAllAsync()
        {
            var accountGroups = await _context.AccountGroups.ToListAsync();

            return accountGroups;
        }

        public Task<bool> IfExistsAsync(string accountCode)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IfExistsWithDifferentIdAsync(string accountCode, Guid currentId)
        {
            throw new NotImplementedException();
        }

        public Task<AccountGroup> UpdateAsync(AccountGroup account)
        {
            throw new NotImplementedException();
        }
    }
}
