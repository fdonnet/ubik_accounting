﻿using LanguageExt;
using Ubik.ApiService.Common.Errors;
using Ubik.Security.Api.Models;
using Ubik.Security.Contracts.Users.Results;

namespace Ubik.Security.Api.Features.Users.Services
{
    public interface IUsersQueriesService
    {
        Task<Either<IServiceAndFeatureError, User>> GetAsync(Guid id);
        Task<Either<IServiceAndFeatureError, User>> GetAsync(string email);
        Task<Either<IServiceAndFeatureError, UserAdminResult>> GetUserWithAuhtorizationsByTenants(string email);
        Task<Either<IServiceAndFeatureError, Tenant>> GetUserSelectedTenantAsync(Guid userId);
        Task<Either<IServiceAndFeatureError, Tenant>> GetUserTenantAsync(Guid userId, Guid tenantId);
        Task<Either<IServiceAndFeatureError, IEnumerable<Tenant>>> GetUserAllTenantsAsync(Guid userId);
    }
    
}
