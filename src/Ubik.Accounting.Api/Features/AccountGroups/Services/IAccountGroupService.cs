using Ubik.Accounting.Api.Models;
using Ubik.ApiService.Common.Exceptions;

namespace Ubik.Accounting.Api.Features.AccountGroups.Services
{
    public interface IAccountGroupService
    {
        public Task<IEnumerable<AccountGroup>> GetAllAsync();
        public Task<ResultT<AccountGroup>> GetAsync(Guid id);
        public Task<ResultT<AccountGroup>> GetWithChildAccountsAsync(Guid id);
        public Task<bool> IfExistsAsync(string accountGroupCode, Guid accountGroupClassificationId);
        public Task<bool> IfExistsAsync(Guid accountGroupId);
        public Task<bool> IfExistsWithDifferentIdAsync(string accountGroupCode,
            Guid accountGroupClassificationId, Guid currentId);

        public Task<bool> HasAnyChildAccountGroups(Guid Id);
        public Task<bool> HasAnyChildAccounts(Guid Id);
        public Task<bool> IfClassificationExists(Guid accountGroupClassificationId);
        public Task<ResultT<AccountGroup>> AddAsync(AccountGroup accountGroup);
        public Task<ResultT<AccountGroup>> UpdateAsync(AccountGroup accountGroup);
        public Task<ResultT<bool>> DeleteAsync(Guid id);
        public Task DeleteAllChildrenOfAsync(Guid id);
    }
}
