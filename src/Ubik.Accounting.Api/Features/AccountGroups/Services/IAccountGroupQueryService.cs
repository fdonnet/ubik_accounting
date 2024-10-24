using LanguageExt;
using Ubik.Accounting.Api.Models;
using Ubik.ApiService.Common.Errors;

namespace Ubik.Accounting.Api.Features.AccountGroups.Services
{
    public interface IAccountGroupQueryService
    {
        Task<IEnumerable<AccountGroup>> GetAllAsync();
        Task<Either<IServiceAndFeatureError, AccountGroup>> GetAsync(Guid id);
        Task<Either<IServiceAndFeatureError, IEnumerable<Account>>> GetChildAccountsAsync(Guid id);
    }
}
