using LanguageExt;
using Ubik.Accounting.Api.Models;
using Ubik.ApiService.Common.Exceptions;

namespace Ubik.Accounting.Api.Features.AccountGroups.Services
{
    public interface IAccountGroupService
    {
        public Task<IEnumerable<AccountGroup>> GetAllAsync();
        public Task<Either<IServiceAndFeatureException,AccountGroup>> GetAsync(Guid id);
        public Task<Either<IServiceAndFeatureException, AccountGroup>> GetWithChildAccountsAsync(Guid id);
        public Task<bool> HasAnyChildAccountGroups(Guid Id);
        public Task<bool> HasAnyChildAccounts(Guid Id);
        public Task<Either<IServiceAndFeatureException, AccountGroup>> AddAsync(AccountGroup accountGroup);
        public Task<Either<IServiceAndFeatureException, AccountGroup>> UpdateAsync(AccountGroup accountGroup);
        public Task<Either<IServiceAndFeatureException, List<AccountGroup>>> DeleteAsync(Guid id);
    }
}
