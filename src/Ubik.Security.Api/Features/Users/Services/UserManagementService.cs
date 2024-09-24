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

        private async Task<Either<IServiceAndFeatureError, User>> ValidateIfNotAlreadyExistsAsync(User user)
        {
            var exists = await _context.Users.AnyAsync(a => a.Email == user.Email);
            return exists
                ? new UserAlreadyExistsError(user.Email)
                : user;
        }

    }
}
