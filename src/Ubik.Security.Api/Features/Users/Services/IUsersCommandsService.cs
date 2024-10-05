using LanguageExt;
using Ubik.ApiService.Common.Errors;
using Ubik.Security.Api.Models;
using Ubik.Security.Contracts.Users.Commands;
using Ubik.Security.Contracts.Users.Results;

namespace Ubik.Security.Api.Features.Users.Services
{
    public interface IUsersCommandsService
    {
        Task<Either<IServiceAndFeatureError, UserStandardResult>> AddAsync(AddUserCommand userCommand);

    }
}
