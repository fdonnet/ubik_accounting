using Ubik.Accounting.Api.Models;

namespace Ubik.Accounting.Api.Features.Accounts
{
    public interface IAccountService
    {
        public Task<IEnumerable<Account>> GetAllAsync();
        public Task<Account?> GetAsync(Guid id);
        public Task<bool> IfExistsAsync(string accountCode);
        public Task<bool> IfExistsWithDifferentIdAsync(string accountCode, Guid currentId);
        public Task<bool> IfExistsAccountGroupAsync(Guid accountGroupId);
        public Task<Account> AddAsync(Account account);
        public Task<Account> UpdateAsync(Account account);
        public Task<bool> DeleteAsync(Guid id);

    }
}
