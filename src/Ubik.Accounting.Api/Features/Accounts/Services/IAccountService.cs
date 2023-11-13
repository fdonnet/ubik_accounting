using LanguageExt;
using Ubik.Accounting.Api.Features.Accounts.Queries.CustomPoco;
using Ubik.Accounting.Api.Models;
using Ubik.ApiService.Common.Exceptions;

namespace Ubik.Accounting.Api.Features.Accounts.Services
{
    public interface IAccountService
    {
        public Task<IEnumerable<Account>> GetAllAsync();
        public Task<Either<IServiceAndFeatureException,Account>> GetAsync(Guid id);
        public Task<Either<IServiceAndFeatureException, IList<AccountGroupClassification>>> GetAccountGroupsAsync(Guid id);
        public Task<bool> IfExistsAsync(string accountCode);
        public Task<bool> IfExistsWithDifferentIdAsync(string accountCode, Guid currentId);
        public Task<Either<IServiceAndFeatureException, Account>> AddAsync(Account account);
        public Task<Either<IServiceAndFeatureException, Account>> UpdateAsync(Account account);
        public Task<Either<IServiceAndFeatureException, bool>> ExecuteDeleteAsync(Guid id);
        public Task<bool> IfExistsCurrencyAsync(Guid currencyId);
        public Task<Either<IServiceAndFeatureException, AccountAccountGroup>> AddInAccountGroupAsync(Guid id, Guid accountGroupId);
        public Task<Either<IServiceAndFeatureException, AccountAccountGroup>> DeleteFromAccountGroupAsync(Guid id, Guid accountGroupId);
    }
}
