﻿using LanguageExt;
using Ubik.ApiService.Common.Errors;
using Ubik.Security.Api.Models;
using Ubik.Security.Contracts.Tenants.Commands;

namespace Ubik.Security.Api.Features.Tenants.Admin.Services
{
    public interface ITenantsAdminCommandsService
    {
        public Task<Either<IServiceAndFeatureError, Tenant>> AddAsync(AddTenantCommand command);
        public Task<Either<IServiceAndFeatureError, Tenant>> UpdateAsync(UpdateTenantCommand command);
        public Task<Either<IServiceAndFeatureError, bool>> ExecuteDeleteAsync(Guid id);
    }
}
