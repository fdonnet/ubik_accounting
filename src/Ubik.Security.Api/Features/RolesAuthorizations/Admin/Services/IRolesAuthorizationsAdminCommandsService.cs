using LanguageExt;
using Ubik.ApiService.Common.Errors;
using Ubik.Security.Api.Models;
using Ubik.Security.Contracts.RoleAuthorizations.Commands;

namespace Ubik.Security.Api.Features.RolesAuthorizations.Admin.Services
{
    public interface IRolesAuthorizationsAdminCommandsService
    {
        public Task<Either<IServiceAndFeatureError, RoleAuthorization>> AddAsync(AddRoleAuthorizationCommand command);
        public Task<Either<IServiceAndFeatureError, bool>> ExecuteDeleteAsync(Guid id);
    }
}
