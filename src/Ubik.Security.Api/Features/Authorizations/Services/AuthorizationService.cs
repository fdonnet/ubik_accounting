using LanguageExt;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Ubik.ApiService.Common.Errors;
using Ubik.ApiService.Common.Services;
using Ubik.Security.Api.Data;
using Ubik.Security.Api.Models;

namespace Ubik.Security.Api.Features.Authorizations.Services
{
    public class AuthorizationService(SecurityDbContext ctx, ICurrentUserService userService) : IAuthorizationService
    {
        private readonly SecurityDbContext _context = ctx;
        private readonly ICurrentUserService _userService = userService;

        public async Task<Either<IServiceAndFeatureError, Authorization>> AddAsync(Authorization authorization)
        {
            return await ValidateIfNotAlreadyExistsAsync(authorization).ToAsync()
               .MapAsync(async ac =>
               {
                   ac.Id = NewId.NextGuid();
                   await _context.Authorizations.AddAsync(ac);
                   _context.SetAuditAndSpecialFields();
                   return ac;
               });
        }

        private async Task<Either<IServiceAndFeatureError, Authorization>> ValidateIfNotAlreadyExistsAsync(Authorization auth)
        {
            var exists = await _context.Authorizations.AnyAsync(a => a.Code == auth.Code);
            return exists
                ? new ResourceAlreadyExistsError("Authorization", "Code", auth.Code)
                : auth;
        }
    }
}
