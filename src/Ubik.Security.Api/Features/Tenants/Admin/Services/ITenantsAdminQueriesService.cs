using LanguageExt;
using Ubik.ApiService.Common.Errors;
using Ubik.Security.Api.Models;

namespace Ubik.Security.Api.Features.Tenants.Admin.Services
{
    public interface ITenantsAdminQueriesService
    {
        Task<Either<IServiceAndFeatureError, Tenant>> GetAsync(Guid id);
        Task<IEnumerable<Tenant>> GetAllAsync();
    }
}
