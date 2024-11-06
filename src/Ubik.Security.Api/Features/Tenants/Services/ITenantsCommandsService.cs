using LanguageExt;
using Ubik.ApiService.Common.Errors;
using Ubik.Security.Api.Models;
using Ubik.Security.Contracts.Tenants.Commands;

namespace Ubik.Security.Api.Features.Tenants.Services
{
    public interface ITenantsCommandsService
    {
        public Task<Either<IFeatureError, Tenant>> AddAsync(AddTenantCommand command);
        public Task<Either<IFeatureError, Tenant>> UpdateAsync(UpdateTenantCommand command);
        public Task<Either<IFeatureError, bool>> DeleteAsync(Guid id);
    }
}
