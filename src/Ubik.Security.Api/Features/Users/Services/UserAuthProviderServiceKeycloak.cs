using LanguageExt;
using Ubik.ApiService.Common.Errors;
using Ubik.Security.Api.Models;

namespace Ubik.Security.Api.Features.Users.Services
{
    public class UserAuthProviderServiceKeycloak : IUserAuthProviderService
    {
        public async Task<Either<IServiceAndFeatureError, User>> AddUserAsync(User user)
        {
            throw new NotImplementedException();
        }
    }
}
