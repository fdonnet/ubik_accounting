using LanguageExt;
using Ubik.ApiService.Common.Errors;
using Ubik.Security.Api.Models;

namespace Ubik.Security.Api.Features.Authorizations.Admin.Services
{
    public interface IAuthorizationsAdminQueriesService
    {
        Task<Either<IServiceAndFeatureError, Authorization>> GetAsync(Guid id);
        Task<IEnumerable<Authorization>> GetAllAsync();
    }
}
