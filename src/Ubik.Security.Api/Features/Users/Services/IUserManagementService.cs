using LanguageExt;
using Ubik.ApiService.Common.Errors;
using Ubik.Security.Api.Models;

namespace Ubik.Security.Api.Features.Users.Services
{
    public interface IUserManagementService
    {
        public Task<Either<IServiceAndFeatureError, User>> AddAsync(User user);
    }
}
