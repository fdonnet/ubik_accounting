using LanguageExt;
using Ubik.Accounting.Structure.Api.Models;
using Ubik.ApiService.Common.Errors;

namespace Ubik.Accounting.Structure.Api.Features.AccountGroups.Services
{
    public interface IAccountGroupQueryService
    {
        Task<IEnumerable<AccountGroup>> GetAllAsync();
        Task<Either<IFeatureError, AccountGroup>> GetAsync(Guid id);
        Task<Either<IFeatureError, IEnumerable<Account>>> GetChildAccountsAsync(Guid id);
    }
}
