using LanguageExt;
using Ubik.ApiService.Common.Errors;
using Ubik.Security.Api.Models;

namespace Ubik.Security.Api.Features.Users.Services
{
    public interface IUserAuthProviderService
    {
        public Task<Either<IServiceAndFeatureError, User>> AddUserAsync(User user);

    }
}
