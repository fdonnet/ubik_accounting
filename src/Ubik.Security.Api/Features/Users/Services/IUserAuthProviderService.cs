using LanguageExt;
using Ubik.ApiService.Common.Errors;
using Ubik.Security.Contracts.Users.Commands;

namespace Ubik.Security.Api.Features.Users.Services
{
    public interface IUserAuthProviderService
    {
        public Task<Either<IFeatureError, bool>> AddUserAsync(AddUserCommand user);
        public Task<Either<IFeatureError, bool>> CheckIfUsersPresentInAuth();

    }
}
