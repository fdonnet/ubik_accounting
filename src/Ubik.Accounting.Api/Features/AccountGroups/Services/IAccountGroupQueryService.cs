using LanguageExt;
using Ubik.Accounting.Api.Models;
using Ubik.ApiService.Common.Errors;

namespace Ubik.Accounting.Api.Features.AccountGroups.Services
{
    public interface IAccountGroupQueryService
    {
        Task<Either<IServiceAndFeatureError, IEnumerable<AccountGroup>>> GetAllAsync(Guid id);
    }
}
