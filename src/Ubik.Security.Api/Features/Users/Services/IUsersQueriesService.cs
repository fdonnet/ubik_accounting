using LanguageExt;
using Ubik.ApiService.Common.Errors;
using Ubik.Security.Api.Models;

namespace Ubik.Security.Api.Features.Users.Services
{
    public interface IUsersQueriesService
    {
        Task<Either<IServiceAndFeatureError, User>> GetAsync(Guid id);
        Task<Either<IServiceAndFeatureError, User>> GetAsync(string email);
    }
}
