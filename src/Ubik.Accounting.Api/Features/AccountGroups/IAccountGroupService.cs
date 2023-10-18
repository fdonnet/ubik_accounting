using Ubik.Accounting.Api.Models;

namespace Ubik.Accounting.Api.Features.AccountGroups
{
    public interface IAccountGroupService
    {
        public Task<IEnumerable<AccountGroup>> GetAllAsync();
        public Task<AccountGroup?> GetAsync(Guid id);
        public Task<bool> IfExistsAsync(string accountGroupCode);
        public Task<bool> IfExistsAsync(Guid accountGroupId);
        public Task<bool> IfExistsWithDifferentIdAsync(string accountGroupCode, Guid currentId);
        public Task<AccountGroup> AddAsync(AccountGroup accountGroup);
        public Task<AccountGroup> UpdateAsync(AccountGroup accountGroup);
        public Task<bool> DeleteAsync(Guid id);
    }
}
