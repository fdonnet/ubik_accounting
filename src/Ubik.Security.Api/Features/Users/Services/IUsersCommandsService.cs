﻿using LanguageExt;
using Ubik.ApiService.Common.Errors;
using Ubik.Security.Api.Models;
using Ubik.Security.Contracts.Tenants.Commands;
using Ubik.Security.Contracts.Users.Commands;

namespace Ubik.Security.Api.Features.Users.Services
{
    public interface IUsersCommandsService
    {
        Task<Either<IServiceAndFeatureError, User>> AddAsync(AddUserCommand userCommand);
        Task<Either<IServiceAndFeatureError, Tenant>> AddNewTenantAsync(Guid userId,AddTenantCommand command);
        Task<Either<IServiceAndFeatureError, Role>> AddRoleInTenantAsync(Guid userId, Guid roleId);
    }
}
