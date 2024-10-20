using LanguageExt;
using Ubik.Accounting.Api.Models;
using Ubik.ApiService.Common.Errors;

namespace Ubik.Accounting.Api.Features.Accounts.Services
{
    public interface IAccountQueryService
    {
        public Task<IEnumerable<Account>> GetAllAsync();
        public Task<IEnumerable<AccountAccountGroup>> GetAllAccountGroupLinksAsync();
        public Task<Either<IServiceAndFeatureError, Account>> GetAsync(Guid id);
    }
}
