using LanguageExt;
using Ubik.ApiService.Common.Errors;
using Ubik.Security.Api.Models;
using Ubik.Security.Contracts.Authorizations.Commands;
using Ubik.Security.Contracts.RoleAuthorizations.Commands;
using Ubik.Security.Contracts.Roles.Commands;

namespace Ubik.Security.Api.Features.Roles.Admin.Services
{
    public interface IRolesAdminCommandsService
    {
        public Task<Either<IServiceAndFeatureError, Role>> AddAsync(AddRoleCommand authorizationCommand);
        public Task<Either<IServiceAndFeatureError, Role>> UpdateAsync(UpdateRoleCommand authorizationCommand);
        public Task<Either<IServiceAndFeatureError, bool>> ExecuteDeleteAsync(Guid id);
    }
}
