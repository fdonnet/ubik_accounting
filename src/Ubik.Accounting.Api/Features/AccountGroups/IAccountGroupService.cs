using Ubik.Accounting.Api.Models;

namespace Ubik.Accounting.Api.Features.AccountGroups
{
    public interface IAccountGroupService
    {
        public Task<IEnumerable<AccountGroup>> GetAllAsync();
        public Task<AccountGroup?> GetAsync(Guid id);
        public Task<bool> IfExistsAsync(string accountCode);
        public Task<bool> IfExistsWithDifferentIdAsync(string accountCode, Guid currentId);
        public Task<AccountGroup> AddAsync(AccountGroup account);
        public Task<AccountGroup> UpdateAsync(AccountGroup account);
        public Task<bool> DeleteAsync(Guid id);
    }
}
