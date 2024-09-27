using LanguageExt;
using Microsoft.EntityFrameworkCore;
using Ubik.ApiService.Common.Errors;
using Ubik.ApiService.Common.Services;
using Ubik.Security.Api.Data;
using Ubik.Security.Api.Features.Users.Errors;
using Ubik.Security.Api.Models;

namespace Ubik.Security.Api.Features.Authorizations.Services
{
    //public class AuthorizationService(SecurityDbContext ctx, ICurrentUserService userService) : IAuthorizationService
    //{
    //    private readonly SecurityDbContext _context = ctx;
    //    private readonly ICurrentUserService _userService = userService;
    //    //public async Task Task<Either<IServiceAndFeatureError, Authorization>> AddAsync(Authorization authorization)
    //    //{
    //    //    throw new NotImplementedException();
    //    //}

    //    ////TODO Already exist need to be a standard error type. (no need to duplicate)
    //    //private async Task<Either<IServiceAndFeatureError, Authorization>> ValidateIfNotAlreadyExistsAsync(Authorization auth)
    //    //{
    //    //    var exists = await _context.Authorizations.AnyAsync(a => a.Code == auth.Code);
    //    //    return exists
    //    //        ? new AuthorizationAlreadyExistsError(auth.Code)
    //    //        : auth;
    //    //}
    //}
}
