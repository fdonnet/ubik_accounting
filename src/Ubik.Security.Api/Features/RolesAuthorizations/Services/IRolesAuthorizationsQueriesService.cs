using LanguageExt;
using Ubik.ApiService.Common.Errors;
using Ubik.Security.Api.Models;

namespace Ubik.Security.Api.Features.RolesAuthorizations.Services
{
    public interface IRolesAuthorizationsQueriesService
    {
        Task<Either<IFeatureError, RoleAuthorization>> GetAsync(Guid id);
        Task<IEnumerable<RoleAuthorization>> GetAllAsync();
    }
}
