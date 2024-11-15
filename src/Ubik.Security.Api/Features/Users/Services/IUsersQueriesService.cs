using LanguageExt;
using Ubik.ApiService.Common.Errors;
using Ubik.Security.Api.Models;
using Ubik.Security.Contracts.Users.Results;

namespace Ubik.Security.Api.Features.Users.Services
{
    public interface IUsersQueriesService
    {
        Task<Either<IFeatureError, User>> GetAsync(Guid id);
        Task<Either<IFeatureError, User>> GetUserInSelectedTenantAsync(Guid id);
        Task<Either<IFeatureError, IEnumerable<Role>>> GetUserRolesInSelectedTenantAsync(Guid id);
        Task<Either<IFeatureError, Role>> GetUserRoleInSelectedTenantAsync(Guid id, Guid roleId);
        Task<Either<IFeatureError, User>> GetAsync(string email);
        Task<Either<IFeatureError, UserAdminOrMeResult>> GetUserWithAuhtorizationsByTenants(string email);
        Task<Either<IFeatureError, UserAdminOrMeResult>> GetUserWithAuhtorizationsByTenants(Guid id);
        Task<Either<IFeatureError, Tenant>> GetUserSelectedTenantAsync(Guid userId);
        Task<Either<IFeatureError, Tenant>> GetUserTenantAsync(Guid userId, Guid tenantId);
        Task<Either<IFeatureError, IEnumerable<Tenant>>> GetUserAllTenantsAsync(Guid userId);
    }
    
}
