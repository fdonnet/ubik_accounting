using LanguageExt;
using Microsoft.EntityFrameworkCore;
using Ubik.Accounting.Structure.Api.Data;
using Ubik.Accounting.Structure.Api.Models;
using Ubik.ApiService.Common.Errors;

namespace Ubik.Accounting.Structure.Api.Features.VatRates.Services
{
    public class VatRateQueryService(AccountingDbContext ctx) : IVatRateQueryService
    {
        public async Task<IEnumerable<VatRate>> GetAllAsync()
        {
            var result = await ctx.VatRates.ToListAsync();

            return result;
        }

        public async Task<Either<IServiceAndFeatureError, VatRate>> GetAsync(Guid id)
        {
            var result = await ctx.VatRates.FindAsync(id);

            return result == null
                ? new ResourceNotFoundError("VatRate", "Id", id.ToString())
                : result;
        }
    }
}
