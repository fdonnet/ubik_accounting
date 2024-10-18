using LanguageExt;
using Microsoft.EntityFrameworkCore;
using Ubik.Accounting.Api.Data;
using Ubik.Accounting.Api.Models;
using Ubik.ApiService.Common.Errors;

namespace Ubik.Accounting.Api.Features.AccountGroups.Services
{
    public class AccountGroupQueryService(AccountingDbContext ctx) : IAccountGroupQueryService
    {
        public async Task<Either<IServiceAndFeatureError, IEnumerable<AccountGroup>>> GetAllAsync(Guid id)
        {
            return await ctx.AccountGroups.ToListAsync();
        }
    }
}
