using LanguageExt;
using Ubik.Accounting.Api.Features.Accounts.Queries.CustomPoco;
using Ubik.Accounting.Api.Models;
using Ubik.ApiService.Common.Errors;

namespace Ubik.Accounting.Api.Features.Accounts.Services
{
    public interface IAccountService
    {
        public Task<Either<IServiceAndFeatureError, Account>> AddAsync(Account account);
        public Task<Either<IServiceAndFeatureError, Account>> UpdateAsync(Account account);
        public Task<Either<IServiceAndFeatureError, bool>> ExecuteDeleteAsync(Guid id);
        public Task<Either<IServiceAndFeatureError, AccountAccountGroup>> AddInAccountGroupAsync(AccountAccountGroup accountAccountGroup);
        public Task<Either<IServiceAndFeatureError, AccountAccountGroup>> DeleteFromAccountGroupAsync(Guid id, Guid accountGroupId);
        public Task<Either<IServiceAndFeatureError, IEnumerable<AccountGroupClassification>>> GetAccountGroupsWithClassificationInfoAsync(Guid id);
        public Task<IEnumerable<AccountAccountGroup>> GetAllAccountGroupLinksAsync();
    }
}
