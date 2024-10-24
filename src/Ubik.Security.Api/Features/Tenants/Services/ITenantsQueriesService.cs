using LanguageExt;
using Ubik.ApiService.Common.Errors;
using Ubik.Security.Api.Models;

namespace Ubik.Security.Api.Features.Tenants.Services
{
    public interface ITenantsQueriesService
    {
        Task<Either<IServiceAndFeatureError, Tenant>> GetAsync(Guid id);
        Task<IEnumerable<Tenant>> GetAllAsync();
    }
}
