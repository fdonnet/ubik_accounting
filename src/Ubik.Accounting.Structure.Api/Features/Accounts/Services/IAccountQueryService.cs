using LanguageExt;
using Ubik.Accounting.Structure.Api.Features.Accounts.CustomPoco;
using Ubik.Accounting.Structure.Api.Models;
using Ubik.ApiService.Common.Errors;

namespace Ubik.Accounting.Structure.Api.Features.Accounts.Services
{
    public interface IAccountQueryService
    {
        public Task<IEnumerable<Account>> GetAllAsync();
        public Task<IEnumerable<AccountAccountGroup>> GetAllAccountGroupLinksAsync();
        public Task<Either<IServiceAndFeatureError, Account>> GetAsync(Guid id);
        public Task<Either<IServiceAndFeatureError, IEnumerable<AccountGroupClassification>>> GetAccountGroupsWithClassificationInfoAsync(Guid id);
    }
}
