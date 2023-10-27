using Ubik.Accounting.Api.Models;

namespace Ubik.Accounting.Api.Features.Accounts
{
    public interface IAccountService
    {
        public Task<IEnumerable<Account>> GetAllAsync();
        public Task<Account?> GetAsync(Guid id);
        public Task<bool> IfExistsAsync(string accountCode);
        public Task<bool> IfExistsWithDifferentIdAsync(string accountCode, Guid currentId);
        public Task<Account> AddAsync(Account account);
        public Account Update(Account account);
        public Task<bool> ExecuteDeleteAsync(Guid id);
        public Task<bool> IfExistsCurrencyAsync(Guid currencyId);

    }
}
