using LanguageExt;
using Ubik.Accounting.Api.Models;
using Ubik.ApiService.Common.Errors;

namespace Ubik.Accounting.Api.Features.AccountGroups.Services
{
    public interface IAccountGroupService
    {
        public Task<IEnumerable<AccountGroup>> GetAllAsync();
        public Task<Either<IServiceAndFeatureError,AccountGroup>> GetAsync(Guid id);
        public Task<Either<IServiceAndFeatureError, AccountGroup>> GetWithChildAccountsAsync(Guid id);
        public Task<Either<IServiceAndFeatureError, AccountGroup>> AddAsync(AccountGroup accountGroup);
        public Task<Either<IServiceAndFeatureError, AccountGroup>> UpdateAsync(AccountGroup accountGroup);
        public Task<Either<IServiceAndFeatureError, List<AccountGroup>>> DeleteAsync(Guid id);
    }
}
