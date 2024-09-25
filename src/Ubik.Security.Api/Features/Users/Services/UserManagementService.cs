using LanguageExt;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Ubik.ApiService.Common.Errors;
using Ubik.ApiService.Common.Services;
using Ubik.Security.Api.Data;
using Ubik.Security.Api.Features.Users.Errors;
using Ubik.Security.Api.Models;

namespace Ubik.Security.Api.Features.Users.Services
{
    public class UserManagementService(SecurityDbContext ctx, ICurrentUserService userService) : IUserManagementService
    {
        private readonly SecurityDbContext _context = ctx;
        private readonly ICurrentUserService _userService = userService;

        public async Task<Either<IServiceAndFeatureError, User>> AddAsync(User user)
        {
            return await ValidateIfNotAlreadyExistsAsync(user).ToAsync()
                .MapAsync(async ac =>
                {
                    ac.Id = NewId.NextGuid();
                    await _context.Users.AddAsync(ac);
                    _context.SetAuditAndSpecialFields();
                    return ac;
                });
        }

        public async Task<Either<IServiceAndFeatureError, bool>> ExecuteDeleteAsync(Guid id)
        {
            return await GetAsync(id).ToAsync()
                .MapAsync(async ac =>
                {
                    await _context.Users.Where(x => x.Id == id).ExecuteDeleteAsync();
                    return true;
                });
        }

        public async Task<Either<IServiceAndFeatureError, User>> GetAsync(Guid id)
        {
            var result = await _context.Users.FindAsync(id);

            return result == null
                ? new UserNotFoundError(id)
                : result;
        }

        private async Task<Either<IServiceAndFeatureError, User>> ValidateIfNotAlreadyExistsAsync(User user)
        {
            var exists = await _context.Users.AnyAsync(a => a.Email == user.Email);
            return exists
                ? new UserAlreadyExistsError(user.Email)
                : user;
        }

    }
}
