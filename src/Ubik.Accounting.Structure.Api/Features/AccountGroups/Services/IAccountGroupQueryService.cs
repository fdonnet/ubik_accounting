using LanguageExt;
using Ubik.Accounting.Structure.Api.Models;
using Ubik.ApiService.Common.Errors;

namespace Ubik.Accounting.Structure.Api.Features.AccountGroups.Services
{
    public interface IAccountGroupQueryService
    {
        Task<IEnumerable<AccountGroup>> GetAllAsync();
        Task<Either<IServiceAndFeatureError, AccountGroup>> GetAsync(Guid id);
        Task<Either<IServiceAndFeatureError, IEnumerable<Account>>> GetChildAccountsAsync(Guid id);
    }
}
