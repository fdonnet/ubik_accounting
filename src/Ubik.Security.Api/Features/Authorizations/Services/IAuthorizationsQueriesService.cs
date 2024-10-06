using LanguageExt;
using Ubik.ApiService.Common.Errors;
using Ubik.Security.Api.Models;
using Ubik.Security.Contracts.Authorizations.Results;

namespace Ubik.Security.Api.Features.Authorizations.Services
{
    public interface IAuthorizationsQueriesService
    {
        Task<Either<IServiceAndFeatureError, Authorization>> GetAsync(Guid id);
    }
}
