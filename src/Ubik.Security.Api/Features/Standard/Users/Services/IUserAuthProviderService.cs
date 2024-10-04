using LanguageExt;
using Ubik.ApiService.Common.Errors;
using Ubik.Security.Contracts.Users.Commands;

namespace Ubik.Security.Api.Features.Standard.Users.Services
{
    public interface IUserAuthProviderService
    {
        public Task<Either<IServiceAndFeatureError, bool>> AddUserAsync(AddUserCommand user);

    }
}
