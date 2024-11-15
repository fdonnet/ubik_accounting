using LanguageExt;
using Ubik.ApiService.Common.Errors;
using Ubik.Security.Api.Models;

namespace Ubik.Security.Api.Features.Roles.Services
{
    public interface IRolesAdminQueriesService
    {
        Task<Either<IFeatureError, Role>> GetAsync(Guid id);
        Task<IEnumerable<Role>> GetAllAsync();
    }
}
