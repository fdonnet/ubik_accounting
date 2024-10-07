﻿using Dapper;
using LanguageExt;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Ubik.ApiService.Common.Errors;
using Ubik.ApiService.Common.Exceptions;
using Ubik.Security.Api.Data;
using Ubik.Security.Api.Models;
using Ubik.Security.Contracts.RoleAuthorizations.Commands;
using Ubik.Security.Contracts.RoleAuthorizations.Events;

namespace Ubik.Security.Api.Features.RolesAuthorizations.Admin.Services
{
    public class RolesAuthorizationsAdminQueriesService(SecurityDbContext ctx) : IRolesAuthorizationsAdminQueriesService
    {
        public async Task<IEnumerable<RoleAuthorization>> GetAllAsync()
        {
            var con = ctx.Database.GetDbConnection();
            var sql = """
                      SELECT ra.* 
                      FROM roles_authorizations ra
                      INNER JOIN roles r ON r.id = ra.role_id
                      WHERE r.tenant_id IS NULL
                      """;

            return await con.QueryAsync<RoleAuthorization>(sql);
        }

        public async Task<Either<IServiceAndFeatureError, RoleAuthorization>> GetAsync(Guid id)
        {
            var p = new DynamicParameters();
            p.Add("@id", id);

            var con = ctx.Database.GetDbConnection();
            var sql = """
                      SELECT ra.* 
                      FROM roles_authorizations ra
                      INNER JOIN roles r ON r.id = ra.role_id
                      WHERE ra.id = @id
                      AND r.tenant_id IS NULL
                      """;

            var result = await con.QueryFirstOrDefaultAsync<RoleAuthorization>(sql, p);

            return result == null
                ? new ResourceNotFoundError("RoleAuthorization", "Id", id.ToString())
                : result;
        }
    }
}