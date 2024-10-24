using LanguageExt;
using Ubik.ApiService.Common.Errors;
using Ubik.Security.Api.Models;

namespace Ubik.Security.Api.Features.Authorizations.Services
{
    public interface IAuthorizationsQueriesService
    {
        Task<Either<IServiceAndFeatureError, Authorization>> GetAsync(Guid id);
        Task<IEnumerable<Authorization>> GetAllAsync();
    }
}
