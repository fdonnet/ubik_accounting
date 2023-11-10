using Dapper;
using LanguageExt;
using LanguageExt.Common;
using LanguageExt.SomeHelp;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Net.WebSockets;
using System.Reflection.Metadata.Ecma335;
using Ubik.Accounting.Api.Data;
using Ubik.Accounting.Api.Features.Classifications.Exceptions;
using Ubik.Accounting.Api.Features.Classifications.Queries.CustomPoco;
using Ubik.Accounting.Api.Models;
using Ubik.ApiService.Common.Exceptions;
using Ubik.ApiService.Common.Services;

namespace Ubik.Accounting.Api.Features.Classifications.Services
{
    public class ClassificationService : IClassificationService
    {
        private readonly AccountingContext _context;
        private readonly ICurrentUserService _userService;
        public ClassificationService(AccountingContext ctx, ICurrentUserService userService)
        {
            _context = ctx;
            _userService = userService;
        }

        public async Task<bool> IfExistsAsync(Guid id)
        {
            return await _context.Classifications.AnyAsync(a => a.Id == id);
        }

        public async Task<IEnumerable<Classification>> GetAllAsync()
        {
            var result = await _context.Classifications.ToListAsync();

            return result;
        }

        public async Task<Either<IServiceAndFeatureException, Classification>> GetAsync(Guid id)
        {
            var result = await _context.Classifications.FirstOrDefaultAsync(a => a.Id == id);

            return result == null
                ? new ClassificationNotFoundException(id)
                : result;
        }

        /// <summary>
        /// Dapper to get all the account linked to a classification (Postgres)
        /// TODO: see if we can do better to remain multi provider => it seems that I hate EF core include and projections.
        /// TODO:change tenant id array to selected
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Either<IServiceAndFeatureException, IList<Account>>> GetClassificationAccountsAsync(Guid id)
        {
            if ((await GetAsync(id)).IsLeft)
                return new ClassificationNotFoundException(id);

            var p = new DynamicParameters();
            p.Add("@id", id);
            p.Add("@tenantId", _userService.CurrentUser.TenantIds[0]);

            var con = _context.Database.GetDbConnection();
            var sql = """
                SELECT a.*
                FROM "Classifications" c
                INNER JOIN "AccountGroups" ag ON c."Id" = ag."AccountGroupClassificationId"
                INNER JOIN "AccountsAccountGroups" aag on aag."AccountGroupId" = ag."Id"
                INNER JOIN "Accounts" a ON aag."AccountId" = a."Id"
                WHERE a."TenantId" = @tenantId 
                AND c."Id" = @id
                """;

            return (await con.QueryAsync<Account>(sql, p)).ToList();
            
        }

        /// <summary>
        /// Dapper to get all the account not linked to a specific classification (Postgres)
        /// TODO:change tenant id array to selected
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Either<IServiceAndFeatureException, IList<Account>>> GetClassificationAccountsMissingAsync(Guid id)
        {
            if ((await GetAsync(id)).IsLeft)
                return new ClassificationNotFoundException(id);

            var p = new DynamicParameters();
            p.Add("@id", id);
            p.Add("@tenantId", _userService.CurrentUser.TenantIds[0]);

            var con = _context.Database.GetDbConnection();
            var sql = """
                SELECT a1.*
                FROM "Accounts" a1
                WHERE a1."TenantId" = @tenantId
                AND a1."Id" NOT IN (
                	SELECT a."Id"
                	FROM "Classifications" c
                	INNER JOIN "AccountGroups" ag ON c."Id" = ag."AccountGroupClassificationId"
                	INNER JOIN "AccountsAccountGroups" aag on aag."AccountGroupId" = ag."Id"
                	INNER JOIN "Accounts" a ON aag."AccountId" = a."Id"
                	WHERE c."Id" = @id)
                """;

            return (await con.QueryAsync<Account>(sql, p)).ToList();
        }

        public async Task<Either<IServiceAndFeatureException, ClassificationStatus>> GetClassificationStatus(Guid id)
        {
            var missingAccount = await GetClassificationAccountsMissingAsync(id);

            if(missingAccount.IsLeft)
                return new ClassificationNotFoundException(id);

            var classificationStatus = missingAccount.Match(
                Right: c => 
                {
                    ClassificationStatus status = c.Any()
                        ? new ClassificationStatus
                        {
                            Id = id,
                            IsReady = false,
                            MissingAccounts = c
                        }
                        : new ClassificationStatus
                        {
                            Id = id,
                            IsReady = false
                        };
                    return status;
                },
                Left: err => throw new Exception("Cannot be in left state at this point"));

            return classificationStatus;
        }
    }
}
