using LanguageExt;
using Microsoft.EntityFrameworkCore;
using Ubik.ApiService.Common.Errors;
using Ubik.Security.Api.Models;

namespace Ubik.Security.Api.Features.Users.Services
{
    public interface IUserManagementService
    {
        public Task<Either<IServiceAndFeatureError, User>> AddAsync(User user);
        public Task<Either<IServiceAndFeatureError, bool>> ExecuteDeleteAsync(Guid id);
        public Task<Either<IServiceAndFeatureError, User>> GetAsync(Guid id);
    }
}
