using LanguageExt;
using Ubik.ApiService.Common.Errors;
using Ubik.Security.Api.Models;

namespace Ubik.Security.Api.Features.Authorizations.Services
{
    public interface IAuthorizationService
    {
        public Task<Either<IServiceAndFeatureError, Authorization>> AddAsync(Authorization authorization);
    }
}
