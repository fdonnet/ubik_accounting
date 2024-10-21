using LanguageExt;
using Microsoft.EntityFrameworkCore;
using Ubik.Accounting.Api.Data;
using Ubik.Accounting.Api.Models;
using Ubik.ApiService.Common.Errors;
using Ubik.ApiService.Common.Services;

namespace Ubik.Accounting.Api.Features.Classifications.Services
{
    public class ClassificationQueryService(AccountingDbContext ctx, ICurrentUser currentUser) : IClassificationQueryService
    {
        public async Task<IEnumerable<Classification>> GetAllAsync()
        {
            return await ctx.Classifications.ToListAsync();
        }

        public async Task<Either<IServiceAndFeatureError, Classification>> GetAsync(Guid id)
        {
            var result = await ctx.Classifications.FindAsync(id);

            return result == null
                ? new ResourceNotFoundError("Classification", "Id", id.ToString())
                : result;
        }
    }
}
