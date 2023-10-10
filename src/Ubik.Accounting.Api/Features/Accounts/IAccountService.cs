using Ubik.Accounting.Api.Models;

namespace Ubik.Accounting.Api.Features.Accounts
{
    public interface IAccountService
    {
        public Task<IEnumerable<Account>> GetAccountsAsync();
        public Task<Account?> GetAccountAsync(Guid id);
        public Task<bool> IfExists(string accountCode);
        public Task<bool> IfExistsWithDifferentId(string accountCode, Guid currentId);
        public Task<Account> AddAccountAsync(Account account);
        public Task<Account> UpdateAccountAsync(Account account);
    }
}
